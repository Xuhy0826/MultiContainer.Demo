using System;
using System.Collections.Generic;
using System.Dynamic;
using Domain.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Domain.Core.Helper
{
    public class LinkerFactory
    {
        private readonly IUrlHelper _url;
        private Action<ExpandoObject> _addItemParamsAction;
        private Dictionary<HttpMet, string> MetDesDic = new Dictionary<HttpMet, string>
        {
            {HttpMet.GET, "get entity" },
            {HttpMet.POST, "create new entity" },
            {HttpMet.DELETE, "delete entity" },
            {HttpMet.PATCH, "update partly" },
            {HttpMet.PUT, "update totally" },
            {HttpMet.OPTIONS, "controller options" },
        };
        public LinkerFactory(IUrlHelper url)
        {
            _url = url;
        }

        /// <summary>
        /// 判断响应是否包含links
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static bool IncludeLinks(string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return false;
            }
            //判断是否是带“hateoas”的请求
            var includeLinks =
                parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            //var primaryMediaType = includeLinks
            //    ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
            //    : parsedMediaType.SubTypeWithoutSuffix;
            return includeLinks;
        }
        /// <summary>
        /// 创建自驱动链接
        /// </summary>
        /// <param name="query">条件参数</param>
        /// <param name="hasPrevious">是否有“前一页”</param>
        /// <param name="hasNext">是否有“后一页”</param>
        /// <returns></returns>
        public IEnumerable<RelativeLink> CreateLinksForCollections<TQuery>(TQuery query, string actionName, bool hasPrevious, bool hasNext) where TQuery : QueryModel
        {
            var links = new List<RelativeLink>
            {
                new RelativeLink(CreatePagedItemsResourceUri(query, ResourceUriType.CurrentPage, actionName),
                    "self", "GET")
            };
            if (hasPrevious)
            {
                links.Add(new RelativeLink(CreatePagedItemsResourceUri(query, ResourceUriType.PreviousPage, actionName),
                    "previous_page", "GET"));
            }
            if (hasNext)
            {
                links.Add(new RelativeLink(CreatePagedItemsResourceUri(query, ResourceUriType.NextPage, actionName),
                    "next_page", "GET"));
            }
            return links;

        }

        /// <summary>
        /// 创建针对单个实体的链接
        /// </summary>
        /// <param name="id">实体的id</param>
        /// <param name="fields">请求的字段名</param>
        /// <param name="linkMembers">Key: 指定具体的Http方法，Value: 指定具体的Action名称</param>
        /// <returns></returns>
        public IEnumerable<RelativeLink> CreateLinksForItem(long id, string fields, Dictionary<HttpMet, string> linkMembers)
        {
            var links = new List<RelativeLink>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                foreach (var linkMember in linkMembers)
                {
                    links.Add(new RelativeLink(_url.Link(linkMember.Value, new { id }), MetDesDic[linkMember.Key], linkMember.Key.ToString()));
                }
            }
            else
            {
                foreach (var linkMember in linkMembers)
                {
                    links.Add(new RelativeLink(_url.Link(linkMember.Value, new { id, fields }), MetDesDic[linkMember.Key], linkMember.Key.ToString()));
                }
            }
            return links;
        }

        /// <summary>
        /// 创建页数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string CreatePagedItemsResourceUri<TQuery>(TQuery query, ResourceUriType type, string actionName) where TQuery : QueryModel
        {
            var pageNumber = type switch
            {
                ResourceUriType.PreviousPage => query.PageNumber - 1,
                ResourceUriType.NextPage => query.PageNumber + 1,
                ResourceUriType.CurrentPage => query.PageNumber,
                _ => query.PageNumber
            };
            ExpandoObject queryParams = new ExpandoObject();
            queryParams.TryAdd("fields", query.Fields);
            queryParams.TryAdd("orderBy", query.OrderBy);
            queryParams.TryAdd("pageSize", query.PageSize);
            queryParams.TryAdd("searchTerm", query.SearchTerm);
            queryParams.TryAdd("pageNumber", pageNumber);

            _addItemParamsAction?.Invoke(queryParams);

            return _url.Link(actionName, queryParams);
        }
        /// <summary>
        /// 创建“新增项”的链接
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public RelativeLink CreatePostItemLink(string actionName)
        {
            return new RelativeLink(_url.Link(actionName, null), MetDesDic[HttpMet.POST], nameof(HttpMet.POST));
        }

        /// <summary>
        /// 增加其他参数的委托
        /// </summary>
        /// <param name="queryParamsAddAction"></param>
        public void AddItemParam(Action<ExpandoObject> queryParamsAddAction)
        {
            _addItemParamsAction = queryParamsAddAction;
        }

        public enum HttpMet
        {
            GET,
            POST,
            OPTIONS,
            DELETE,
            PATCH,
            PUT
        }
    }
}
