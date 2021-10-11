using System;
using System.Collections.Generic;
using System.Linq;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;

namespace Topsis.Adapters.Algorithm.Unit.Test
{
    internal static class TopsisBuilder
    {
        public static TopsisInput BuildInput(Questionnaire questionnaire, 
            IEnumerable<StakeholderVote> answers)
        {
            return new TopsisInput() { Questionnaire = questionnaire, Votes = answers.ToList() };
        }

        #region [ Stakeholders ]
        private static IEnumerable<StakeholderVote> BuildAnswers(Questionnaire questionnaire, 
            IEnumerable<ApplicationUser> stakeholders)
        {
            foreach (var sh in stakeholders)
            {
                var vote = new StakeholderVote() { ApplicationUserId = sh.Id };

                foreach (var alt in questionnaire.Alternatives)
                {
                    foreach (var c in questionnaire.Criteria)
                    {
                        vote.Answers.Add(new StakeholderAnswer
                        {
                            Alternative = alt,
                            AlternativeId = alt.Id,
                            Criterion = c,
                            CriterionId = c.Id,
                            Value = new Random().Next(1,5)
                        });
                    }
                }

                yield return vote;
            }
        }

        private static IEnumerable<ApplicationUser> BuildStakeholders(int stakeholdersNo)
        {
            for (int i = 1; i <= stakeholdersNo; i++)
            {
                yield return new ApplicationUser() { Id = i.ToString() };
            }
        }
        #endregion

        #region [ Questionnaire ]
        internal static Workspace BuildWorkspace(IEnumerable<Alternative> alternatives,
            IEnumerable<Criterion> criteria)
        {
            var result = new Workspace
            {
                Questionnaire = new Questionnaire
                {
                    Alternatives = alternatives.ToList(),
                    Criteria = criteria.ToList()
                }
            };

            result.ChangeQuestionnaireSettings(OutputLinguisticScale.Scale5, 1, 4);
            return result;
        }

        public static IEnumerable<Alternative> BuildAlternatives(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return BuildAlternative(i);
            }
        }

        public static IEnumerable<Criterion> BuildCriteria(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return BuildCriterion(i);
            }
        }

        private static Alternative BuildAlternative(int i)
        {
            return new Alternative() { Id = i, Title = $"Alternative {i}" };
        }

        private static Criterion BuildCriterion(int i)
        {
            return new Criterion() { Id = i, Title = $"Criterion {i}" };
        }
        #endregion
    }
}