using MongoDB.Bson;

namespace HaskyNavLink.Models
{
    public class EditNavList
    {
        /// <summary>
        ///  id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 导航链接
        /// </summary>
        public string? NavLink { get; set; }


        /// <summary>
        ///  导航名称
        /// </summary>
        public string? NavName { get; set; }

        /// <summary>
        ///  创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 创建者
        /// </summary>
        public string? Creater { get; set; }


        /// <summary>
        /// 部门名称
        /// </summary>
        public string? DepName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? IsStatue { get; set; } = 1;

        /// <summary>
        /// 排序
        /// </summary>
        public int? OrderBy { get; set; } = 0;

        /// <summary>
        /// 是否推荐
        /// </summary>
        public int? IsRec { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string? ReMark { get; set; }

        /// <summary>
        /// 部门排序
        /// </summary>
        public int? DepOrderBy { get; set; } = 1;

    }
}
