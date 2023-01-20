using System.Collections.Concurrent;

namespace SoundForest.Framework.Messaging;
internal class MessageBus : IMessageBus
{
    private readonly ConcurrentBag<KeyValuePair<string, Delegate>> _subscribers;

    public MessageBus()
    {
        _subscribers = new ConcurrentBag<KeyValuePair<string, Delegate>>();
    }

    public void Publish<T>(T message)
        where T : IMessageEvent
    {
        try
        {
            var name = typeof(T).Name;

            foreach (var subscriber in _subscribers.Where(s => s.Key?.Equals(name, StringComparison.OrdinalIgnoreCase) is true))
            {
                var action = subscriber.Value as Action<T>;
                action?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void Subscribe<T>(Action<T> action)
        where T : IMessageEvent
    {
        try
        {
            var name = typeof(T).Name;

            if (!_subscribers.Any(
                s => s.Key.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                     s.Value.Equals(action)))
            {
                _subscribers.Add(new KeyValuePair<string, Delegate>(name, action));
            }
        }
        catch
        {
        }
    }
}
