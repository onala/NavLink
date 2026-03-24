namespace HaskyNavLink.Models
{

    /// <summary>
    /// 系统缓存
    /// </summary>
    public static class SysCache
    {
        /// <summary>
        /// 
        /// </summary>
        public static int NavStatue { get; set; } = 0;
        /// <summary>
        /// 登录用户
        /// </summary>
        public static List<AuthLoginUser>? LoginUserList { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public static string? ConnStr{get;set;}

        /// <summary>
        /// 数据库名
        /// </summary>
        public static string? DbName { get; set; }

    }

    /// <summary>
    ///     
    /// </summary>
    public class AuthLoginUser
    {
        /// <summary>
        ///  
        /// </summary>
        public string? LoginUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? BS { get; set; }
    }


    /// <summary>
    /// 认证状态
    /// </summary>
    public class AuthStateProvider
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public string? CurrentUser { get; set; }

        /// <summary>
        /// 用户改变事件
        /// </summary>
        public event Action? OnUserChanged;

        /// <summary>
        /// 设置用户
        /// </summary>
        /// <param name="user"></param>
        public void SetUser(string user)
        {
            CurrentUser = user;
            OnUserChanged?.Invoke();
        }
    }

}
