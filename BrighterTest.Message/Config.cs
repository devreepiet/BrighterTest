using BrighterTest.Lib.Commands;
using BrighterTest.Lib.MessageMappers;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;

namespace BrighterTest.Lib;

public static class Config
{
    public const string ExchangeName = "brightertest.exchange";
    public const string AmqpUri      = "amqp://guest:guest@localhost:5672";

    public static IServiceCollection AddBrighterSender(this IServiceCollection services)
    {
        var rmqConnection = new RmqMessagingGatewayConnection
        {
            AmpqUri  = new AmqpUriSpecification(new Uri(AmqpUri)),
            Exchange = new Exchange(ExchangeName),
        };

        var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnection);

        // Add services to the container.
        services.AddBrighter(options =>
                 {
                     options.ChannelFactory = new ChannelFactory(rmqMessageConsumerFactory);
                 })
                .MapperRegistry(registry => registry.AddGeneric())
                .UseExternalBus(
                     new RmqProducerRegistryFactory(
                         rmqConnection,
                         GetPublications()).Create(),
                     true);

        return services;
    }

    public static IServiceCollection AddBrighterReceiver(this IServiceCollection services)
    {
        var rmqConnection = new RmqMessagingGatewayConnection
        {
            AmpqUri  = new AmqpUriSpecification(new Uri(AmqpUri)),
            Exchange = new Exchange(ExchangeName)
        };

        var channel = new ChannelFactory(new RmqMessageConsumerFactory(rmqConnection));

        var libraryAssembly = typeof(GenericMessageMapper<>).Assembly;
        services.AddServiceActivator(options =>
                 {
                     options.Subscriptions  = GetSubscriptions();
                     options.ChannelFactory = channel;
                 })
                .MapperRegistry(registry => registry.AddGeneric())
                .AsyncHandlersFromAssemblies(libraryAssembly)
                 //.HandlersFromAssemblies(libraryAssembly)
                .UseExternalBus(
                     new RmqProducerRegistryFactory(
                         rmqConnection,
                         GetPublications()).Create(),
                     true);

        return services;
    }

    private static IEnumerable<Subscription> GetSubscriptions()
    {
        var subscriptions = new List<Subscription>();

        foreach (var type in GetTypes())
        {
            var fullName = type.FullName;

            subscriptions.Add(new RmqSubscription(type,
                                                  new SubscriptionName(fullName),
                                                  new ChannelName(fullName),
                                                  new RoutingKey(fullName),
                                                  isDurable: true,
                                                  highAvailability: true));
        }

        return subscriptions;
    }

    private static IEnumerable<RmqPublication> GetPublications()
    {
        var publications = new List<RmqPublication>();

        foreach (var type in GetTypes())
        {
            publications.Add(new RmqPublication()
            {
                MakeChannels = OnMissingChannel.Create,
                Topic        = new RoutingKey(type.FullName)
            });
        }

        return publications;
    }

    private static List<Type> GetTypes()
    {
        var type = typeof(GenericMessageMapper<>);

        //  Get all the Commands from the project
        var commands = type.Assembly
                           .GetTypes()
                           .Where(x => x.IsClass && !x.IsAbstract &&
                                       x.GetInterfaces().Contains(typeof(IRequest)))
                           .ToList();

        return commands;
    }

    private static ServiceCollectionMessageMapperRegistry AddGeneric(
        this ServiceCollectionMessageMapperRegistry registry)
    {
        var types = GetTypes().Where(type =>
                                         type.GetInterfaces()
                                             .Contains(typeof(IRequest)));

        foreach (var type in types)
        {
            registry.Add(type, typeof(GenericMessageMapper<>).MakeGenericType(type));
        }

        return registry;
    }
}