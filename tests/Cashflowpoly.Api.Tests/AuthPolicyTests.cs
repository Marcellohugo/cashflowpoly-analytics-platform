// Fungsi file: Memastikan batas bcrypt dan kebijakan registrasi publik tidak dapat dilonggarkan tanpa sengaja.
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AuthPolicyTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthPolicyTests
// Membuka scope tipe AuthPolicyTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors` dengan hasil bertipe `void`; operasi ini menangani
    // production policies reject oversized passwords dan public instructors.
    public void ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors()
    // Membuka scope metode ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
    {
        // Menjalankan pemeriksaan bahwa `PasswordPolicy.IsWithinBcryptLimit(new string('a', 72))` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
        Assert.True(PasswordPolicy.IsWithinBcryptLimit(new string('a', 72)));
        // Menjalankan pemeriksaan bahwa `PasswordPolicy.IsWithinBcryptLimit(new string('a', 73))` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
        Assert.False(PasswordPolicy.IsWithinBcryptLimit(new string('a', 73)));
        // Menjalankan pemeriksaan bahwa `PasswordPolicy.IsWithinBcryptLimit(string.Concat(Enumerable.Repeat(”😀”, 19)))` bernilai salah; pengujian gagal
        // jika kondisi justru terpenuhi dalam ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
        Assert.False(PasswordPolicy.IsWithinBcryptLimit(string.Concat(Enumerable.Repeat("😀", 19))));

        // Menyiapkan variabel lokal `registration` untuk nilai registration dengan objek baru bertipe `AuthRegistrationOptions` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var registration = new AuthRegistrationOptions();
        // Menjalankan pemeriksaan bahwa `registration.CanRegisterPublicly(”PLAYER”)` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
        Assert.True(registration.CanRegisterPublicly("PLAYER"));
        // Menjalankan pemeriksaan bahwa `registration.CanRegisterPublicly(”INSTRUCTOR”)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
        Assert.False(registration.CanRegisterPublicly("INSTRUCTOR"));
    // Menutup scope metode ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors; bagian berikut berada di luar batas blok tersebut dalam
    // ProductionPolicies_RejectOversizedPasswordsAndPublicInstructors.
    }
// Menutup scope tipe AuthPolicyTests; bagian berikut berada di luar batas blok tersebut.
}
