using System.Runtime.InteropServices.ComTypes;
using BrighterTest.Lib.Commands;
using Paramore.Brighter;
using System.Text.Json;

namespace BrighterTest.Lib.MessageMappers;

public class GenericMessageMapper<T> : IAmAMessageMapper<T> where T : class, IRequest

{
    /// <inheritdoc />
    public Message MapToMessage(T request)
    {
        var name = request.GetType().FullName;

        var header = new MessageHeader(messageId: request.Id, topic: name, messageType: MessageType.MT_COMMAND);
        var body = new MessageBody(JsonSerializer.Serialize(request));
        var message = new Message(header, body);
        return message;
    }

    /// <inheritdoc />
    public T MapToRequest(Message message)
    {
        return JsonSerializer.Deserialize<T>(message.Body.Value)!;
    }
}