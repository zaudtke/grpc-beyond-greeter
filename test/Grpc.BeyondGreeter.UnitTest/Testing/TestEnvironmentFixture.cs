using System;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Grpc.BeyondGreeter.UnitTest.Testing
{
    public class TestEnvironmentFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        public GrpcChannel GrpcChannel { get; }

        public TestEnvironmentFixture()
        {
            var httpClient = CreateDefaultClient();
            httpClient.BaseAddress ??= new Uri("http://localhost");
            GrpcChannel =
                GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions {HttpClient = httpClient});
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            builder.UseEnvironment("Test")
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(NullLoggerProvider.Instance);
                });
            
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // App Config
            // Services
            // in memory db
        }
    }
}