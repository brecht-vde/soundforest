namespace SoundForest.Framework.Messaging;
public interface IMessageBus
{
    public void Publish<T>(T message)
        where T : IMessageEvent;

    public void Subscribe<T>(Action<T> action)
        where T : IMessageEvent;
}
