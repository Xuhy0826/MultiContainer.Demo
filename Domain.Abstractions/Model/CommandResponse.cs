using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using Mark.Common;

namespace Domain.Core.Model
{
    public class CommandResponse<TResult> where TResult : IDataTransferObject
    {
        public bool ModelValid { get; set; } = true;
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; } = "success";
        public IEnumerable<TResult> RawData { get; set; }
        public IEnumerable<ExpandoObject> ShapedData { get; set; }
    }
}
