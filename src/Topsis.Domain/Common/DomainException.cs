using System;

namespace Topsis.Domain.Common
{
    public enum DomainErrors
    { 
        Unknown = 0,
        Workspace_InvalidStatusChange = 1,
        WorkspaceStatus_CannotAddCriterion = 2,
        WorkspaceStatus_CannotAddAlternative = 3,
        Workspace_InvalidWeight = 4,
        WorkspaceStatus_CannotCalculateResults = 5,
        WorkspaceStatus_CannotFindCriterion = 6,
    }

    public class DomainException : Exception
    {
        public DomainException(DomainErrors error, string message = null) : base(message ?? error.ToString())
        {
            Error = error;
        }

        public DomainErrors Error { get; }
    }
}
