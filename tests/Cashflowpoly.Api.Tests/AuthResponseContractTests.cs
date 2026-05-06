// Fungsi file: Menguji kontrak JSON respons autentikasi API.
using System.Text.Json;
using Cashflowpoly.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk kontrak response login dan register.
/// </summary>
public sealed class AuthResponseContractTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi respons login menyertakan display_name agar client tidak perlu lookup profil pemain.
    /// </summary>
    public void LoginResponse_SerializesDisplayName()
    {
        var response = new LoginResponse(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "player_one",
            "PLAYER",
            "Player One",
            "token",
            DateTimeOffset.Parse("2026-01-01T00:00:00Z"));

        var json = JsonSerializer.Serialize(response);

        Assert.Contains(@"""display_name"":""Player One""", json);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi respons register menyertakan display_name agar UI dapat mengisi sesi dari respons auth.
    /// </summary>
    public void RegisterResponse_SerializesDisplayName()
    {
        var response = new RegisterResponse(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "player_two",
            "PLAYER",
            "Player Two",
            "token",
            DateTimeOffset.Parse("2026-01-01T00:00:00Z"));

        var json = JsonSerializer.Serialize(response);

        Assert.Contains(@"""display_name"":""Player Two""", json);
    }
}
