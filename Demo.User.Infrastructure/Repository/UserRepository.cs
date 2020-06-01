using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.User.Domain.Model;
using Demo.User.Domain.Model.Dto;
using Demo.User.Infrastructure.ViewModel;
using Infrastructure.Core;
using Mark.Common.ExtensionMethod;
using Mark.Common.Services;
using Microsoft.EntityFrameworkCore;


namespace Demo.User.Infrastructure
{
    public class UserRepository : Repository<Domain.Aggregate.User, long, DomainContext>, IUserRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;
        public UserRepository(DomainContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException();
        }

        public async Task<bool> Exist(long id)
        {
            return await DbContext.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exist(UserQuery parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            var queryExpression = DbContext.Users as IQueryable<Domain.Aggregate.User>;
            SetFilter(parameters, queryExpression);
            return await queryExpression.CountAsync() > 0;
        }

        public async Task<IList<Domain.Aggregate.User>> GetUserCollectionAsync(UserQuery parameters, bool byPage = false)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            var queryExpression = DbContext.Users as IQueryable<Domain.Aggregate.User>;
            SetFilter(parameters, queryExpression);
            var mappingDictionary = _propertyMappingService.GetPropertyMapping<UserDto, Domain.Aggregate.User>();
            queryExpression = queryExpression.ApplySort(parameters.OrderBy, mappingDictionary);
            if (byPage)
            {
                return await PagedList<Domain.Aggregate.User>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
            }
            return await queryExpression.ToListAsync();
        }

        private void SetFilter(UserQuery parameters, IQueryable<Domain.Aggregate.User> queryExpression)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                parameters.Name = parameters.Name.Trim();
                queryExpression = queryExpression.Where(x => x.Name.Contains(parameters.Name));
            }
            if (!string.IsNullOrWhiteSpace(parameters.No))
            {
                parameters.No = parameters.No.Trim();
                queryExpression = queryExpression.Where(x => x.No.Contains(parameters.No));
            }
            if (!string.IsNullOrWhiteSpace(parameters.TelNumber))
            {
                parameters.TelNumber = parameters.TelNumber.Trim();
                queryExpression = queryExpression.Where(x => x.No.Contains(parameters.TelNumber));
            }
            if (parameters.Level != null)
                queryExpression = queryExpression.Where(x => x.Level == parameters.Level);
            if (parameters.Gender != null)
                queryExpression = queryExpression.Where(x => x.Gender == parameters.Gender);
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x =>
                    x.Name.Contains(parameters.SearchTerm) ||
                    x.No.Contains(parameters.SearchTerm) ||
                    x.TelNumber.Contains(parameters.SearchTerm));
            }
        }
    }
}
