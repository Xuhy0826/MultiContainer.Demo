using System;
using Demo.User.Domain.Model.Enums;
using Domain.Abstractions;

namespace Demo.User.Domain.Aggregate
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class User : Entity<long>, IAggregateRoot
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelNumber { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HomeAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool VipFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public float RewardPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
