using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Import
{
    public interface IImportService
    {
        Task<Workspace> ImportAsync(IFormFile file);
    }
}
