using Topsis.Domain;
using Topsis.Domain.Specifications;
using Xunit;

namespace Topsis.AcceptanceTests.Units
{
    public class WorkspaceStatusChangeTests
    {
        [Theory]
        [InlineData(WorkspaceStatus.Draft, WorkspaceStatus.Draft)]
        [InlineData(WorkspaceStatus.Draft, WorkspaceStatus.Published)]
        [InlineData(WorkspaceStatus.Published, WorkspaceStatus.Published)]
        [InlineData(WorkspaceStatus.Published, WorkspaceStatus.AcceptingVotes)]
        [InlineData(WorkspaceStatus.AcceptingVotes, WorkspaceStatus.AcceptingVotes)]
        [InlineData(WorkspaceStatus.AcceptingVotes, WorkspaceStatus.Finalized)]
        public void Can_Change(WorkspaceStatus current, WorkspaceStatus newStatus)
        { 
            var sut = new WorkspaceStatusChangeSpec(current);

            Assert.True(sut.IsSatisfiedBy(newStatus));
        }

        [Theory]
        [InlineData(WorkspaceStatus.Draft, WorkspaceStatus.AcceptingVotes)]
        [InlineData(WorkspaceStatus.Published, WorkspaceStatus.Finalized)]
        public void Cannot_Change(WorkspaceStatus current, WorkspaceStatus newStatus)
        {
            var sut = new WorkspaceStatusChangeSpec(current);

            Assert.False(sut.IsSatisfiedBy(newStatus));
        }
    }
}
