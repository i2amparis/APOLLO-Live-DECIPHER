using System.Collections.Generic;

namespace Topsis.Domain
{
    public class TopsisInput
    {
        public TopsisInput()
        {
            Questionnaire = new Questionnaire();
            Votes = new List<StakeholderVote>();
        }

        public Questionnaire Questionnaire { get; set; }
        public List<StakeholderVote> Votes { get; set; }
    }
}
