using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Demo.User.Domain.Model.Abstract;

namespace Demo.User.Domain.Model.Dto
{
    public class UserCommandDto : IUserDto, IValidatableObject
    {
        [Display(Name = "手机号码")]
        [StringLength(15, ErrorMessage = "{0}的最大长度是15")]
        public string TelNumber { get; set; }

        [Required(ErrorMessage = "{0}是必填项")]
        [Display(Name = "用户名")]
        [StringLength(20, ErrorMessage = "{0}的最大长度是20")]
        public string Name { get; set; }

        [Display(Name = "出生日期")]
        public DateTime? BirthDay { get; set; }


        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BirthDay != null)
            {
                if (BirthDay.Value > DateTime.Now)
                {
                    yield return new ValidationResult("出生日期不能大于当前日期", new[] { nameof(BirthDay) });
                }
            }
        }
    }
}
