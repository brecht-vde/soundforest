namespace SoundForest.Framework.Messaging;
public abstract record DataMessageEvent<T> : IMessageEvent
{
    public DataMessageEvent(T data)
    {
        Data = data;
    }

    public T Data { get; init; }
}
