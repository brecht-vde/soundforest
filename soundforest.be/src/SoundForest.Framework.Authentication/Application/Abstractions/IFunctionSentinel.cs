namespace SoundForest.Framework.Authentication.Application.Abstractions;
public interface IFunctionSentinel
{
    public bool RequiresElevation(string functionName);
}
