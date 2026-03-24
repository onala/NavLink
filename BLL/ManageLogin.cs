using HaskyNavLink.Common;
using HaskyNavLink.DAL;
using HaskyNavLink.Models;
using MongoDB.Bson;

namespace HaskyNavLink.BLL
{
    internal class ManageLogin
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static async Task<bool> Login(string userName, string passWord, string? bs, string ip)
        {
            var user = await ManageLoginDal.Login(userName);
            if (user == null)
            {
                return false;
            }

            if (user?.PassWord == passWord.SM3Encrypt())
            {
                AddUser(userName, bs, user?.NickName ?? "匿名");
                await UpdateUser(userName, ip);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 检查用户名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<bool> CheckUserName(string username, string bs)
        {
            var user = SysCache.LoginUserList?.Find(f => f.LoginUserName == username && f.BS == bs);
            return user != null;
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static AuthLoginUser? GetLoginUser(string bs)
        {
            SysCache.LoginUserList ??= [];
            var user = SysCache.LoginUserList?.Find(f => f.BS == bs);
            return user;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="bs"></param>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static bool AddUser(string username, string? bs, string nickname)
        {
            try
            {
                var user = SysCache.LoginUserList?.Find(f => f.LoginUserName == username);
                if (user != null)
                {
                    SysCache.LoginUserList?.Remove(user);
                }
                (SysCache.LoginUserList ?? []).Add(new AuthLoginUser { LoginUserName = username, LoginTime = DateTime.Now, BS = bs, NickName = nickname });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <returns></returns>

        public static async Task<bool> UpdateUser(string username, string ip)
        {
            try
            {
                var user = await DAL.ManageLoginDal.Login(username);
                if (user == null) return false;
                LoginUser lu = new()
                {
                    CreateTime = user?.CreateTime ?? DateTime.Now,
                    LoginIP = ip,
                    LoginTime = DateTime.Now,
                    UserName = username,
                    UserType = user?.UserType ?? 0,
                    NickName = user?.NickName ?? "匿名",
                    Id = user?.Id ?? ObjectId.GenerateNewId(),
                    PassWord = user?.PassWord ?? string.Empty
                };
                var ru = await ManageLoginDal.UpdateUser(lu);
                return ru;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool DeleteUser(string username)
        {
            try
            {
                var user = SysCache.LoginUserList?.Find(f => f.LoginUserName == username);
                if (user != null)
                {
                    SysCache.LoginUserList?.Remove(user);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
