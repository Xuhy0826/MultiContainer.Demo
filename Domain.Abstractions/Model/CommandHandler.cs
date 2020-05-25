using System.Threading;
using System.Threading.Tasks;
using Mark.Common;
using MediatR;

namespace Domain.Core.Model
{
    public abstract class CommandHandler<TCommand, TDto> : IRequestHandler<TCommand, CommandResponse<TDto>>
        where TDto : IDataTransferObject
        where TCommand : IRequest<CommandResponse<TDto>>
    {
        public abstract Task<CommandResponse<TDto>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
