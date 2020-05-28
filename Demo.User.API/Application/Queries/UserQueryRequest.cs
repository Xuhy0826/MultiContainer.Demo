using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.User.Domain.Model;
using Demo.User.Domain.Model.Dto;
using Demo.User.Infrastructure;
using Domain.Core.Model;
using Mark.Common.ExtensionMethod;
using Mark.Common.Services;
using MediatR;

namespace Demo.User.API.Application.Queries
{
    public class UserQueryRequest : UserQuery, IRequest<QueryResponse<UserDto>>
    {
    }

    public class UserQueryRequestHandler : QueryHandler<UserQueryRequest, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserQueryRequestHandler(IUserRepository repository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService) : base(propertyMappingService, propertyCheckerService)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override async Task<QueryResponse<UserDto>> Handle(UserQueryRequest request, CancellationToken cancellationToken)
        {
            QueryResponse<UserDto> response = new QueryResponse<UserDto>();
            if (!_propertyCheckerService.TypeHasProperties<UserDto>(request.Fields))
            {
                response.Status = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Message = $"Ah oh, request.Fields is 【{request.Fields}】";
                return response;
            }
            if (request.Id == null)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.Message = $"Ah oh, Id is a empty value";
                return response;
            }
            var user = await _repository.GetAsync(request.Id.Value, cancellationToken);
            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.Message = $"Ah oh, User of Id:{request.Id} is not exist";
                return response;
            }
            var dto = _mapper.Map<UserDto>(user);

            //数据重塑
            var shapedData = dto.ShapeData(request.Fields);
            response.RawData = new List<UserDto> { dto };
            response.ShapedData = new List<ExpandoObject> { shapedData };
            return response;
        }
    }
}
