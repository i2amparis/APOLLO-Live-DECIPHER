using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topsis.Domain.Contracts;

namespace Topsis.Domain.Common
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> items)
        {
            return items == null || items.Count < 1;
        }

        public static T FindOrDefault<T>(this ICollection<T> items, int id) where T : IEntity<int>
        {
            return items.FirstOrDefault(x => x.Id == id);
        }

        public static void ChangeOrder<T>(this ICollection<T> items, int id, bool moveUp) where T : ICanBeOrdered
        {
            var orderedItems = items.OrderBy(x => x.Order).ToArray();
            if (orderedItems.IsNullOrEmpty())
            {
                return;
            }

            var item = FindOrDefault(orderedItems, id);
            if (item == null)
            {
                return;
            }

            var newOrder = moveUp ? item.Order - 1 : item.Order + 1;
            var replacedItem = orderedItems.FirstOrDefault(x => x.Order == newOrder);
            if (replacedItem == null)
            {
                return;
            }

            replacedItem.Order = item.Order;
            item.Order = (short)newOrder;
        }

        public static void ResetOrder<T>(this ICollection<T> items) where T : ICanBeOrdered
        {
            short order = 1;
            foreach (var item in items.OrderBy(x => x.Order))
            {
                item.Order = order;
                order++;
            }
        }
    }
}
