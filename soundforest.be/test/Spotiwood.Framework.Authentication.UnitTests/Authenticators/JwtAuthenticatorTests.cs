using AutoFixture.Idioms;
using Spotiwood.Framework.Authentication.Infrastructure.Authenticators;
using Spotiwood.UnitTests.Common;
using System.Reflection;
using Xunit;

namespace Spotiwood.Framework.Authentication.UnitTests.Authenticators;
public sealed class JwtAuthenticatorTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(JwtAuthenticator).GetConstructors(BindingFlags.Public));
    }
}
