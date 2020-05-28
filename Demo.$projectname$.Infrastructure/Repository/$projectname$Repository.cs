using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Mark.Common.Services;
using Demo.$projectname$.Domain.Aggregate;
using Demo.$projectname$.Domain.Model;
using Demo.$projectname$.Infrastructure.Container;

namespace Demo.$projectname$.Infrastructure
{
    public class $projectname$Repository : Repository<$projectname$, long, DomainContext>, I$projectname$Repository
    {
        private readonly IPropertyMappingService _propertyMappingService;
        public $projectname$Repository(DomainContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException();
        }

        public async Task<bool> Exist(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exist($projectname$Query parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<$projectname$>> Get$projectname$CollectionAsync($projectname$Query parameters, bool byPage = false)
        {
            throw new NotImplementedException();
        }
    }
}
