using HaskyNavLink.Common;
using HaskyNavLink.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Text;

namespace HaskyNavLink.Controllers
{
    /// <summary>
    /// 导航控制器
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class NavController() : Controller
    {
        /// <summary>
        /// 添加导航
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("AddNav/{sign:required}")]
        public async Task<IActionResult> AddNav([FromRoute] string sign)
        {
            var bodystream = HttpContext.Request.BodyReader.AsStream();
            var reader = new StreamReader(bodystream, Encoding.UTF8, true);
            var tvalcd = await reader.ReadToEndAsync();
            var checksign = tvalcd.SM3Encrypt().ToLower();

            if (checksign != sign)
                return BadRequest(new { error = "参数无效" });

            var request = JsonConvert.DeserializeObject<NavList>(tvalcd);
            var success = await BLL.NavListBll.AddNavList(request ?? new());
            return Ok(new
            {
                code = success,
                message = success > 0 ? "添加导航成功" : "添加导航失败，请稍后再试"
            });

        }

        /// <summary>
        /// 编辑导航
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("EditNav/{sign:required}")]
        public async Task<IActionResult> EditNav([FromRoute] string sign)
        {
            var bodystream = HttpContext.Request.BodyReader.AsStream();
            var reader = new StreamReader(bodystream, Encoding.UTF8, true);
            var tvalcd = await reader.ReadToEndAsync();
            var checksign = tvalcd.SM3Encrypt().ToLower();

            if (checksign != sign)
                return BadRequest(new { error = "参数无效" });

            var request = JsonConvert.DeserializeObject<EditNavList>(tvalcd);

            var success = await BLL.NavListBll.UpdateNavList(request ?? new());

            return Ok(new
            {
                code = success,
                message = success > 0 ? "编辑导航成功" : "编辑导航失败，请稍后再试"
            });

        }

        /// <summary>
        /// 删除导航
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("DelNav/{sign:required}")]
        public async Task<IActionResult> DelNav([FromRoute] string sign)
        {
            var bodystream = HttpContext.Request.BodyReader.AsStream();
            var reader = new StreamReader(bodystream, Encoding.UTF8, true);
            var tvalcd = await reader.ReadToEndAsync();
            var checksign = tvalcd.SM3Encrypt().ToLower();

            if (checksign != sign)
                return BadRequest(new { error = "参数无效" });

            var request = JsonConvert.DeserializeObject<DelNavList>(tvalcd);
            var delid = new ObjectId(request?.Id);
            var success = await BLL.NavListBll.DeleteNavList(delid);

            return Ok(new
            {
                code = success,
                message = success > 0 ? "删除导航成功" : "删除导航失败，请稍后再试"
            });

        }

    }
}
