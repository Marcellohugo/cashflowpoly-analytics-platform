// Fungsi file: Memastikan batas bcrypt dan kebijakan registrasi publik tidak dapat dilonggarkan tanpa sengaja.
using Cashflowpoly.Api.Security;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AuthPolicyTests
{
    [Fact]
    public void ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors()
    {
        Assert.True(PasswordPolicy.IsWithinBcryptLimit(new string('a', 72)));
        Assert.False(PasswordPolicy.IsWithinBcryptLimit(new string('a', 73)));
        Assert.False(PasswordPolicy.IsWithinBcryptLimit(string.Concat(Enumerable.Repeat("😀", 19))));

        var registration = new AuthRegistrationOptions();
        Assert.True(registration.CanRegisterPublicly("PLAYER"));
        Assert.False(registration.CanRegisterPublicly("INSTRUCTOR"));
    }
}
