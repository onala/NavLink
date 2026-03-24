# HaskyNavLink

轻量级的企业内部导航站，基于 Razor Pages（.NET 8）与 MongoDB 构建。包含前后端简单的导航管理、用户登录会话管理与缓存机制，适合作为公司内部资源汇总与快速访问入口。

## 关键特性
- 基于 Razor Pages 的主页面：`Pages/Index.cshtml` 与 `Pages/Index.cshtml.cs`
- REST API 控制器：`Controllers/APIController.cs`
- 应用服务单例：`Services/AppService.cs`，应用启动时执行数据种子 `SeedAsync`
- 内存缓存用于导航与部门列表，加速页面加载
- 用户登录/会话由 `BLL/ManageLogin.cs` 与 `DAL/ManageLoginDal.cs` 管理
- 使用 MongoDB 作为数据存储（连接字符串与数据库名从配置读取）
- 已配置防伪（Antiforgery）支持自定义 Header 与 Cookie 名称

## 要求
- .NET 8 SDK
- MongoDB 可用（本地或远程）
- 推荐使用 Visual Studio Enterprise 2026（已在该环境下测试启动脚本）

## 配置
在 `appsettings.json` 或 环境变量中添加以下键：
- `ConnectionStrings:MongoDB` — MongoDB 连接字符串
- `MongoDB:DatabaseName` — 数据库名称

示例（appsettings.json）：
{ "ConnectionStrings": { "MongoDB": "mongodb://localhost:27017" }, "MongoDB": { "DatabaseName": "HaskyNavLinkDb" } }

程序中还会读取 `Antiforgery` 配置（已在 `Program.cs` 里设定自定义 Header `RequestVerificationToken` 与 Cookie `XSRF-TOKEN`）。

## 本地运行
- Visual Studio：打开解决方案，设置为启动项目后直接 F5 或 Ctrl+F5 运行。
- dotnet CLI:
  - 在项目根目录运行：
    - `dotnet restore`
    - `dotnet build`
    - `dotnet run --project .`

程序启动时会调用 `AppService.SeedAsync()` 进行必要的数据初始化。

## 主要路由 / API 示例
- Razor Pages 页面：`/`（主页，`Pages/Index.cshtml`）
- 控制器基础路由：`/API`
  - GET `/API/GetLoginUser` — 返回当前会话用户信息（由请求头与连接信息匹配）
  - GET `/API/LogOut/{user}` — 注销指定用户（立即从内存会话列表删除）
  - POST `/API/MgLogin/{sign}` — 登录；body 为 JSON 登录请求（参见 `APIController.LoginRequest`），`{sign}` 为 body 计算后的小写 SM3 哈希（服务端会验证）

示例：获取当前登录用户
curl -X GET https://localhost:5001/API/GetLoginUser

注：`MgLogin` 路由要求客户端根据项目实现签名规则生成 `sign` 参数。

## 开发注意事项
- Razor 页面中将 C# 对象序列化为 JS 变量时需用 `@Html.Raw(...)` 或包裹为 `@(...)`，避免 Razor 将其误判为非表达式或者对字符串做额外编码，示例：
- <script> const ALL_NAVS = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.NavLists)); const CURRENT_USER = "@ViewData["CurrentUserName"]"; </script>
- 如果遇到 Razor 报错 “(JS) 应为表达式。”，通常是因为直接嵌入未经包裹/Raw 的复杂字符串/JSON，使用上面方式可解决。
- 修改配置后建议执行：__Build > Clean Solution__ 然后 __Build > Rebuild Solution__。

## 常见问题
- MongoDB 连接失败：检查 `ConnectionStrings:MongoDB` 是否正确并且 MongoDB 服务可访问。
- 登录失败：确保客户端按照服务端要求为 `MgLogin` 生成正确的 `sign` 参数（SM3 加密的小写 hex）。
- 前端脚本报错包含 `</script>`：请在序列化前对该字段进行转义或采用 Base64 传输再在前端解码。

## 目录概览（关键文件）
- `Program.cs` — 应用启动、服务注册、种子初始化
- `Pages/Index.cshtml`、`Pages/Index.cshtml.cs` — 主页面与后台逻辑
- `Controllers/APIController.cs` — 提供登录、登出与会话查询 API
- `Services/AppService.cs` — 负责初始数据加载/种子
- `BLL/ManageLogin.cs`、`DAL/ManageLoginDal.cs` — 登录会话业务与数据访问
- `wwwroot/js/site.js`、`wwwroot/css/site.css` — 前端资源

欢迎按照项目结构进行扩展、测试与提交 PR。如需我把此 README 写入仓库并创建 Commit，请告知。  