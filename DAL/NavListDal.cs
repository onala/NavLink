using HaskyNavLink.Common;
using HaskyNavLink.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace HaskyNavLink.DAL
{
    /// <summary>
    /// 导航列表数据访问层
    /// </summary>
    internal class NavListDal : DB<NavList>
    {
        /// <summary>
        /// 获取所有导航（支持分页和排序）
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static List<NavList> GetAllNavList(int pageIndex = 1, int pageSize = int.MaxValue)
        {
            try
            {
                var query = Db.GetCollection().Find(f => f.IsStatue == 1) // 只查启用状态
                              .SortByDescending(f => f.OrderBy) // 按排序号降序
                              .Skip((pageIndex - 1) * pageSize)
                              .Limit(pageSize);
                return query.ToList();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "获取导航列表失败");
                return [];
            }
        }

        /// <summary>
        /// 新增导航
        /// </summary>
        public static async Task<int> AddNavList(NavList nl)
        {
            try
            {
                nl.Id = ObjectId.GenerateNewId();
                nl.CreateTime = DateTime.Now;
                var result = await Db.CreateAsync(nl);
                return result != null ? 1 : 0;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "新增导航失败");
                return 0;
            }
        }

        /// <summary>
        /// 修改导航
        /// </summary>
        public static async Task<int> UpdateNavList(NavList nl)
        {
            try
            {
                Expression<Func<NavList, bool>> filter = f => f.Id == nl.Id;
                var update = Builders<NavList>.Update
                    .Set(f => f.NavName, nl.NavName)
                    .Set(f => f.NavLink, nl.NavLink)
                    .Set(f => f.DepName, nl.DepName)
                    .Set(f => f.IsStatue, nl.IsStatue)
                    .Set(f => f.OrderBy, nl.OrderBy)
                    .Set(f => f.IsRec, nl.IsRec)
                    .Set(f => f.ReMark, nl.ReMark);

                var result = await Db.UpdateAsync(filter, update);
                return (int)result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "修改导航失败");
                return 0;
            }
        }

        /// <summary>
        /// 删除导航（软删除：修改状态为0）
        /// </summary>
        public static async Task<int> DeleteNavList(ObjectId id)
        {
            try
            {
                Expression<Func<NavList, bool>> filter = f => f.Id == id;
                var update = Builders<NavList>.Update.Set(f => f.IsStatue, 0);
                var result = await Db.UpdateAsync(filter, update);
                return (int)result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "删除导航失败");
                return 0;
            }
        }

        /// <summary>
        /// 根据ID获取单个导航
        /// </summary>
        public static async Task<NavList?> GetNavById(ObjectId id)
        {
            try
            {
                var result = await Db.GetByIdAsync(f => f.Id == id);
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "根据ID获取导航失败");
                return null;
            }
        }

        /// <summary>
        /// 搜索导航（按名称/部门）
        /// </summary>
        public static List<NavList> SearchNav(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                var filter = Builders<NavList>.Filter.And(
                    Builders<NavList>.Filter.Eq(f => f.IsStatue, 1),
                    Builders<NavList>.Filter.Or(
                        Builders<NavList>.Filter.Regex(f => f.NavName, new BsonRegularExpression(keyword, "i")),
                        Builders<NavList>.Filter.Regex(f => f.DepName, new BsonRegularExpression(keyword, "i"))
                    )
                );

                var query = Db.GetCollection().Find(filter)
                              .SortByDescending(f => f.OrderBy)
                              .Skip((pageIndex - 1) * pageSize)
                              .Limit(pageSize);
                return query.ToList();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "搜索导航失败");
                return [];
            }
        }
    }
}