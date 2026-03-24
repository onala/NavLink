using HaskyNavLink.Common;
using HaskyNavLink.Models;

namespace HaskyNavLink.DAL
{
    /// <summary>
    /// 管理登录
    /// </summary>
    internal class ManageLoginDal : DB<LoginUser>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<LoginUser?> Login(string username)
        {
            var rtn = await Db.GetByIdAsync(f => f.UserName == username);
            return rtn?.FirstOrDefault();
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateUser(LoginUser user)
        {
            var rtn = await Db.UpdateEntityAsync(f => f.Id == user.Id, user);
            return rtn > 0;
        }

    }
}
