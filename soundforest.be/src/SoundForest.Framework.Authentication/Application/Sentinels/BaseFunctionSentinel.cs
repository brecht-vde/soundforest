using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using SoundForest.Framework.Authentication.Application.Abstractions;
using System.Collections.Concurrent;
using System.Reflection;

namespace SoundForest.Framework.Authentication.Application.Sentinels;
internal sealed class FunctionSentinel : IFunctionSentinel
{
    private readonly ConcurrentBag<string> _elevatedFunctions = new ConcurrentBag<string>();

    public FunctionSentinel(params Type[] markers)
        : this(markers.ToList())
    {

    }

    public FunctionSentinel(IEnumerable<Type> markers)
        : this(markers.Select(marker => marker.Assembly))
    {

    }

    public FunctionSentinel(params Assembly[] assemblies)
        : this(assemblies.ToList())
    {

    }

    public FunctionSentinel(IEnumerable<Assembly> assemblies)
    {
        assemblies = assemblies?.Any() is not true ? AppDomain.CurrentDomain.GetAssemblies() : assemblies;

        var functions = assemblies.SelectMany(assembly => assembly.DefinedTypes)
            .SelectMany(type => type.DeclaredMethods)
            .Where(method => method.GetCustomAttributes<FunctionAttribute>().Any());

        foreach (var function in functions)
        {
            var requiresElevation = function.GetCustomAttributes<AuthorizeAttribute>().Any() is true;

            if (requiresElevation)
            {
                var name = function.GetCustomAttribute<FunctionAttribute>()!.Name;
                _elevatedFunctions.Add(name);
            }
        }
    }

    public bool RequiresElevation(string functionName)
        => _elevatedFunctions.Contains(functionName);
}
