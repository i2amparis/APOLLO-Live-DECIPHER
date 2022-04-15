using System;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class FeedbackTip
    {
        public FeedbackTip(StakeholderTopsis stakeholderTopsis, double groupTopsis)
        {
            AlternativeId = stakeholderTopsis.AlternativeId;
            Mytopsis = stakeholderTopsis.Topsis;
            GroupTopsis = groupTopsis;
        }

        public int AlternativeId { get; }
        public double Mytopsis { get; }
        public double GroupTopsis { get; }

        public double Distance => Math.Abs(GroupTopsis - Mytopsis);
        public bool IsMyTopsisAboveGroup => Mytopsis > GroupTopsis;

        public string GetTitle()
        {
            // todo: correct labels.
            if (IsMyTopsisAboveGroup)
            {
                return $"Your vote is above group.";
            }

            return $"Your vote is below group";
        }
    }
}
