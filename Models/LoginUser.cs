using MongoDB.Bson;

namespace HaskyNavLink.Models
{
    /// <summary>
    /// 管理登录
    /// </summary>
    public class LoginUser
    {
        /// <summary>
        /// Id
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? PassWord { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public int? UserType { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string? LoginIP { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }
    }
}
