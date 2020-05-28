using Demo.User.Domain.Model.Enums;
using Domain.Core.Model;

namespace Demo.User.Domain.Model
{
    public class UserQuery : QueryModel
    {
        public long? Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public string TelNumber { get; set; }
        public int? Level { get; set; }
    }
}
