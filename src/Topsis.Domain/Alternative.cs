using System;
using System.Collections.Generic;

namespace Topsis.Domain
{

    public class Alternative : EntityWithTitle, Contracts.ICanBeOrdered
    {
        public short Order { get; set; }

        internal static IEnumerable<Alternative> Bulk(int count)
        {
            for (short order = 1; order <= count; order++)
            {
                yield return new Alternative
                {
                    Title = $"Alternative {order}",
                    Order = order
                };
            }
        }

        internal bool IsReadyForVoting()
        {
            return string.IsNullOrWhiteSpace(Title) == false;
        }

        public override string ToString()
        {
            return $"{Id}, {Title}, {Order}";
        }
    }
}
