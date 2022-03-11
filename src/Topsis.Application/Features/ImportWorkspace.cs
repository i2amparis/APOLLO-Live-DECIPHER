using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Import;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Features
{
    public static class ImportWorkspace
    {
        public class Command : IRequest<Workspace>
        {
            public Command(IFormFile file)
            {
                File = file;
            }

            public IFormFile File { get; }
        }


        public class Handler : IRequestHandler<Command, Workspace>
        {
            private readonly IImportService _import;

            public Handler(IImportService import)
            {
                _import = import;
            }

            public async Task<Workspace> Handle(Command cmd, CancellationToken cancellationToken)
            {
                return await _import.ImportAsync(cmd.File);
            }

        }
    }
}
