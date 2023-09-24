using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Security;

namespace Topsis.Adapters.Encryption.Services
{
    internal class RecaptchaService : IRecaptchaService
    {
        private readonly RecaptchaSettings _settings;
        private readonly ILogger<RecaptchaService> _logger;
        private RecaptchaEnterpriseServiceClient _client;
        private readonly ProjectName _projectName;

        public RecaptchaService(IOptions<RecaptchaSettings> options, ILogger<RecaptchaService> logger)
        {
            _settings = options.Value;
            _logger = logger;
            _projectName = new ProjectName(_settings.ProjectId);
        }

        public async Task<bool> ValidateRecaptchaAsync(string token, string action)
        {
            var client = EnsureClient();

            // Build the assessment request.
            var assessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    // Set the properties of the event to be tracked.
                    Event = new Event()
                    {
                        SiteKey = _settings.SiteKey,
                        Token = token,
                        ExpectedAction = action
                    },
                },
                ParentAsProjectName = _projectName
            };

            Assessment response = await _client.CreateAssessmentAsync(assessmentRequest);

            // Check if the token is valid.
            if (response.TokenProperties.Valid == false)
            {
                _logger.LogInformation("The CreateAssessment call failed because the token was: " +
                    response.TokenProperties.InvalidReason.ToString());
                return false;
            }

            // Check if the expected action was executed.
            if (response.TokenProperties.Action != action)
            {
                _logger.LogInformation("The action attribute in reCAPTCHA tag is: " +
                    response.TokenProperties.Action.ToString());
                _logger.LogInformation("The action attribute in the reCAPTCHA tag does not " +
                    "match the action you are expecting to score");
                return false;
            }

            // Get the risk score and the reason(s).
            // For more information on interpreting the assessment,
            // see: https://cloud.google.com/recaptcha-enterprise/docs/interpret-assessment
            _logger.LogDebug("The reCAPTCHA score is: " + ((decimal)response.RiskAnalysis.Score));
            foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
            {
                _logger.LogDebug($"Risk reason: {reason}");
            }

            return true;
        }

        private RecaptchaEnterpriseServiceClient EnsureClient()
        {
            if (_client != null)
            {
                return _client;
            }

            try
            {
                _client = RecaptchaEnterpriseServiceClient.Create();
                return _client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RecaptchaEnterpriseServiceClient. Check env variable 'GOOGLE_APPLICATION_CREDENTIALS'.");
                throw;
            }
        }
    }
}
