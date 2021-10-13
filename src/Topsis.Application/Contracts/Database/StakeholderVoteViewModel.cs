﻿using System;
using System.Collections.Generic;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class StakeholderVoteViewModel
    { 
        public IList<KeyValuePair<int, string>> AlternativesOrdered { get; set; }
        public IList<Criterion> CriteriaOrdered { get; set; }
        public List<NameValueOption> AlternativeRange { get; set; }
        public string WorkspaceTitle { get; set; }
        public string WorkspaceId { get; set; }
        public List<NameValueOption> CriteriaImportanceRange { get; set; }
        public List<StakeholderAnswerDto> StakeholderAnswers { get; private set; }
        public WorkspaceStatus WorkspaceStatus { get; set; }

        public void AddStakeholderAnswers(List<StakeholderAnswerDto> answers)
        {
            StakeholderAnswers = answers;
        }

    }
}
