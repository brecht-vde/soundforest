using AutoFixture.Idioms;
using SoundForest.UnitTests.Common;
using SoundForest.Framework.Authentication.Infrastructure.Authenticators;
using System.Reflection;
using Xunit;

namespace SoundForest.Framework.Authentication.UnitTests.Authenticators;
public sealed class JwtAuthenticatorTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(JwtAuthenticator).GetConstructors(BindingFlags.Public));
    }
}
