using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Features;
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

        [Fact]
        public async Task TestAsync()
        {
            var random = Guid.NewGuid().GetHashCode();
            var moderatorId = await _fixture.SendAsync(new CreateModerator.Command($"{random}@email.com", "password", "George", "Moderatorakis"));
            _fixture.SwitchUser(moderatorId);

            // import file.
            var file = GetFile();
            var workspace = await _fixture.SendAsync(new ImportWorkspace.Command(file));

            var result = await _fixture.ExecuteDbContextAsync(
                db => db.WsWorkspacesReports.FirstOrDefaultAsync(x => x.Id == workspace.Id));

            Assert.NotNull(result);
        }

        private IFormFile GetFile()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Data/input_mcda_dlci.xlsx");
            var byteArray = File.ReadAllBytes(path);
            var stream = new MemoryStream(byteArray);
            return new FormFile(stream, 0, byteArray.Length, "input_mcda_dlci", "input_mcda_dlci.xlsx");
        }
    }
}
