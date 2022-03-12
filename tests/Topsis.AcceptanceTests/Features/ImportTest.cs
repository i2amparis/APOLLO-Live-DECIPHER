using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Features;
using Topsis.Domain.Common;
using Xunit;
using Xunit.Abstractions;

namespace Topsis.AcceptanceTests.Features
{

    public class ImportTest : AcceptanceTest
    {

        public ImportTest(SliceFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            _fixture.SetContext<ImportTest>(Scenario.Workspace_CalculateResults);
        }

        [Theory]
        [InlineData("input_mcda_dlci.xlsx")]
        [InlineData("input_mcda_tech.xlsx")]
        public async Task Dlci_Test(string filename)
        {
            // arrange.
            var random = Guid.NewGuid().GetHashCode();
            var moderatorId = await _fixture.SendAsync(new CreateModerator.Command($"{random}@email.com", "password", "George", "Moderatorakis"));
            _fixture.SwitchUser(moderatorId);

            var workspace = await _fixture.ExecuteDbContextAsync(db => db.WsWorkspaces.FirstOrDefaultAsync(x => x.ImportKey == filename));
            if (workspace != null)
            {
                await _fixture.SendAsync(new EditWorkspace.DeleteCommand() { WorkspaceId = workspace.Id.Hash() });
            }

            // import file.
            var file = GetFile(filename);
            workspace = await _fixture.SendAsync(new ImportWorkspace.Command(file));

            Assert.NotNull(workspace);
            Assert.Equal(Domain.WorkspaceStatus.Finalized, workspace.CurrentStatus);
        }

        private IFormFile GetFile(string filename)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Data", filename);
            var byteArray = File.ReadAllBytes(path);
            var stream = new MemoryStream(byteArray);
            var name = Path.GetFileNameWithoutExtension(filename);
            return new FormFile(stream, 0, byteArray.Length, name, filename);
        }
    }
}
