using Mark.Common;

namespace Domain.Core.Model
{
    public class CommandModel<TDto>
    {
        public TDto DataObject { get; set; }
    }
}
