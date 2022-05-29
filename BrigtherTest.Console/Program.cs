using BrighterTest.Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
                     .ConfigureServices(ConfigureServices)
                     .Build();

host.Run();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddBrighterReceiver();

    services.AddHostedService<ServiceActivatorHostedService>();
}