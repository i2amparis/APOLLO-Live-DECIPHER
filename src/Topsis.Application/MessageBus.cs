using MediatR;
using System.Threading.Tasks;

namespace Topsis.Application
{
    public interface IMessageBus
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> msg);
    }

    public class MessageBus : IMessageBus
    {
        private readonly IMediator _mediator;

        public MessageBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> msg)
        {
            return _mediator.Send(msg);
        }
    }
}
