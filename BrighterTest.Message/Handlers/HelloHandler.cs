using BrighterTest.Lib.Commands;
using Paramore.Brighter;

namespace BrighterTest.Lib.Handlers;

public class HelloHandlerAsync : RequestHandlerAsync<HelloEvent>
{
    /// <inheritdoc />
    public override async Task<HelloEvent> HandleAsync(HelloEvent @event, CancellationToken cancellationToken = new CancellationToken())
    {
        Console.WriteLine("Hello {0}", @event.Name);

        return await base.HandleAsync(@event, cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
    }
}

//public class HelloHandler : RequestHandler<HelloEvent>
//{
//    /// <inheritdoc />
//    public override HelloEvent Handle(HelloEvent @event)
//    {
//        Console.WriteLine("Hello {0}", @event.Name);

//        return base.Handle(@event);
//    }
//}