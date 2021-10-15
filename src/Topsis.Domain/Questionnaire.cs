using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Topsis.Domain.Common;

namespace Topsis.Domain
{

    /// <summary>
    /// this actually a matrix (criterion x alternative).
    /// </summary>
    public class Questionnaire : Entity
    {
        public Questionnaire()
        {
            DefaultCriterionWeight = 1;
            Criteria = new List<Criterion>();
            Alternatives = new List<Alternative>();
        }

        public double DefaultCriterionWeight { get; set; }

        #region [ Settings: saved as json string in db. ]
        // dont change the name of it, in use from QuestionnaireConfiguration.
        private string _settings;
        public QuestionnaireSettings GetSettings()
        {
            return string.IsNullOrEmpty(_settings)
                ? new QuestionnaireSettings()
                : Serializer.DeserializeFromJson<QuestionnaireSettings>(_settings);
        }

        public void SetSettings(QuestionnaireSettings value)
        {
            _settings = Serializer.SerializeToJson(value);
        }
        #endregion

        public List<Criterion> Criteria { get; set; }
        public List<Alternative> Alternatives { get; set; }
        public IDictionary<int, Alternative> AlternativesDictionary => Alternatives?.ToDictionary(x => x.Id, x => x);

        public bool IsReady()
        {
            return Criteria?.Count > 0
                && Alternatives?.Count > 0
                && Criteria.All(x => x.IsReadyForVoting())
                && Alternatives.All(x => x.IsReadyForVoting());
        }

        internal void AddCriterion(string title)
        {
            var newOrder = Criteria.Any() ? Criteria.Max(x => x.Order) + 1 : 1;
            Criteria.Add(new Criterion { Title = title, Order = (short)newOrder });
        }

        internal void AddAlternative(string title)
        {
            var newOrder = Alternatives.Any() ? Alternatives.Max(x => x.Order) + 1 : 1;
            Alternatives.Add(new Alternative { Title = title, Order = (short)newOrder });
        }

        internal void RemoveCriterion(int criterionId)
        {
            var item = Criteria.FindOrDefault(criterionId);
            if (item == null)
            {
                return;
            }

            Criteria.Remove(item);
            Criteria.ResetOrder();
        }

        internal void RemoveAlternative(int alternativeId)
        {
            var item = Alternatives.FindOrDefault(alternativeId);
            if (item == null)
            {
                return;
            }

            Alternatives.Remove(item);
            Alternatives.ResetOrder();
        }

        //internal void ChangeCriterionWeight(double weight)
        //{
        //    DefaultCriterionWeight = weight;
        //    foreach (var item in Criteria)
        //    {
        //        item.Importance = weight;
        //    }
        //}

        internal void ChangeAlternativeRange(List<NameValueOption> range)
        {
            var settings = GetSettings();
            settings.AlternativeRange = range;
            SetSettings(settings);
        }

        internal void ChangeCriteriaWeightsRange(List<NameValueOption> range)
        {
            var settings = GetSettings();
            settings.CriteriaWeightRange = range;
            SetSettings(settings);
        }

        internal void AddCriterionOption()
        {
            var settings = GetSettings();
            settings.AlternativeRange.Add(NameValueOption.From(settings.AlternativeRange.Count));
            SetSettings(settings);
        }

        internal void DeleteCriterionOption(int index)
        {
            var settings = GetSettings();
            if (index >= settings.AlternativeRange.Count)
            {
                return;
            }

            settings.AlternativeRange.RemoveAt(index);
            SetSettings(settings);
        }

        internal void ChangeSettings(OutputLinguisticScale scale, double rigorousness, int criterionWeightMax)
        {
            var settings = GetSettings();
            settings.ChangeScale(scale);
            settings.ChangeCriteriaWeight(criterionWeightMax);
            settings.Rigorousness = rigorousness;
            SetSettings(settings);
        }
    }
}
