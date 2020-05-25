using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using Mark.Common;
using Mark.Common.ExtensionMethod;
using Microsoft.Extensions.Primitives;

namespace Domain.Core.Model
{
    public class QueryResponse<TResult> where TResult : IDataTransferObject
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; } = "success";
        public IEnumerable<TResult> RawData { get; set; }
        public IEnumerable<ExpandoObject> ShapedData { get; set; }
        public StringValues MoreInfo { get; set; }
        /// <summary>
        /// 数据重塑
        /// </summary>
        /// <param name="fields"></param>
        public void ShapeData(string fields)
        {
            if (RawData == null)
            {
                throw new ArgumentNullException();
            }

            ShapedData = RawData.ShapeData<TResult>(fields);
        }
    }
}
