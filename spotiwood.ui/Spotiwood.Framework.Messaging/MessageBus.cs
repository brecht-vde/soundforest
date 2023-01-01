using System.Collections.Concurrent;

namespace Spotiwood.Framework.Messaging;
internal class MessageBus : IMessageBus
{
    private readonly ConcurrentBag<KeyValuePair<Type, Delegate>> _subscribers;

    public MessageBus()
    {
        _subscribers = new ConcurrentBag<KeyValuePair<Type, Delegate>>();
    }

    public void Publish<T>(T message)
    {
        try
        {
            foreach (var subscriber in _subscribers.Where(s => s.Key?.Equals(typeof(T)) is true))
            {
                if (subscriber.Value?.GetType()?.Equals(typeof(Action<T>)) is true)
                {
                    (subscriber.Value as Action<T>)?.Invoke(message);
                    continue;
                }

                if (subscriber.Value?.GetType()?.Equals(typeof(Func<T, Task>)) is true)
                {
                    (subscriber.Value as Func<T, Task>)?.Invoke(message);
                    continue;
                }
            }
        }
        catch
        {
        }
    }

    public void Subscribe<T>(Action<T> syncAction)
    {
        try
        {
            Register<T>(syncAction);
        }
        catch
        {
        }
    }

    public void Subscribe<T>(Func<T, Task> asyncAction)
    {
        try
        {
            Register<T>(asyncAction);
        }
        catch
        {
        }
    }

    private void Register<T>(Delegate action)
    {
        try
        {
            if (!_subscribers.Any(s =>
                s.Key.Equals(typeof(T)) &&
                s.Value.Equals(action)))
            {
                _subscribers.Add(new KeyValuePair<Type, Delegate>(typeof(T), action));
            }
        }
        catch
        {
        }
    }
}
