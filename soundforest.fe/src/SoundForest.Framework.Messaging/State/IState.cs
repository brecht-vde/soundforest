namespace SoundForest.Framework.Messaging.State;
public interface IStateContainer<T>
{
    public T State { get; set; }
}
