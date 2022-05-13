using System;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class FeedbackTip
    {
        public FeedbackTip(StakeholderTopsis stakeholderTopsis, double groupTopsis, string alternativeTitle)
        {
            AlternativeId = stakeholderTopsis.AlternativeId;
            Mytopsis = stakeholderTopsis.Topsis;
            GroupTopsis = groupTopsis;
            AlternativeTitle = alternativeTitle;
        }

        public int AlternativeId { get; }
        public double Mytopsis { get; }
        public double GroupTopsis { get; }
        public string AlternativeTitle { get; }

        public double Distance => Math.Abs(GroupTopsis - Mytopsis);
        public bool IsMyTopsisAboveGroup => Mytopsis > GroupTopsis;
    }
}
