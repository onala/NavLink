using HaskyNavLink.BLL;
using HaskyNavLink.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using System.Net;
using System.Net.Sockets;
using System.Text;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace HaskyNavLink.Controllers
{
    /// <summary>
    /// API接口
    /// </summary>
    [Route("[controller]")]      // 添加备用路由: /API
    [ApiController]
    public class APIController : Controller
    {
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet("LogOut/{user:required}")]
        public async Task<IActionResult> LogOut([FromRoute] string user)
        {
            var rtn = ManageLogin.DeleteUser(user);
            return Ok(new { code = rtn ? 1 : 0, message = rtn ? "注销成功" : "注销失败" });
        }

        /// <summary>
        ///  获取登录用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLoginUser")]
        public async Task<IActionResult> GetLoginUser()
        {
            // 获取 User-Agent（包含浏览器类型、版本、操作系统等信息）
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
            // 获取 Accept-Language（浏览器语言设置）
            var acceptLanguage = HttpContext.Request.Headers.AcceptLanguage.ToString();
            // 获取 IP 地址（可以作为辅助标识）
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string bs = (userAgent + acceptLanguage + ipAddress).Trim();
            var rtn = ManageLogin.GetLoginUser(bs);
            return Ok(new
            {
                code = rtn != null ? 1 : 0,
                message = rtn != null ? rtn.ToJson() : "{}",
            });

        }



        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("MgLogin/{sign:required}")]
        public async Task<IActionResult> MgLogin([FromRoute] string sign)
        {
            var bodystream = HttpContext.Request.BodyReader.AsStream();
            var reader = new StreamReader(bodystream, Encoding.UTF8, true);
            var tvalcd = await reader.ReadToEndAsync();
            var checksign = tvalcd.SM3Encrypt().ToLower();

            if (checksign != sign)
                return BadRequest(new { error = "参数无效" });

            var request = JsonConvert.DeserializeObject<LoginRequest>(tvalcd);

            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest(new { error = "参数无效" });
            // 获取 User-Agent（包含浏览器类型、版本、操作系统等信息）
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
            // 获取 Accept-Language（浏览器语言设置）
            var acceptLanguage = HttpContext.Request.Headers.AcceptLanguage.ToString();
            // 获取 IP 地址（可以作为辅助标识）
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string bs = (userAgent + acceptLanguage + ipAddress).Trim();
            string ip = GetClientIp(HttpContext);
            var success = await BLL.ManageLogin.Login(request.UserName, request.PassWord ?? "", bs, ip);

            return Ok(new
            {
                code = success ? 1 : 0,
                message = success ? "登录成功" : "用户名或密码错误"
            });


            static string GetClientIp(HttpContext context)
            {
                string text = string.Empty;
                if (context.Connection.RemoteIpAddress != null)
                {
                    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var value))
                    {
                        text = value.LastOrDefault() ?? string.Empty;
                    }
                    else if (context.Request.Headers.TryGetValue("X-Real-IP", out var value2))
                    {
                        text = value2.FirstOrDefault() ?? string.Empty;
                    }

                    if (string.IsNullOrEmpty(text))
                    {
                        text = ((!((context.Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString())?.ToString().Trim() == "0.0.0.1")) ? GetLanIp(context) : "127.0.0.1");
                    }
                }

                return text;
            }

            static string GetLanIp(HttpContext context)
            {
                if (context.Request.Host.Host != null)
                {
                    IPAddress[] hostAddresses = Dns.GetHostAddresses(context.Request.Host.Host);
                    foreach (IPAddress iPAddress in hostAddresses)
                    {
                        if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return iPAddress.ToString();
                        }
                    }
                }

                return string.Empty;
            }
        }



        /// <summary>
        /// 登录请求参数
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            ///  用户名
            /// </summary>
            public string? UserName { get; set; }
            /// <summary>
            ///  密码
            /// </summary>
            public string? PassWord { get; set; }
        }

        /// <summary>
        /// 登出请求参数
        /// </summary>
        public class LogOutRequest
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string? UserName { get; set; }
        }
    }
}
