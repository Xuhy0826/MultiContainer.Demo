using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.User.Domain.Model;
using Demo.User.Domain.Model.Dto;
using Demo.User.Infrastructure;
using Demo.User.Infrastructure.ViewModel;
using Domain.Core.Model;
using Mark.Common.Services;
using MediatR;

namespace Demo.User.API.Application.Queries
{
    public class UserPagedCollectionQueryRequest : UserQuery, IRequest<QueryResponse<UserDto>>
    {
        public UserPagedCollectionQueryRequest()
        {
            OrderBy = nameof(UserDto.No);
        }
    }

    public class UserPagedCollectionQueryRequestHandler : QueryHandler<UserPagedCollectionQueryRequest, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserPagedCollectionQueryRequestHandler(IUserRepository repository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService)
            : base(propertyMappingService, propertyCheckerService)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
        }

        public override async Task<QueryResponse<UserDto>> Handle(UserPagedCollectionQueryRequest request,
            CancellationToken cancellationToken)
        {
            var response = new QueryResponse<UserDto>();
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
            var returnVisits = await _repository.GetUserCollectionAsync(request, true) as PagedList<Domain.Aggregate.User>;
            request.HasNextPage = returnVisits.HasNextPage;
            request.HasPreviousPage = returnVisits.HasPreviousPage;
            var paginationMetadata = new
            {
                totalCount = returnVisits.TotalCount,
                pageSize = returnVisits.PageSize,
                currentPage = returnVisits.CurrentPage,
                totalPages = returnVisits.TotalPages
            };
            //Hateaos：将分页信息一起返回
            response.MoreInfo = JsonSerializer.Serialize(paginationMetadata,
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            //AutoFactory进行属性映射
            response.RawData = _mapper.Map<IEnumerable<UserDto>>(returnVisits);
            //数据重塑
            response.ShapeData(request.Fields);
            return await Task.FromResult(response);
        }
    }
}
