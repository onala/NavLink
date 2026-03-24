using HaskyNavLink.BLL;
using HaskyNavLink.Models;
using MongoDB.Bson;

namespace HaskyNavLink.Services
{
    public class AppService
    {
        private List<GetNavList>? _navList;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private int _totalCount; // 总记录数

        private List<DepList>? _depList;

        /// <summary>
        /// 初始化导航数据
        /// </summary>
        public async Task SeedAsync()
        {

            await GetAllNavListAsync();
            GetAllDepList();
        }

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public List<DepList> GetAllDepList()
        {
            if (_depList == null || _depList.Count == 0)
            {
                _depList = DepListBll.GetDepList();
            }
            return _depList;
        }


        /// <summary>
        /// 获取所有导航（带缓存）
        /// </summary>
        public async Task<List<GetNavList>?> GetAllNavListAsync(int pageIndex = 1, int pageSize = 200)
        {
            // 缓存未命中或分页时重新查询
            if (_navList == null || pageIndex > 1)
            {
                _navList = await Task.Run(() => NavListBll.GetNavList(pageIndex, pageSize));
                _totalCount = _navList.Count; // 实际项目中建议单独查询总条数
            }
            return _navList;

        }

        /// <summary>
        /// 搜索导航
        /// </summary>
        public async Task<List<GetNavList>?> SearchNavAsync(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            await _semaphore.WaitAsync();
            try
            {
                _navList = await Task.Run(() => NavListBll.SearchNav(keyword, pageIndex, pageSize));
                _totalCount = _navList.Count;
                return _navList;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 新增导航
        /// </summary>
        public async Task<int> AddNavAsync(NavList nav)
        {
            var result = await NavListBll.AddNavList(nav);
            if (result > 0) ClearNavCache(); // 新增成功清空缓存
            return result;
        }

        /// <summary>
        /// 修改导航
        /// </summary>
        public async Task<bool> UpdateNavListAsync(EditNavList nav)
        {
            var result = await NavListBll.UpdateNavList(nav);
            if (result > 0) ClearNavCache();
            return result > 0;
        }




        /// <summary>
        /// 删除导航
        /// </summary>
        public async Task<int> DeleteNavAsync(ObjectId id)
        {
            var result = await NavListBll.DeleteNavList(id);
            if (result > 0) ClearNavCache();
            return result;
        }

        /// <summary>
        /// 根据ID获取导航
        /// </summary>
        public static async Task<NavList?> GetNavByIdAsync(ObjectId id)
        {
            return await NavListBll.GetNavById(id);
        }

        /// <summary>
        /// 清空导航缓存
        /// </summary>
        public void ClearNavCache()
        {
            _semaphore.Wait();
            try
            {
                _navList = null;
                _totalCount = 0;
                SysCache.NavStatue = 0;
                SeedAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        /// <summary>
        /// 获取总记录数
        /// </summary>
        public int GetTotalCount() => _totalCount;
    }
}