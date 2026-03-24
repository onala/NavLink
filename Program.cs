using HaskyNavLink.Models;
using HaskyNavLink.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<AppService>();

//  配置防伪令牌（完全匹配你的前端 Header 名称）
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken"; // 匹配前端 Header 名称
    options.Cookie.Name = "XSRF-TOKEN";             // 自定义 Cookie 名称
    options.Cookie.SameSite = SameSiteMode.Lax;     // 兼容大多数场景
    options.Cookie.HttpOnly = false;                // 允许前端读取（非必须，方便排查）
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // 开发环境关闭 HTTPS 要求
});

//  添加控制器服务（必须）
builder.Services.AddControllers();
SysCache.ConnStr = builder.Configuration["ConnectionStrings:MongoDB"] ?? "";
SysCache.DbName = builder.Configuration["MongoDB:DatabaseName"] ?? "";


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
using (var serviceScope = app.Services.CreateScope())
{
    var service = serviceScope.ServiceProvider.GetRequiredService<AppService>();
    await service.SeedAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.Run();
