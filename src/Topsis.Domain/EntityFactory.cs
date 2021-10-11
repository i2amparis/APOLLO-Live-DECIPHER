using System;
using Topsis.Domain.Contracts;

namespace Topsis.Domain
{
    public static class EntityFactory
    {
        public static Workspace CreateQuestionnaire(string title, 
            int criteriaNo,
            int alternativesNo,
            IUserContext userContext)
        {
            var result = new Workspace()
            {
                Title = title,
                UserId = userContext.UserId
            };

            result.Questionnaire = new Questionnaire();
            result.Questionnaire.Criteria.AddRange(Criterion.Bulk(criteriaNo));
            result.Questionnaire.Alternatives.AddRange(Alternative.Bulk(alternativesNo));
            result.Questionnaire.SetSettings(QuestionnaireSettings.Default());

            return result;
        }

        public static string NewId()
        {
            return Normalize(Guid.NewGuid().ToString());
        }

        public static string Normalize(string id)
        {
            return id?.ToUpper();
        }
    }
}
