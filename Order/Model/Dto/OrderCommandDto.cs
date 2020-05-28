using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Order.Model.Dto
{
    public class OrderCommandDto : IOrderDto, IValidatableObject
    {
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (State != null)
            {
                if (State.Value < 0 || State.Value > 3)
                {
                    yield return new ValidationResult("异常信息", new[] { nameof(State) });
                }
            }
        }
    }
}
