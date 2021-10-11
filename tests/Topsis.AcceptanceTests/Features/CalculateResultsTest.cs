using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Features;
using Topsis.Domain.Common;
using Xunit;
using Xunit.Abstractions;

namespace Topsis.AcceptanceTests.Features
{

    public class CalculateResultsTest : AcceptanceTest
    {

        public CalculateResultsTest(SliceFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
            _fixture.SetContext<CalculateResultsTest>(Scenario.Workspace_CalculateResults);
        }

        [Fact]
        public async Task GenerateResults_TestAsync()
        {
            var moderatorId = await _fixture.CreateModeratorAsync();

            // create workspace.
            var workspace = await _fixture.BuildFinalizedWorkspace();
            workspace = await _fixture.Vote(workspace);

            await _fixture.SendAsync(new CalculateResults.Command
            {
                WorkspaceId = workspace.Id.Hash()
            });

            var result = await _fixture.ExecuteDbContextAsync(
                db => db.WsWorkspacesReports.FirstOrDefaultAsync(x => x.Id == workspace.Id));

            Assert.NotNull(result);
        }
    }
}
