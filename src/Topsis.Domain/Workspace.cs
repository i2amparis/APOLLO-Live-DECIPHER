using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Topsis.Application.Contracts.Database;
using Topsis.Domain.Common;
using Topsis.Domain.Specifications;

namespace Topsis.Domain
{
    public enum WorkspaceStatus : short
    { 
        /// <summary>
        /// Editing add/remove criteria and alternatives (admin/moderator).
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Workspace is published, guests can view it but cannot vote yet.
        /// </summary>
        Published = 1,

        /// <summary>
        /// Now guests can vote.
        /// </summary>
        AcceptingVotes = 2,

        /// <summary>
        /// Stop accepting votes. Accept feedback. Shows results in report page.
        /// </summary>
        FinalizedWithFeedback = 3,

        /// <summary>
        /// Stop accepting votes. Shows results in report page.
        /// </summary>
        Finalized = 4,

        /// <summary>
        /// Stop showing (home/report pages), only admin/moderators can see it. 
        /// </summary>
        Archived = 5
    }

    public class Workspace : Entity
    {
        public Workspace()
        {
            CurrentStatus = WorkspaceStatus.Draft;
            Questionnaire = new Questionnaire();
            Votes = new List<StakeholderVote>();
            Reports = new List<WorkspaceReport>();
        }

        public bool IsFinalized()
        {
            return CurrentStatus == WorkspaceStatus.Finalized || CurrentStatus == WorkspaceStatus.FinalizedWithFeedback;
        }


        public bool CanProvideTips()
        {
            return CurrentStatus == WorkspaceStatus.AcceptingVotes
                || CurrentStatus == WorkspaceStatus.FinalizedWithFeedback;
        }

        public int? ParentId { get; set; }
        public Workspace Parent { get; set; }

        [StringLength(255)]
        public string ImportKey { get; set; }
        public Questionnaire Questionnaire { get; set; }
        public List<StakeholderVote> Votes { get; set; }
        public List<WorkspaceReport> Reports { get; set; }

        [StringLength(255)]
        public string UserId { get; set; }

        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }

        public WorkspaceStatus CurrentStatus { get; set; }

        public void MoveCriterion(int criterionId, bool moveUp)
        {
            Questionnaire?.Criteria.ChangeOrder(criterionId, moveUp);
        }

        public void MoveAlternative(int alternativeId, bool moveUp)
        {
            Questionnaire?.Alternatives?.ChangeOrder(alternativeId, moveUp);
        }

        public void ChangeStatus(WorkspaceStatus newStatus)
        {
            var spec = new WorkspaceStatusChangeSpec(this);
            if (spec.IsSatisfiedBy(newStatus))
            {
                CurrentStatus = newStatus;
                return;
            }

            throw new DomainException(DomainErrors.Workspace_InvalidStatusChange, $"{CurrentStatus}->{newStatus}");
        }

        public void ChangeInfo(string title, string description)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Criterion AddCriterion(string title)
        {
            EnsureInDraftStatusOrThrowException();
            return Questionnaire.AddCriterion(title);
        }

        public Alternative AddAlternative(string title)
        {
            EnsureInDraftStatusOrThrowException();
            return Questionnaire.AddAlternative(title);
        }

        public void RemoveCriterion(int criterionId)
        {
            EnsureInDraftStatusOrThrowException();
            Questionnaire.RemoveCriterion(criterionId);
        }

        public void RemoveAlternative(int alternativeId)
        {
            EnsureInDraftStatusOrThrowException();
            Questionnaire.RemoveAlternative(alternativeId);
        }

        public void ChangeCriterion(int id, string title, CriterionType type)
        {
            var item = Questionnaire.Criteria.FindOrDefault(id);
            if (item == null || string.IsNullOrEmpty(title))
            {
                return;
            }

            item.Title = title;
            item.Type = type;
        }

        public void ChangeAlternative(int id, string title)
        {
            var item = Questionnaire.Alternatives.FindOrDefault(id);
            if (item == null || string.IsNullOrEmpty(title))
            {
                return;
            }

            item.Title = title;
        }

        public void ChangeAlternativeRange(List<NameValueOption> range)
        {
            EnsureInDraftStatusOrThrowException();

            Questionnaire.ChangeAlternativeRange(range);
        }

        public void ChangeCriteriaWeightsRange(List<NameValueOption> range)
        {
            EnsureInDraftStatusOrThrowException();

            Questionnaire.ChangeCriteriaWeightsRange(range);
        }

        public void ChangeQuestionnaireSettings(OutputLinguisticScale scale, 
            double rigorousness, 
            int criterionWeightMax)
        {
            EnsureInDraftStatusOrThrowException();

            Questionnaire.ChangeSettings(scale, rigorousness, criterionWeightMax);
        }

        #region [ Reports ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns>The created or updated report.</returns>
        public WorkspaceReport CreateOrUpdateReport(FeedbackRound round)
        {
            var report = Reports.SingleOrDefault(x => x.Round == round);
            if (report != null)
            {
                report.InitializeFrom(this);
                return report;
            }

            // create new report.
            var currentRound = GetCurrentRound();
            if ((short)round - (short)currentRound > 1)
            {
                throw new DomainException(DomainErrors.Workspace_InvalidReportRound, $"{round}<{currentRound}");
            }

            report = WorkspaceReport.Create(this, round);
            Reports.Add(report);
            return report;
        }

        public void FinalizeReport(int reportId, WorkspaceAnalysisResult result)
        {
            var report = Reports.SingleOrDefault(x => x.Id == reportId);
            report.SetResult(result);
        }

        public WorkspaceAnalysisResult GetReportData()
        {
            return Reports?.LastOrDefault()?.GetAnalysisResult();
        }

        public bool HasReport()
        {
            return IsFinalized() && Reports?.Any() == true;
        }

        public FeedbackRound GetCurrentRound()
        {
            var result = Reports?.LastOrDefault()?.Round;
            if (result == null || result == FeedbackRound.Undefined)
            {
                return FeedbackRound.First;
            }

            return (FeedbackRound)result;
        }
        #endregion

        #region [ Helpers ]
        private void EnsureInDraftStatusOrThrowException()
        {
            if (CurrentStatus != WorkspaceStatus.Draft)
            {
                throw new DomainException(DomainErrors.WorkspaceStatus_CannotAddAlternative);
            }
        }

        public void AddCriterionOption()
        {
            EnsureInDraftStatusOrThrowException();
            Questionnaire.AddCriterionOption();
        }

        public void DeleteCriterionOption(int index)
        {
            EnsureInDraftStatusOrThrowException();
            Questionnaire.DeleteCriterionOption(index);
        }

        public void ChangeCriterion(int criterionId, string title, CriterionType criterionType, object importanceMin, object importanceMax)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
