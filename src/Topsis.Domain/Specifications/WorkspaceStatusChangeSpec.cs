namespace Topsis.Domain.Specifications
{
    public class WorkspaceStatusChangeSpec
    {
        private readonly Workspace _workspace;
        private WorkspaceStatus _currentStatus;

        public WorkspaceStatusChangeSpec(Workspace workspace) : this(workspace.CurrentStatus)
        {
            _workspace = workspace;
        }

        public WorkspaceStatusChangeSpec(WorkspaceStatus currentStatus)
        {
            _currentStatus = currentStatus;
        }

        public bool IsSatisfiedBy(WorkspaceStatus newStatus)
        {
            return _currentStatus switch
            {
                WorkspaceStatus.Draft => CheckFromDraft(newStatus),
                WorkspaceStatus.Published => CheckFromPublished(newStatus),
                WorkspaceStatus.FinalizedWithFeedback => CheckFromFinalized(newStatus),
                WorkspaceStatus.Finalized => CheckFromFinalized(newStatus),
                _ => true,
            };
        }

        private bool CheckFromDraft(WorkspaceStatus newStatus)
        {
            return newStatus switch
            {
                WorkspaceStatus.Draft => true,
                WorkspaceStatus.Published => _workspace == null || _workspace.Questionnaire?.IsReady() == true,
                _ => false,
            };
        }

        private bool CheckFromPublished(WorkspaceStatus newStatus)
        {
            switch (newStatus)
            {
                case WorkspaceStatus.Draft:
                case WorkspaceStatus.Published:
                case WorkspaceStatus.AcceptingVotes:
                    return true;
                default:
                    return false;
            }
        }

        private bool CheckFromFinalized(WorkspaceStatus newStatus)
        {
            return newStatus switch
            {
                WorkspaceStatus.AcceptingVotes => true,
                WorkspaceStatus.FinalizedWithFeedback => true,
                WorkspaceStatus.Finalized => true,
                WorkspaceStatus.Archived => true,
                _ => false,
            };
        }
    }
}
