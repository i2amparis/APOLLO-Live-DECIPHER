using System;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Algorithm
{
    public class StakeholderAnswerDto
    {
        public int WorkspaceId { get; set; }
        public string StakeholderId { get; set; }
        public int AlternativeId { get; set; }
        public int CriterionId { get; set; }
        public int CriterionWeight { get; set; }
        
        public double AnswerValue { get; set; }
        public int VoteId { get; set; }
        public int? JobCategoryId { get; set; }
    }

    public class StakeholderDemographicsDto
    {
        public StakeholderDemographicsDto(string id, Gender? genderId, int? jobCategoryId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            GenderId = genderId;
            JobCategoryId = jobCategoryId;
        }

        public string Id { get; set; }
        public Gender? GenderId { get; set; }
        public int? JobCategoryId { get; set; }
    }
}