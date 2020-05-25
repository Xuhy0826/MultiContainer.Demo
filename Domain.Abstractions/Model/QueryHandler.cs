using System;
using System.Threading;
using System.Threading.Tasks;
using Mark.Common;
using Mark.Common.Services;
using MediatR;

namespace Domain.Core.Model
{
    public abstract class QueryHandler<TQueryModel, TDto> : IRequestHandler<TQueryModel, QueryResponse<TDto>>
    where TDto : IDataTransferObject
    where TQueryModel : IRequest<QueryResponse<TDto>>
    {
        protected readonly IPropertyMappingService _propertyMappingService;
        protected readonly IPropertyCheckerService _propertyCheckerService;

        protected QueryHandler(
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException();
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException();
        }

        public abstract Task<QueryResponse<TDto>> Handle(TQueryModel request, CancellationToken cancellationToken);
    }
}
