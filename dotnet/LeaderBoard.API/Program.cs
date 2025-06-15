using LeaderBoard.Core;

namespace LeaderBoard.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 配置Kestrel服务器
        builder.WebHost.ConfigureKestrel(options =>
        {
            // 配置并发连接
            options.Limits.MaxConcurrentConnections = 10000;
            options.Limits.MaxConcurrentUpgradedConnections = 10000;
            
            // 配置请求限制
            options.Limits.MaxRequestBodySize = 1024 * 1024; // 1MB
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
        });

        // 添加服务到容器
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // 注册 LeaderBoardEngine 为单例服务
        builder.Services.AddSingleton<LeaderBoardEngine>();

        // 配置 CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // 配置 HTTP 请求管道
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
} 