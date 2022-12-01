using System.Threading.Tasks;
using Topsis.AcceptanceTests.TestServer;
using Topsis.Application.Features;
using Topsis.Application.Features.SendEmail;
using Xunit;
using Xunit.Abstractions;

namespace Topsis.AcceptanceTests.Features
{
    public class EmailTest : AcceptanceTest
    {
        public EmailTest(SliceFixture sliceFixture, ITestOutputHelper output) : base(sliceFixture, output)
        {
        }

        [Fact]
        public async Task Create_Workspace_TestAsync()
        {
            // arrange
            await _fixture.SendAsync(new SendEmailCommand("a.soursos@gmail.com", "test", "test"));
        }
    }
}
