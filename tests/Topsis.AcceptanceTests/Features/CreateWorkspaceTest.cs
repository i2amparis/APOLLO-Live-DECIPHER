using System;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Features;
using Xunit;
using Xunit.Abstractions;

namespace Topsis.AcceptanceTests.Features
{

    public class CreateWorkspaceTest : AcceptanceTest
    {

        public CreateWorkspaceTest(SliceFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
            _fixture.SetContext<CreateWorkspaceTest>(Scenario.Workspace_Create);
        }

        [Fact]
        public async Task Create_Workspace_TestAsync()
        {
            // arrange
            var email = $"{new Random().Next(1_000, 10_000)}@email.com";
            var moderatorId = await _fixture.SendAsync(new CreateModerator.Command(email, "password", "George", "Moderatorakis"));
            _fixture.SwitchUser(moderatorId);

            // act
            short criteriaNo = 3;
            short alternativeNo = 10;
            string title = "my title";
            var id = await _fixture.SendAsync(new Application.Features.CreateWorkspace.Command 
            { 
                Title = title,  
                CriteriaNo = criteriaNo,
                AlternativesNo = alternativeNo
            });

            // assert
            var workspace = (await _fixture.SendAsync(new GetWorkspace.ById.Request(id))).Result;
            Assert.NotNull(workspace);
            Assert.Equal(moderatorId, workspace.UserId);

            Assert.Equal(title, workspace.Title);
            Assert.Equal(criteriaNo, workspace.Questionnaire.Criteria.Count);
            Assert.Equal(alternativeNo, workspace.Questionnaire.Alternatives.Count);
        }
    }
}
