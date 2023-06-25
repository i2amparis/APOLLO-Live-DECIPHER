using System;

namespace Topsis.Domain
{
    [Flags]
    public enum WorkspaceLoginFields
    { 
        None = 0,
        JobCategory = 1,
        Country = 2,
        Gender = 4,
    }

    public class WorkspaceSettings
    {
        public WorkspaceSettings()
        {
            VoteFormTitle = null;
            LoginFormFields = WorkspaceLoginFields.JobCategory | WorkspaceLoginFields.Country;
        }

        public string VoteFormTitle { get; set; }
        public WorkspaceLoginFields LoginFormFields { get; set; }
    }
}