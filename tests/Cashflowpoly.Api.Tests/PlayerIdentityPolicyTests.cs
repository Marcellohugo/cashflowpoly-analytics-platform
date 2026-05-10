using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk policy identitas user-player.
/// </summary>
public sealed class PlayerIdentityPolicyTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa profil pemain kanonik memakai ID user untuk kompatibilitas data yang ada.
    /// </summary>
    public void GetCanonicalPlayerId_ReturnsUserId()
    {
        var userId = Guid.NewGuid();

        var playerId = PlayerIdentityPolicy.GetCanonicalPlayerId(userId);

        Assert.Equal(userId, playerId);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi owner profil pemain memakai instruktur ketika akun dibuat oleh instruktur.
    /// </summary>
    public void ResolvePlayerOwnerUserId_UsesInstructorOwner_WhenProvided()
    {
        var playerUserId = Guid.NewGuid();
        var instructorUserId = Guid.NewGuid();

        var owner = PlayerIdentityPolicy.ResolvePlayerOwnerUserId(playerUserId, instructorUserId);

        Assert.Equal(instructorUserId, owner);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi owner fallback untuk registrasi mandiri tetap user pemain demi kompatibilitas query lama.
    /// </summary>
    public void ResolvePlayerOwnerUserId_FallsBackToPlayerUserId_WhenInstructorOwnerMissing()
    {
        var playerUserId = Guid.NewGuid();

        var owner = PlayerIdentityPolicy.ResolvePlayerOwnerUserId(playerUserId, instructorUserId: null);

        Assert.Equal(playerUserId, owner);
    }
}
