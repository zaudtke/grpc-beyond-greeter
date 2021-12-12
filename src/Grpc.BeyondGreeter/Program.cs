//using System.Runtime.InteropServices;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Server.Kestrel.Core;
//using Microsoft.Extensions.Hosting;

//namespace Grpc.BeyondGreeter
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        // Additional configuration is required to successfully run gRPC on macOS.
//        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
//                    {
//                        webBuilder.ConfigureKestrel(options =>
//                        {
//                            options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2);
//                        });
//                    }
//                    webBuilder.UseStartup<Startup>();
//                });
//    }
//}


using Grpc.BeyondGreeter.Features.Greetings;
using Grpc.BeyondGreeter.Features.Members;
using Grpc.BeyondGreeter.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2);
    });
}

builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<RpcExceptionInterceptor>();
});

var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<GreeterService>();
    endpoints.MapGrpcService<MembershipService>();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });

});

app.Run();

public partial class Program { }