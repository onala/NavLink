using MongoDB.Bson;

namespace HaskyNavLink.Models
{
    public class DepList
    {

        /// <summary>
        /// ID
        /// </summary>
        public ObjectId? Id { get; set;}

        /// <summary>
        /// 部门名称
        /// </summary>
        public string? DepName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string? DepCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }

    }
}
