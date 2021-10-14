using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Domain;
using Xunit;

namespace Topsis.Adapters.Algorithm.Unit.Test
{
    public class AnalysisTest
    {
        [Fact]
        public async void Should_Calculate_Topsis_Results_For_2_Stakeholders()
        {
            var workspace = BuildMinimalWorkshop();
            var answers1 = BuildMinimalStakeholderAnswers("s1", 3);
            var answers2 = BuildMinimalStakeholderAnswers("s2", 4);

            var sut = new TopsisAnalyzer();
            var output = await sut.AnalyzeAsync(workspace, new Dictionary<int, string>(), answers1.Union(answers2).ToArray());

            Assert.NotNull(output);
            Assert.NotEmpty(output.StakeholderTopsis);
            Assert.NotEmpty(output.StakeholdersConsensus);
        }

        [Fact]
        public async void Should_Calculate_Topsis_Results_For_Stakeholder()
        {
            var workspace = BuildExcelExp16Workshop();
            var answers = BuildExcelExp16StakeholderAnswers();

            var sut = new TopsisAnalyzer();
            var output = await sut.AnalyzeAsync(workspace, new Dictionary<int, string>(), answers);

            Assert.NotNull(output);
            Assert.NotEmpty(output.StakeholderTopsis);
            Assert.NotEmpty(output.StakeholdersConsensus);
        }

        [Fact]
        public async void Should_Run_Calculation_For_3_Criteria_15_Alternatives_And_10000_Stakeholders_Under_6_Seconds()
        {
            var workspace = BuildExcelExp16Workshop();
            var answers = MultipleBuildExcelExp16StakeholderAnswers(10_000);

            var sw = Stopwatch.StartNew();
            var sut = new TopsisAnalyzer();
            var output = await sut.AnalyzeAsync(workspace, new Dictionary<int, string>(), answers);
            sw.Stop();

            var secs = sw.ElapsedMilliseconds / 1000;
            Assert.True(secs < 6);
        }

        #region [ Helpers ]

        private static Workspace BuildMinimalWorkshop()
        {
            // https://docs.google.com/forms/d/1BYr1h7Z21n-h1f5xd2V0wjt7fZRMmgzuQLLslNZdpgY/viewform?edit_requested=true
            var alternatives = new[] {
                new Alternative() { Id = 1 , Title = "SDG 1: No Poverty"},
                new Alternative() { Id = 2 , Title = "SDG 2: Zero Hunger"}
            };

            var criteria = new[] {
                new Criterion(){ Id = 1, Title = "Importance", Type = CriterionType.Benefit },
                new Criterion(){ Id = 2, Title = "Relevance", Type = CriterionType.Benefit },
                new Criterion(){ Id = 3, Title = "Trend", Type = CriterionType.Benefit }
            };

            return TopsisBuilder.BuildWorkspace(alternatives, criteria);
        }

        private static StakeholderAnswerDto[] BuildMinimalStakeholderAnswers(string stakeholderId = "1", double offset = 4)
        {
            return new[] {
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 1, AnswerValue = offset-1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 2, AnswerValue = offset-2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 3, AnswerValue = offset-1, CriterionWeight = 4 },


                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 1, AnswerValue = offset, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 2, AnswerValue = offset-1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 3, AnswerValue = offset-1, CriterionWeight = 4 },
            };
        }

        private StakeholderAnswerDto[] MultipleBuildExcelExp16StakeholderAnswers(int stakeholderCount = 1000)
        {
            var result = new List<StakeholderAnswerDto>();
            for (int i = 0; i < stakeholderCount; i++)
            {
                result.AddRange(BuildExcelExp16StakeholderAnswers(i.ToString()));
            }

            return result.ToArray();
        }

        private static StakeholderAnswerDto[] BuildParisReinforceStakeholderAnswers(string stakeholderId = "1")
        {
            return new[] {
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 1, AnswerValue = 5, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 1, AnswerValue = 3, CriterionWeight = 4 },

                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 2, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 2, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },

                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 3, AnswerValue = 5, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 3, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 3, AnswerValue = 4, CriterionWeight = 4 },

                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 4, AnswerValue = 5, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 4, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 4, AnswerValue = 1, CriterionWeight = 4 },
            };
        }

        private static Workspace BuildParisReinforceWorkshop()
        {
            // https://docs.google.com/forms/d/1BYr1h7Z21n-h1f5xd2V0wjt7fZRMmgzuQLLslNZdpgY/viewform?edit_requested=true
            var alternatives = new[] {
                new Alternative() { Id = 1 , Title = "R1. Απραξία πολιτικής"},
                new Alternative() { Id = 2 , Title = "R2. Πολιτική αστάθεια"},
                new Alternative() { Id = 3 , Title = "R3. Έλλειψη θεσμικής ικανότητας"}
            };

            var criteria = new[] {
                new Criterion(){ Id = 1, Title = "C1. Πιθανότητα εκδήλωσης", Type = CriterionType.Cost },
                new Criterion(){ Id = 2, Title = "C2. Επίπεδο ανησυχίας", Type = CriterionType.Cost },
                new Criterion(){ Id = 3, Title = "C3. Επίδραση στο πλαίσιο πολιτικής", Type = CriterionType.Cost },
                new Criterion(){ Id = 4, Title = "C4. Δυνατότητα μετριασμού", Type = CriterionType.Benefit }
            };

            var workspace = TopsisBuilder.BuildWorkspace(alternatives, criteria);
            return workspace;
        }

        private static Workspace BuildExcelExp16Workshop()
        {
            // https://docs.google.com/forms/d/1BYr1h7Z21n-h1f5xd2V0wjt7fZRMmgzuQLLslNZdpgY/viewform?edit_requested=true
            var alternatives = new[] {
                new Alternative() { Id = 1 , Title = "SDG 1: No Poverty"},
                new Alternative() { Id = 2 , Title = "SDG 2: Zero Hunger"},
                new Alternative() { Id = 3 , Title = "SDG 3: Good Health and Well-Being"},
                new Alternative() { Id = 4 , Title = "SDG 4: Quality Education"},
                new Alternative() { Id = 5 , Title = "SDG 5: Gender Equality"},
                new Alternative() { Id = 6 , Title = "SDG 6: Clean Water and Sanitation"},
                new Alternative() { Id = 7 , Title = "SDG 7: Afforadable and Clean Energy"},
                new Alternative() { Id = 8 , Title = "SDG 8: Decent Work and Economic Growth"},
                new Alternative() { Id = 9 , Title = "SDG 9: Industry, Innovation and Infrastructure"},
                new Alternative() { Id = 10 , Title = "SDG 10: Reduced Inequalities"},
                new Alternative() { Id = 11 , Title = "SDG 11: Sustainable Cities and Communities"},
                new Alternative() { Id = 12 , Title = "SDG 12: Responsible Consumption and Production"},
                new Alternative() { Id = 13 , Title = "SDG 13: Life Below Water"},
                new Alternative() { Id = 14 , Title = "SDG 14: Life of land"},
                new Alternative() { Id = 15 , Title = "SDG 15: Peace, Justice and Strong Institutions"},
            };

            var criteria = new[] {
                new Criterion(){ Id = 1, Title = "Importance", Type = CriterionType.Benefit },
                new Criterion(){ Id = 2, Title = "Relevance", Type = CriterionType.Benefit },
                new Criterion(){ Id = 3, Title = "Trend", Type = CriterionType.Benefit }
            };

            return TopsisBuilder.BuildWorkspace(alternatives, criteria);
        }

        private static StakeholderAnswerDto[] BuildExcelExp16StakeholderAnswers(string stakeholderId = "1")
        {
            return new[] {
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 1, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 1, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 4, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 5, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 6, CriterionId = 1, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 7, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 8, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 9, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 10, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 11, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 12, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 13, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 14, CriterionId = 1, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 15, CriterionId = 1, AnswerValue = 1, CriterionWeight = 4 },

                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 2, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 2, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 4, CriterionId = 2, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 5, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 6, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 7, CriterionId = 2, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 8, CriterionId = 2, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 9, CriterionId = 2, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 10, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 11, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 12, CriterionId = 2, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 13, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 14, CriterionId = 2, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 15, CriterionId = 2, AnswerValue = 1, CriterionWeight = 4 },

                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 1, CriterionId = 3, AnswerValue = 1, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 2, CriterionId = 3, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 3, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 4, CriterionId = 3, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 5, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 6, CriterionId = 3, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 7, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 8, CriterionId = 3, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 9, CriterionId = 3, AnswerValue = 2, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 10, CriterionId = 3, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 11, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 12, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 13, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 14, CriterionId = 3, AnswerValue = 4, CriterionWeight = 4 },
                new StakeholderAnswerDto(){ StakeholderId = stakeholderId, AlternativeId = 15, CriterionId = 3, AnswerValue = 3, CriterionWeight = 4 },
            };
        }

        #endregion
    }
}
