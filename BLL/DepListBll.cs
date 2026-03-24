using HaskyNavLink.DAL;
using HaskyNavLink.Models;

namespace HaskyNavLink.BLL
{
    public class DepListBll
    {
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        public static List<DepList> GetDepList()
        {
            var rtn = DepListDal.GetAllNavList();
            return rtn;
        }

    }
}
