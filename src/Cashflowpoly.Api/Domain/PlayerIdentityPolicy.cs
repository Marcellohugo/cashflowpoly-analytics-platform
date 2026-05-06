// Fungsi file: Mendefinisikan aturan identitas kanonik antara app_users dan players.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Policy identitas user-player yang mempertahankan kompatibilitas data sambil memusatkan aturan ID kanonik.
/// </summary>
internal static class PlayerIdentityPolicy
{
    /// <summary>
    /// Menghasilkan player_id kanonik untuk user PLAYER.
    /// </summary>
    internal static Guid GetCanonicalPlayerId(Guid userId)
    {
        return userId;
    }

    /// <summary>
    /// Menentukan pemilik profil pemain untuk query instruktur.
    /// </summary>
    internal static Guid ResolvePlayerOwnerUserId(Guid playerUserId, Guid? instructorUserId)
    {
        return instructorUserId ?? playerUserId;
    }
}
