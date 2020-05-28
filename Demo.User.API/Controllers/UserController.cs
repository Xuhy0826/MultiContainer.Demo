using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Demo.User.API.Application.Queries;
using Demo.User.Domain.Model.Dto;
using Domain.Core.Helper;
using Domain.Core.Model;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Demo.User.API.Controllers
{
    /// <summary>
    /// “用户”管理功能
    /// </summary>
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;

        public UserController(IMediator mediator, IConfiguration config)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _config = config ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 分页查询所有用户信息数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accept">请求的媒体类型，可选值“application/json”或“application/vnd.demo.hateoas+json”</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpGet(Name = nameof(PagedUsers), Order = 0)]
        public async Task<IActionResult> PagedUsers([FromQuery]UserPagedCollectionQueryRequest request, [FromHeader(Name = "Accept")] string accept)
        {
            var includeLinks = LinkerFactory.IncludeLinks(accept);
            if (int.TryParse(_config["MaxPageSize"], out var maxPageSize))
                request.MaxPageSize = maxPageSize;
            //查询业务员，进行查询得到结果
            var queryResponse = await _mediator.Send(request);
            //查询出错
            if (queryResponse.Status != HttpStatusCode.OK)
            {
                return Problem(queryResponse.Message, nameof(PagedUsers),
                    (int)queryResponse.Status,
                    "Request occur error");
            }
            //写Header
            Response.Headers.Add("X-Pagination", queryResponse.MoreInfo);

            //Hateaos：创建自驱动链接
            if (includeLinks)
            {
                var linkerFactory = new LinkerFactory(Url);
                var links = linkerFactory.CreateLinksForCollections(request, nameof(PagedUsers), request.HasPreviousPage, request.HasNextPage);
                var shapedDatasWithLinks = queryResponse.ShapedData.Select(items =>
                {
                    var dict = items as IDictionary<string, object>;
                    var itemLinks = linkerFactory.CreateLinksForItem((long)dict["Id"], null, GetCurrentControllerItemLinker());
                    dict.Add("links", itemLinks);
                    return dict;
                });
                var createLink = linkerFactory.CreatePostItemLink(nameof(CreateUser));
                var linkedCollectionResource = new
                {
                    value = shapedDatasWithLinks,
                    page_navigator = links,
                    create_new_entity = createLink
                };
                return Ok(linkedCollectionResource);
            }
            return Ok(queryResponse.ShapedData);
        }
        /// <summary>
        /// 根据id查询单个实体的数据
        /// </summary>
        /// <param name="id">实体id</param>
        /// <param name="fields">请求的字段</param>
        /// <param name="mediaType">请求的媒体类型，可选“application/json”或者“application/vnd.demo.hateoas+json”，使用后者返回数据中包含自驱动的链接</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser([FromRoute]long? id, [FromQuery]string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            var request = new UserQueryRequest()
            {
                Id = id,
                Fields = fields
            };
            var includeLinks = LinkerFactory.IncludeLinks(mediaType);
            //查询业务员
            var queryResponse = await _mediator.Send(request);
            if (queryResponse.Status != HttpStatusCode.OK)
            {
                return Problem(queryResponse.Message, nameof(GetUser), (int)queryResponse.Status, "Request occur error");
            }
            IEnumerable<RelativeLink> relativeLinks = new List<RelativeLink>();
            var shapedItemsWithLinks = queryResponse.ShapedData.FirstOrDefault() as IDictionary<string, object>;
            if (shapedItemsWithLinks == null) return Ok("empty result");
            if (includeLinks)
            {
                var linkerFactory = new LinkerFactory(Url);
                relativeLinks = linkerFactory.CreateLinksForItem(request.Id.Value, fields, GetCurrentControllerItemLinker());
                shapedItemsWithLinks.Add("links", relativeLinks);
            }
            return Ok(shapedItemsWithLinks);
        }

        /// <summary>
        /// 查询“用户”数据的集合
        /// </summary>
        /// <param name="request">请求参数集合</param>
        /// <param name="mediaType">请求的媒体类型，可选“application/json”或者“application/vnd.demo.hateoas+json”，使用后者返回数据中包含自驱动的链接</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpGet(template: "all", Name = nameof(GetUsers), Order = 1)]
        public async Task<IActionResult> GetUsers([FromQuery]UserCollectionQueryRequest request, [FromHeader(Name = "Accept")] string mediaType)
        {
            var includeLinks = LinkerFactory.IncludeLinks(mediaType);
            //查询业务员
            var queryResponse = await _mediator.Send(request);
            if (queryResponse.Status != HttpStatusCode.OK)
            {
                return Problem(queryResponse.Message, nameof(GetUsers), (int)queryResponse.Status, "Request occur error");
            }
            //Hateaos：创建自驱动链接
            IEnumerable<IDictionary<string, object>> shapedItemsWithLinks = null;
            if (includeLinks)   //如果返回参数需要包含links
            {
                var linkerFactory = new LinkerFactory(Url);
                shapedItemsWithLinks = queryResponse.ShapedData.Select(item =>
                {
                    var dict = item as IDictionary<string, object>;
                    var itemLinks = linkerFactory.CreateLinksForItem((long)dict["Id"], null, GetCurrentControllerItemLinker());
                    dict.Add("links", itemLinks);
                    return dict;
                });
                var createLink = linkerFactory.CreatePostItemLink(nameof(CreateUser));
                var linkedCollectionResource = new
                {
                    value = shapedItemsWithLinks,
                    create_new_entity = createLink
                };
                return Ok(linkedCollectionResource);
            }
            shapedItemsWithLinks = queryResponse.ShapedData.Select(opr =>
            {
                var oprDict = opr as IDictionary<string, object>;
                return oprDict;
            });
            return Ok(shapedItemsWithLinks);
        }
        /// <summary>
        /// 创建一条“用户”信息
        /// </summary>
        /// <param name="returnVisit"></param>
        /// <param name="mediaType">请求的媒体类型，可选“application/json”或者“application/vnd.demo.hateoas+json”，使用后者返回数据中包含自驱动的链接</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpPost(Name = nameof(CreateUser))]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody]UserAddDto returnVisit, [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改“用户”信息，Put方法
        /// </summary>
        /// <param name="id">“用户”信息的id，最好存在在请求的QueryString中</param>
        /// <param name="user">修改的参数，最好存在请求的Body中”</param>
        /// <param name="mediaType">请求的媒体类型，可选“application/json”或者“application/vnd.demo.hateoas+json”，使用后者返回数据中包含自驱动的链接</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpPut("{id}", Name = nameof(PutUser))]
        public async Task<IActionResult> PutUser([FromRoute]long id, [FromBody]UserUpdateDto user, [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改“用户”信息(局部更新)，Patch方法，请求的mediaType是“application/json-patch+json”
        /// </summary>
        /// <param name="id">“用户”信息的id，最好存在在请求的QueryString中</param>
        /// <param name="patchDocument">请求Body中的数据格式为JSON PATCH（标准参照RFC 6902）</param>
        /// <param name="mediaType">请求的媒体类型，可选“application/json”或者“application/vnd.demo.hateoas+json”，使用后者返回数据中包含自驱动的链接</param>
        /// <returns></returns>
        [Produces("application/json", "application/vnd.demo.hateoas+json")]
        [HttpPatch("{id}", Name = nameof(PatchUser))]
        public async Task<IActionResult> PatchUser([FromRoute]long id, [FromBody]JsonPatchDocument<UserUpdateDto> patchDocument, [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 删除一条“用户”信息
        /// </summary>
        /// <param name="id">“用户”信息的Id</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser([FromRoute]long id)
        {
            throw new NotImplementedException();
        }

        #region Methods
        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private Dictionary<LinkerFactory.HttpMet, string> GetCurrentControllerItemLinker()
        {
            return new Dictionary<LinkerFactory.HttpMet, string>
            {
                {LinkerFactory.HttpMet.GET, nameof(GetUser)},
                {LinkerFactory.HttpMet.DELETE, nameof(DeleteUser)},
                {LinkerFactory.HttpMet.PATCH, nameof(PatchUser)},
                {LinkerFactory.HttpMet.PUT, nameof(PutUser)}
            };
        }
        #endregion
    }
}
