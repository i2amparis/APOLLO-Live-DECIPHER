using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Topsis.Domain.Common;

namespace Topsis.Domain
{
    public enum CriterionType
    { 
        Cost = 0,
        Benefit = 1
    }

    public class Criterion : Entity, Contracts.ICanBeOrdered
    {
        public Criterion()
        {
            Order = 0;
            Type = CriterionType.Benefit;
        }

        public string Title { get; set; }
        public CriterionType Type { get; set; }
        public short Order { get; set; }

        //private double? _positiveIdeal = null;
        //private double? _negativeIdeal = null;
        //public (double,double) GetBestAndWorstValue()
        //{
        //    if (_positiveIdeal.HasValue && _negativeIdeal.HasValue)
        //    {
        //        return (_positiveIdeal.Value, _negativeIdeal.Value);
        //    }

        //    var importance = GetImportance();
        //    if (Type == CriterionType.Benefit)
        //    {
        //        _negativeIdeal = importance.Min;
        //        _positiveIdeal = importance.Max;
        //    }
        //    else
        //    {
        //        _negativeIdeal = importance.Max;
        //        _positiveIdeal = importance.Min;
        //    }

        //    return (_positiveIdeal.Value, _negativeIdeal.Value);
        //}

        internal static IEnumerable<Criterion> Bulk(int count)
        {
            for (short order = 1; order <= count; order++)
            {
                var criterion = new Criterion
                {
                    Title = $"Criterion {order}",
                    Order = order
                };

                yield return criterion;
            }
        }

        internal bool IsReadyForVoting()
        {
            return string.IsNullOrWhiteSpace(Title) == false;
        }

        public double GetIdeal(bool isPositive, double[] values)
        {
            if (Type == CriterionType.Benefit)
            {
                return isPositive ? values.Max() : values.Min();
            }

            if (Type == CriterionType.Cost)
            {
                return isPositive ? values.Min() : values.Max();
            }

            throw new NotSupportedException();
        }
    }

    //public class CriterionImportanceRange
    //{
    //    public CriterionImportanceRange()
    //    {
    //    }

    //    public CriterionImportanceRange(int importanceMin = 1, int importanceMax = 4)
    //    {
    //        Options = new List<CriterionImportanceOption>();
    //        for (int i = importanceMin; i <= importanceMax; i++)
    //        {
    //            Options.Add(CriterionImportanceOption.From(i));
    //        }
    //    }

    //    public List<CriterionImportanceOption> Options { get; set; }

    //    public double Min => Options.Min(x => x.Value);
    //    public double Max => Options.Max(x => x.Value);

    //    public override string ToString()
    //    {
    //        return string.Join(",", Options.Select(x => x.ToString()));
    //    }
    //}

    public class CriterionImportanceOption
    {
        // https://en.wikipedia.org/wiki/Likert_scale

        public string Title { get; set; }
        public double Value { get; set; }

        public static CriterionImportanceOption From(double value, string title = null)
        {
            return new CriterionImportanceOption
            {
                Value = value,
                Title = title ?? (value).ToString()
            };
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
