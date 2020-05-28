using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.User.Domain.Model;
using Demo.User.Domain.Model.Dto;
using Demo.User.Infrastructure;
using Domain.Core.Model;
using Mark.Common.Services;
using MediatR;

namespace Demo.User.API.Application.Queries
{
    public class UserCollectionQueryRequest : UserQuery, IRequest<QueryResponse<UserDto>>
    {
        public UserCollectionQueryRequest()
        {
            OrderBy = nameof(UserDto.No);
        }
    }
    public class UserCollectionQueryRequestHandler : QueryHandler<UserCollectionQueryRequest, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserCollectionQueryRequestHandler(IUserRepository repository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService)
            : base(propertyMappingService, propertyCheckerService)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
        }

        public override async Task<QueryResponse<UserDto>> Handle(UserCollectionQueryRequest request,
            CancellationToken cancellationToken)
        {
            QueryResponse<UserDto> response = new QueryResponse<UserDto>();
            if (!_propertyMappingService.ValidMappingExistsFor<UserDto, Domain.Aggregate.User>(request.OrderBy))
            {
                response.Status = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Message = $"Ah oh, request.OrderBy is 【{request.OrderBy}】";
                return response;
            }
            if (!_propertyCheckerService.TypeHasProperties<UserDto>(request.Fields))
            {
                response.Status = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Message = $"Ah oh, request.Fields is 【{request.Fields}】";
                return response;
            }
            var users = await _repository.GetUserCollectionAsync(request, false);
            //AutoMapper进行属性映射
            response.RawData = _mapper.Map<IEnumerable<UserDto>>(users);
            //数据重塑
            response.ShapeData(request.Fields);
            return await Task.FromResult(response);
        }
    }
}
