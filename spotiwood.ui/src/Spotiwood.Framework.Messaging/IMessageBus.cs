namespace Spotiwood.Framework.Messaging;
public interface IMessageBus
{
    public void Publish<T>(T message);

    public void Subscribe<T>(Action<T> syncAction);

    public void Subscribe<T>(Func<T, Task> asyncAction);
}
