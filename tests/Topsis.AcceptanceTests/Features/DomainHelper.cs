using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Features;
using Topsis.Domain;
using Topsis.Domain.Common;
using static Topsis.Application.Features.Vote.Command;

namespace Topsis.AcceptanceTests.Features
{
    internal static class DomainHelper
    {
        #region [ Workspace ]
        public static async Task<Workspace> BuildFinalizedWorkspace(this SliceFixture fixture)
        {
            var id = await fixture.SendAsync(new CreateWorkspace.Command
            {
                Title = "my title",
                CriteriaNo = 3,
                AlternativesNo = 5
            });

            await ChangeStatus(fixture, id, WorkspaceStatus.Published, WorkspaceStatus.AcceptingVotes);
            await ChangeStatus(fixture, id, WorkspaceStatus.Finalized);

            var workspace = await GetWorkspaceAsync(fixture, id);
            return await Vote(fixture, workspace);
        }

        public static async Task<Workspace> GetWorkspaceAsync(SliceFixture fixture, string id)
        {
            var request = new GetWorkspace.ById.Request(id);
            return (await fixture.SendAsync(request)).Result;
        }

        public static async Task<Workspace> Vote(this SliceFixture fixture, Workspace workspace, int numberOfStakeholders = 10)
        {
            var stakeholders = await CreateStakeholders(fixture, numberOfStakeholders);
            foreach (var stakeholder in stakeholders)
            {
                await AddVoteAsync(fixture, workspace, stakeholder);
            }

            return await GetWorkspaceAsync(fixture, workspace.Id.Hash());
        }

        public static async Task ChangeStatus(this SliceFixture fixture, string id, params WorkspaceStatus[] statuses)
        {
            foreach (var item in statuses)
            {
                await fixture.SendAsync(new EditWorkspace.ChangeStatusCommand
                {
                    Status = item,
                    WorkspaceId = id
                });
            }
        }

        public static async Task AddVoteAsync(this SliceFixture fixture, Workspace workspace, ApplicationUser stakeholder)
        {
            fixture.SwitchUser(stakeholder.Id);

            var answers = new List<StakeholderAnswerDto>();
            var criteriaImportance = new Dictionary<int, int>();
            foreach (var criterion in workspace.Questionnaire.Criteria)
            {
                criteriaImportance[criterion.Id] = new Random().Next(1, 3);
                foreach (var alternative in workspace.Questionnaire.Alternatives)
                {
                    answers.Add(new StakeholderAnswerDto
                    {
                        AlternativeId = alternative.Id,
                        CriterionId = criterion.Id,
                        Value = new Random().Next(1, 5)
                    });
                }
            }

            await fixture.SendAsync(new Vote.Command
            {
                Id = workspace.Id.Hash(),
                Answers = answers,
                CriteriaImportance = criteriaImportance
            });
        }

        #endregion

        #region [ Users ]
        public static async Task<string> CreateModeratorAsync(this SliceFixture fixture,
            string email = null,
            string password = null,
            string firstname = null,
            string lastname = null,
            bool switchToUser = true)
        {
            // arrange
            var moderatorId = await fixture.SendAsync(new CreateModerator.Command(
                email ?? "moderator1@ntua.gr",
                password ?? "password",
                firstname ?? "George",
                lastname ?? "Moderatorakis"));

            if (switchToUser)
            {
                fixture.SwitchUser(moderatorId);
            }

            return moderatorId;
        }

        public static async Task<List<ApplicationUser>> CreateStakeholders(this SliceFixture fixture,
            int number = 10)
        {
            var countries = await fixture.ExecuteDbContextAsync(db => db.WsCountries.ToArrayAsync());
            var jobCategories = await fixture.ExecuteDbContextAsync(db => db.WsJobCategories.ToArrayAsync());

            var result = new List<ApplicationUser>();
            for (int i = 0; i < number; i++)
            {
                var country = countries[new Random().Next(0, countries.Length - 1)];
                var jobCategory = jobCategories[new Random().Next(0, jobCategories.Length - 1)];

                var stakeholder = await fixture.SendAsync(new CreateStakeholder.Command
                {
                    Country = country,
                    JobCategory = jobCategory
                });

                result.Add(stakeholder);
            }

            return result;
        }

        #endregion
    }
}