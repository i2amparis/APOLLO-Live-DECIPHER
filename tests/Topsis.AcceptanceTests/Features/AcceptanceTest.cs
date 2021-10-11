using Topsis.AcceptanceTests.TestServer;
using Xunit;
using Xunit.Abstractions;

namespace Topsis.AcceptanceTests.Features
{
    //public class CreateWorkspaceTest : BaseTest
    //{
    //    public CreateWorkspaceTest(DefaultWebFactory factory, ITestOutputHelper output) 
    //        : base(factory, output, Scenario.Workspace_Create)
    //    {
    //    }

    //    [Fact]
    //    public async System.Threading.Tasks.Task Create_Workspace_TestAsync()
    //    {
    //        var id = await SystemInterface.CreateWorkspaceAsync("my new workspace " + EntityFactory.NewId());

    //        var ws = await SystemInterface.GetWorkspaceAsync(id);

    //    }
    //}

    [Collection(nameof(SliceFixture))]
    public class AcceptanceTest
    {
        protected readonly SliceFixture _fixture;
        public AcceptanceTest(SliceFixture sliceFixture, ITestOutputHelper output)
        {
            _fixture = new SliceFixture();
            //_fixture.Output = output;
        }
    }
}
