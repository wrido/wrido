namespace Wrido.Messages
{
    public abstract class MessageBase
    {
      protected MessageBase()
      {
        Type = GetType().Name;
      }

      public string Type { get; }
    }
}
