namespace HaskyNavLink.Models;

/// <summary>
/// 导航项实体类
/// </summary>
public class NavLinkItem
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 导航名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 导航URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}