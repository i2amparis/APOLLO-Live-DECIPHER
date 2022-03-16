using System;
using System.Collections.Generic;
using System.Linq;

namespace Topsis.Domain
{
    public class QuestionnaireSettings
    {
        private const OutputLinguisticScale DefaultScale = OutputLinguisticScale.Scale5;
        private const int DefaultCriterionWeightMax = 4;
        
        private const int AlternativeRangeMin = 0;
        public const int CriterionWeightMin = 0;

        public QuestionnaireSettings()
        {
            Rigorousness = 1;

            AlternativeRange = new List<NameValueOption>();
            CriteriaWeightRange = new List<NameValueOption>();
        }

        public static QuestionnaireSettings Default()
        {
            var settings = new QuestionnaireSettings();
            settings.ChangeScale(DefaultScale);
            settings.ChangeCriteriaWeight(DefaultCriterionWeightMax);
            return settings;
        }

        public OutputLinguisticScale Scale => AlternativeRange.Any() ? (OutputLinguisticScale)AlternativeRange.Last().Value : OutputLinguisticScale.Unknown;

        public int CriterionWeightMax => CriteriaWeightRange.Any() ? (int)CriteriaWeightRange.Max(x => x.Value) : DefaultCriterionWeightMax;

        /// <summary>
        /// Takes values from 0 to 1.
        /// </summary>
        public double Rigorousness { get; set; }

        public List<NameValueOption> AlternativeRange { get; set; }
        public List<NameValueOption> CriteriaWeightRange { get; set; }

        public int[] GetCriteriaImportanceRange()
        {
            return Enumerable.Range(CriterionWeightMin, CriterionWeightMax - CriterionWeightMin + 1).ToArray();
        }

        #region [ OutputLinguisticScale ]
        internal void ChangeScale(OutputLinguisticScale scale)
        {
            var currentScale = Scale;
            if (currentScale == scale)
            {
                return;
            }

            var newScale = (short)scale;
            var lessOptions = (short)currentScale > newScale;

            ChangeOptions(AlternativeRange, newScale, lessOptions, AlternativeRangeMin);
        }

        private void ChangeOptions(List<NameValueOption> currentList, int newValue, bool lessOptions, int minValue)
        {
            if (lessOptions)
            {
                ScaleDown(currentList, newValue, minValue);
            }
            else
            {
                ScaleUp(currentList, newValue, minValue);
            }
        }

        private void ScaleUp(List<NameValueOption> currentList, int newValue, int minValue)
        {
            for (int i = minValue; i <= newValue; i++)
            {
                if (currentList.Any(x => x.Value == i) == false)
                {
                    currentList.Add(NameValueOption.From(i));
                }
            }
        }

        private void ScaleDown(List<NameValueOption> currentList, int newValue, int minValue)
        {
            var removed = currentList.Where(x => x.Value > newValue).ToArray();
            foreach (var item in removed)
            {
                currentList.Remove(item);
            }
        }

        internal void ChangeCriteriaWeight(int newMax)
        {
            var currentMax = CriterionWeightMax;
            if (currentMax == newMax)
            {
                return;
            }

            var lessOptions = currentMax > newMax;
            ChangeOptions(CriteriaWeightRange, newMax, lessOptions, CriterionWeightMin);
        }
        #endregion
    }

    public class NameValueOption
    {
        // https://en.wikipedia.org/wiki/Likert_scale

        public string Name { get; set; }
        public double Value { get; set; }

        public static NameValueOption From(double value, string name = null)
        {
            return new NameValueOption
            {
                Value = value,
                Name = name ?? value.ToString()
            };
        }
    }
}
