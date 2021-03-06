﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Demo.$projectname$.Domain.Model.Dto
{
    public class $projectname$CommandDto : I$projectname$Dto, IValidatableObject
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
