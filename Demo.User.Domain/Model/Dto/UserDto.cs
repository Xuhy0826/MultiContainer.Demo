using Demo.User.Domain.Model.Abstract;

namespace Demo.User.Domain.Model.Dto
{
    public class UserDto : IUserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 用户电话
        /// </summary>
        public string TelNumber { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 是否VIP
        /// </summary>
        public bool IsVip { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 用户积分
        /// </summary>
        public float RewardPoint { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public byte[] Avatar { get; set; }
    }
}
