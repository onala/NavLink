using HaskyNavLink.Common;
using HaskyNavLink.Models;
using MongoDB.Driver;

namespace HaskyNavLink.DAL
{
    internal class DepListDal : DB<DepList>
    {
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        public static List<DepList> GetAllNavList()
        {
            try
            {
                var query = Db.GetCollection().Find(f => true);
                return query.ToList();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "获取部门列表失败");
                return [];
            }
        }

    }
}
