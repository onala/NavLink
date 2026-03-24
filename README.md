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