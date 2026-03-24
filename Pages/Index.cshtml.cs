using HaskyNavLink.BLL;
using HaskyNavLink.Models;
using HaskyNavLink.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace HaskyNavLink.Pages;

/// <summary>
/// 企业内部导航中心首页模型
/// 支持导航数据查询（带缓存）和修改（同步更新缓存）
/// </summary>
public class IndexModel(AppService appService, IMemoryCache cache, ILogger<IndexModel> logger) : PageModel
{
    #region 常量定义（解耦硬编码）
    /// <summary>
    /// 导航列表缓存键
    /// </summary>
    private const string NavListCacheKey = "Enterprise_NavList_All";
    private const string DepListCacheKey = "Enterprise_DepList_All";
    #endregion

    #region 公开属性
    /// <summary>
    /// 导航列表数据（供页面渲染）
    /// </summary>
    public List<GetNavList>? NavLists { get; set; } = [];

    /// <summary>
    /// 部门列表数据（供页面渲染）
    /// </summary>
    public List<DepList>? DepLists { get; set; } = [];

    /// <summary>
    /// 接收前端传入的修改参数（绑定表单/JSON）
    /// </summary>
    [BindProperty]
    public NavList? UpdateNavModel { get; set; }
    #endregion

    #region 依赖注入
    private readonly AppService _appService = appService;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<IndexModel> _logger = logger;
    #endregion

   

    /// <summary>
    /// GET请求：加载导航列表（优先缓存）
    /// </summary>
    public async Task OnGetAsync(bool refresh = false)
    {

        try
        {
            if (SysCache.NavStatue > 0 && refresh)
            {
                _cache.Remove(NavListCacheKey);
                _appService.ClearNavCache();
            }

            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
            // 获取 Accept-Language（浏览器语言设置）
            var acceptLanguage = HttpContext.Request.Headers.AcceptLanguage.ToString();
            // 获取 IP 地址（可以作为辅助标识）
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string bs = (userAgent + acceptLanguage + ipAddress).Trim();
            var rtn = ManageLogin.GetLoginUser(bs);
            ViewData["CurrentUserName"] = rtn?.LoginUserName ?? "";
            ViewData["CurrentNickName"] = rtn?.NickName ?? "";
            _logger.LogInformation("开始加载企业导航列表，缓存键：{CacheKey}", NavListCacheKey);

            if (!_cache.TryGetValue(DepListCacheKey, out List<DepList>? depLists))
            {
                depLists = _appService.GetAllDepList();
                var chcheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7));
                _cache.Set(DepListCacheKey, depLists, chcheEntryOptions);
            }
            DepLists = depLists;

            if (!_cache.TryGetValue(NavListCacheKey, out List<GetNavList>? navLists))
            {
                _logger.LogInformation("缓存未命中，从数据库加载导航列表");
                navLists = await _appService.GetAllNavListAsync() ?? [];

                // 缓存配置：永不移除 + 备注说明（企业级缓存策略）
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7)); // 增加兜底过期，避免缓存永久占用

                _cache.Set(NavListCacheKey, navLists, cacheEntryOptions);
                _logger.LogInformation("导航列表已存入缓存，共{Count}条数据", navLists.Count);
            }

            NavLists = navLists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载导航列表失败");
            ModelState.AddModelError(string.Empty, "加载网址失败，请刷新重试");
        }
    }




}