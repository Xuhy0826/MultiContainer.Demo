using Domain.Core.Attributes;

namespace Domain.Core.Model
{
    /// <summary>
    /// 查询参数封装
    /// </summary>
    public class QueryModel
    {
        private int _maxPageSize = 20;
        private int _pageSize = 5;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// 排序的字段
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 数据重塑用的字段
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// 请求的MediaType
        /// </summary>
        [SwaggerExclude]
        public string PrimaryMediaType { get; set; }
        /// <summary>
        /// 搜索用的字段
        /// </summary>
        public string SearchTerm { get; set; }
        [SwaggerExclude]
        public bool HasPreviousPage { get; set; }
        [SwaggerExclude]
        public bool HasNextPage { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
        }
        [SwaggerExclude]
        public int MaxPageSize
        {
            get => _maxPageSize;
            set => _maxPageSize = value;
        }
    }
}
