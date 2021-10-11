namespace Topsis.Domain
{
    public enum OutputLinguisticScale : short
    {
        Unknown = 0,
        Scale3 = 2,
        Scale5 = 4,
        Scale7 = 6,
        Scale9 = 8
    }

    public class TopsisSettings
    {
        public TopsisSettings()
        {
            Scale = OutputLinguisticScale.Scale5;
            Rigorousness = 1;
        }

        public OutputLinguisticScale Scale { get; set; }

        /// <summary>
        /// Takes values from 0 to 1.
        /// </summary>
        public double Rigorousness { get; set; }
    }
}
