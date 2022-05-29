using Paramore.Brighter;

namespace BrighterTest.Lib.Commands;

public class HelloEvent : Command
{
    public HelloEvent(string name) : base(Guid.NewGuid())
    {
        Name = name;
    }
    
    public string Name { get; set; }
}