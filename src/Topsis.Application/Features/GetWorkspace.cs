using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Application.Features
{
    public static class GetWorkspace
    {
        public class ById
        {
            public class Request : IRequest<Response>
            {
                public Request(string id)
                {
                    Id = id;
                }

                public string Id { get; set; }
            }

            public class Response
            {
                public Response(Domain.Workspace result)
                {
                    Result = result;
                }

                public Domain.Workspace Result { get; }
            }

            public class Handler : IRequestHandler<Request, Response>
            {
                private readonly IWorkspaceRepository _workspaces;

                public Handler(IWorkspaceRepository workspaces)
                {
                    _workspaces = workspaces;
                }

                public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                {
                    var id = request.Id.DehashInts().First();
                    var result = await _workspaces.GetByIdAsync(id);
                    return new Response(result);
                }
            }
        }

        public class ByPage
        {

            public class Request : IRequest<Response>
            {
                public Request(int page)
                {
                    Page = page;
                }

                public int Page { get; set; }
            }

            public class Response
            {
                public Response(IPaginatedList<Workspace> result)
                {
                    Result = result;
                }

                public IPaginatedList<Workspace> Result { get; }
            }

            public class Handler : IRequestHandler<Request, Response>
            {
                private readonly IWorkspaceRepository _workspaces;

                public Handler(IWorkspaceRepository workspaces)
                {
                    _workspaces = workspaces;
                }

                public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                {
                    var result = await _workspaces.GetPagedReponseAsync(request.Page, 10);

                    return new Response(result);
                }
            }
        }
    }
    
}
