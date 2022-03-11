using Topsis.Domain.Common;

namespace Topsis.Adapters.Import
{
    public class ImportException : DomainException
    {
        public ImportException(DomainErrors error, string message = null) : base(error, message)
        {
        }
    }
}
