using System;
using System.Collections.Generic;
using System.Linq;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Features;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class StakeholderVoteViewModel
    {
        public StakeholderVoteViewModel()
        {
            Tips = new Dictionary<int, FeedbackTip>();
        }

        public IList<KeyValuePair<int, string>> AlternativesOrdered { get; set; }
        public IList<Criterion> CriteriaOrdered { get; set; }
        public List<NameValueOption> AlternativeRange { get; set; }
        public string WorkspaceTitle { get; set; }
        public string WorkspaceId { get; set; }
        public List<NameValueOption> CriteriaImportanceRange { get; set; }
        public List<StakeholderAnswerDto> StakeholderAnswers { get; private set; }
        public WorkspaceStatus WorkspaceStatus { get; set; }
        public IDictionary<int, FeedbackTip> Tips { get; private set; }
        public QuestionnaireSettings Settings { get; set; }

        public void AddStakeholderAnswers(List<StakeholderAnswerDto> answers, WorkspaceReportViewModel report)
        {
            StakeholderAnswers = answers;
            Tips = answers.Any() 
                ? report.Tips.ToDictionary(x => x.AlternativeId, x => x)
                : new Dictionary<int, FeedbackTip>();
        }

        /// <summary>
        /// when user 
        /// </summary>
        /// <param name="answers"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Accept(List<Vote.Command.StakeholderAnswerDto> answers)
        {
            Func<int, int, string> build_key = (cid, altid) => $"{cid}_{altid}";
            var changedAnswers = answers.ToDictionary(x => build_key(x.CriterionId, x.AlternativeId), x => x);
            foreach (var item in StakeholderAnswers)
            {
                var key = build_key(item.CriterionId, item.AlternativeId);
                if (changedAnswers.TryGetValue(key, out var newAnswer))
                {
                    item.AnswerValue = newAnswer.Value;
                }
            }
        }
    }
}
