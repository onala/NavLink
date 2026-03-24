using HaskyNavLink.DAL;
using HaskyNavLink.Models;
using MongoDB.Bson;

namespace HaskyNavLink.BLL
{
    public class NavListBll()
    {

        /// <summary>
        /// 获取导航列表（支持分页）
        /// </summary>
        public static List<GetNavList> GetNavList(int pageIndex = 1, int pageSize = 20)
        {
            var rtn = NavListDal.GetAllNavList(pageIndex, pageSize);
            List<GetNavList> rtnList = [];
            foreach (var n in rtn)
            {
                rtnList.Add(new GetNavList
                {
                    Id = n.Id.ToString(),
                    NavName = n.NavName,
                    NavLink = n.NavLink,
                    OrderBy = n.OrderBy,
                    IsStatue = n.IsStatue,
                    Creater = n.Creater,
                    DepName = n.DepName,
                    CreateTime = n.CreateTime,
                    IsRec = n.IsRec,
                    ReMark = n.ReMark,
                    NickName = n.NickName
                });
            }
            return rtnList;
        }

        /// <summary>
        /// 搜索导航
        /// </summary>
        public static List<GetNavList> SearchNav(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            return string.IsNullOrWhiteSpace(keyword)
                ? GetNavList(pageIndex, pageSize)
                : [];
        }

        /// <summary>
        /// 新增导航
        /// </summary>
        public static async Task<int> AddNavList(NavList nav)
        {
            // 业务校验：导航名称和链接不能为空
            if (string.IsNullOrWhiteSpace(nav.NavName) || string.IsNullOrWhiteSpace(nav.NavLink))
                return -1; // 校验失败
            var rtn = await NavListDal.AddNavList(nav);
            if (rtn > 0)
            {
                SysCache.NavStatue = 1;
            }
            return rtn;
        }

        /// <summary>
        /// 修改导航
        /// </summary>
        public static async Task<int> UpdateNavList(EditNavList nav)
        {
            if (string.IsNullOrEmpty(nav.Id) || string.IsNullOrWhiteSpace(nav.NavName))
                return -1;

            NavList nl = new()
            {
                Id = ObjectId.Parse(nav.Id),
                NavName = nav.NavName,
                NavLink = nav.NavLink,
                OrderBy = nav.OrderBy,
                IsStatue = nav.IsStatue,
                Creater = nav.Creater,
                DepName = nav.DepName,
                CreateTime = nav.CreateTime,
                IsRec = nav.IsRec,
                ReMark = nav.ReMark
            };
            var rtn = await NavListDal.UpdateNavList(nl);
            if (rtn > 0)
            {
                SysCache.NavStatue = 1;
            }
            return rtn;
        }

        /// <summary>
        /// 删除导航（软删除）
        /// </summary>
        public static async Task<int> DeleteNavList(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return -1;
            var rtn = await NavListDal.DeleteNavList(id);
            if (rtn > 0)
            {
                SysCache.NavStatue = 1;
            }
            return rtn;
        }

        /// <summary>
        /// 根据ID获取导航
        /// </summary>
        public static async Task<NavList?> GetNavById(ObjectId id)
        {
            return await NavListDal.GetNavById(id);
        }
    }
}