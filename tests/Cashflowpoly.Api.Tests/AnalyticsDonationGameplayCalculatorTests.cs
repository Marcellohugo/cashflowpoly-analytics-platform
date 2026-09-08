// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsDonationGameplayCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsDonationGameplayCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsDonationGameplayCalculatorTests
// Membuka scope tipe AnalyticsDonationGameplayCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_GroupsDonationAmountAndRankByFriday` dengan hasil bertipe `void`; operasi ini menangani compute groups donasi
    // nominal dan rank berdasarkan friday.
    public void Compute_GroupsDonationAmountAndRankByFriday()
    // Membuka scope metode Compute_GroupsDonationAmountAndRankByFriday; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_GroupsDonationAmountAndRankByFriday.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_GroupsDonationAmountAndRankByFriday.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”JumatBerkah”`, `”””{”amount”:10}”””`, `5`, `”FRI”` dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            CreateEvent(playerId, "JumatBerkah", """{"amount":10}""", dayIndex: 5, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”JumatBerkah”`, `”””{”amount”:20}”””`, `5`, `”FRI”` dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            CreateEvent(playerId, "JumatBerkah", """{"amount":20}""", dayIndex: 5, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”JumatBerkah”`, `”””{”amount”:5}”””`, `12`, `”FRI”` dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            CreateEvent(playerId, "JumatBerkah", """{"amount":5}""", dayIndex: 12, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”PoinPeringkatDonasi”`, `”””{”rank”:1,”points”:15}”””`, `5`, `”FRI”`
            // dalam Compute_GroupsDonationAmountAndRankByFriday.
            CreateEvent(playerId, "PoinPeringkatDonasi", """{"rank":1,"points":15}""", dayIndex: 5, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”PoinPeringkatDonasi”`, `”””{”rank”:2,”points”:10}”””`, `12`, `”FRI”`
            // dalam Compute_GroupsDonationAmountAndRankByFriday.
            CreateEvent(playerId, "PoinPeringkatDonasi", """{"rank":2,"points":10}""", dayIndex: 12, weekday: "FRI")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_GroupsDonationAmountAndRankByFriday.
        };
        // Menyiapkan variabel lokal `allEvents` untuk nilai all event dengan mematerialisasi urutan `playerEvents.Concat(new[] {
        // CreateEvent(Guid.NewGuid(), ”AkhirGiliran”, ”{}”, dayIndex: 19, weekday: ”FRI”) })` menjadi List; enumerasi dijalankan dan hasilnya disimpan
        // dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var allEvents = playerEvents.Concat(new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_GroupsDonationAmountAndRankByFriday.
        {
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”AkhirGiliran”` sebagai
            // argumen ke `CreateEvent`; Meneruskan nilai literal `”{}”` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `19` sebagai argumen bernama
            // `dayIndex`; Meneruskan nilai literal `”FRI”` sebagai argumen bernama `weekday`.
            CreateEvent(Guid.NewGuid(), "AkhirGiliran", "{}", dayIndex: 19, weekday: "FRI")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_GroupsDonationAmountAndRankByFriday.
        }).ToList();

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new DonationGameplayCalculator().Compute` dengan `playerEvents`,
        // `allEvents`, `100`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new DonationGameplayCalculator().Compute(playerEvents, allEvents, coinsNetEndGame: 100);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`35`, `metrics.DonationTotalCoins`); pengujian
        // gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(35, metrics.DonationTotalCoins);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.DonationChampionCardsEarned`);
        // pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(2, metrics.DonationChampionCardsEarned);
        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `metrics.DonationAmountPerFriday`, `item => { Assert.Equal(5,
        // item.DayIndex); Assert.Equal(30, item.Amount); }`, `item => { Assert.Equal(12, item.DayIndex); Assert.Equal(5, item.Amount); }`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Collection(metrics.DonationAmountPerFriday,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(5, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`30`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(30, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(12, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(5, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            });
        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `metrics.DonationRankPerFriday`, `item => { Assert.Equal(5, item.DayIndex);
        // Assert.Equal(1, item.Rank); }`, `item => { Assert.Equal(12, item.DayIndex); Assert.Equal(2, item.Rank); }`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Collection(metrics.DonationRankPerFriday,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(5, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.Rank`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(1, item.Rank);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(12, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `item.Rank`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
                Assert.Equal(2, item.Rank);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsDonationAmountAndRankByFriday.
            });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12.5`,
        // `metrics.DonationStabilityStdDeviation`); pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(12.5, metrics.DonationStabilityStdDeviation);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`87.5`, `metrics.DonationStability!.Value`,
        // `12`); pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(87.5, metrics.DonationStability!.Value, precision: 12);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`28.57142857142857`,
        // `metrics.DonationStabilityIndex!.Value`, `12`); pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(28.57142857142857, metrics.DonationStabilityIndex!.Value, precision: 12);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.35`, `metrics.DonationRatio`); pengujian
        // gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(0.35, metrics.DonationRatio);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`35`, `metrics.DonationAggressivenessPercent`);
        // pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(35, metrics.DonationAggressivenessPercent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2d / 3d`, `metrics.FridayParticipationRate`);
        // pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(2d / 3d, metrics.FridayParticipationRate);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6.666666666666666`,
        // `metrics.DonationCommitmentScore!.Value`, `12`); pengujian gagal jika keduanya berbeda dalam Compute_GroupsDonationAmountAndRankByFriday.
        Assert.Equal(6.666666666666666, metrics.DonationCommitmentScore!.Value, precision: 12);
    // Menutup scope metode Compute_GroupsDonationAmountAndRankByFriday; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_GroupsDonationAmountAndRankByFriday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday` dengan hasil bertipe `void`; operasi ini menangani compute
    // returns null derived metrics when no donasi dan no friday.
    public void Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday()
    // Membuka scope metode Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
    {
        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new DonationGameplayCalculator().Compute` dengan
        // `Array.Empty<EventDb>()`, `Array.Empty<EventDb>()`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new DonationGameplayCalculator().Compute(
            // Meneruskan memanggil `Array.Empty<EventDb>` dengan tanpa argumen sebagai argumen ke `new DonationGameplayCalculator().Compute`.
            Array.Empty<EventDb>(),
            // Meneruskan memanggil `Array.Empty<EventDb>` dengan tanpa argumen sebagai argumen ke `new DonationGameplayCalculator().Compute`.
            Array.Empty<EventDb>(),
            // Meneruskan nilai literal `0` sebagai argumen bernama `coinsNetEndGame`.
            coinsNetEndGame: 0);

        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `metrics.DonationAmountPerFriday`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Empty(metrics.DonationAmountPerFriday);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `metrics.DonationRankPerFriday`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Empty(metrics.DonationRankPerFriday);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.DonationTotalCoins`); pengujian
        // gagal jika keduanya berbeda dalam Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Equal(0, metrics.DonationTotalCoins);
        // Menjalankan pemeriksaan Null atas `metrics.DonationStabilityStdDeviation` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Null(metrics.DonationStabilityStdDeviation);
        // Menjalankan pemeriksaan Null atas `metrics.DonationRatio` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Null(metrics.DonationRatio);
        // Menjalankan pemeriksaan Null atas `metrics.DonationAggressivenessPercent` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Null(metrics.DonationAggressivenessPercent);
        // Menjalankan pemeriksaan Null atas `metrics.FridayParticipationRate` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Null(metrics.FridayParticipationRate);
        // Menjalankan pemeriksaan Null atas `metrics.DonationCommitmentScore` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
        Assert.Null(metrics.DonationCommitmentScore);
    // Menutup scope metode Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker` dengan hasil bertipe `void`; operasi ini menangani compute
    // ranks every friday using donasi then highest tie breaker.
    public void Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker()
    // Membuka scope metode Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `secondPlayerId` untuk nilai second pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var secondPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `thirdPlayerId` untuk nilai third pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var thirdPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”JumatBerkah”`, `”””{”amount”:1}”””`, `5`, `”FRI”` dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            CreateEvent(playerId, "JumatBerkah", """{"amount":1}""", dayIndex: 5, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”JumatBerkah”`, `”””{”amount”:4}”””`, `12`, `”FRI”` dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            CreateEvent(playerId, "JumatBerkah", """{"amount":4}""", dayIndex: 12, weekday: "FRI")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
        };
        // Menyiapkan variabel lokal `allEvents` untuk nilai all event dengan mematerialisasi urutan `playerEvents.Concat(new[] {
        // CreateEvent(secondPlayerId, ”JumatBerkah”, ”””{”amount”:3}”””, dayIndex: 5, weekday: ”FRI”), CreateEvent(thirdPlayerId, ”JumatBerkah”,
        // ”””{”amount”...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var allEvents = playerEvents.Concat(new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
        {
            // Meneruskan `secondPlayerId` (nilai second pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”JumatBerkah”` sebagai
            // argumen ke `CreateEvent`; Meneruskan nilai literal `”””{”amount”:3}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `5` sebagai
            // argumen bernama `dayIndex`; Meneruskan nilai literal `”FRI”` sebagai argumen bernama `weekday`.
            CreateEvent(secondPlayerId, "JumatBerkah", """{"amount":3}""", dayIndex: 5, weekday: "FRI"),
            // Meneruskan `thirdPlayerId` (nilai third pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”JumatBerkah”` sebagai
            // argumen ke `CreateEvent`; Meneruskan nilai literal `”””{”amount”:3}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `5` sebagai
            // argumen bernama `dayIndex`; Meneruskan nilai literal `”FRI”` sebagai argumen bernama `weekday`.
            CreateEvent(thirdPlayerId, "JumatBerkah", """{"amount":3}""", dayIndex: 5, weekday: "FRI"),
            // Meneruskan `secondPlayerId` (nilai second pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”JumatBerkah”` sebagai
            // argumen ke `CreateEvent`; Meneruskan nilai literal `”””{”amount”:4}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `12` sebagai
            // argumen bernama `dayIndex`; Meneruskan nilai literal `”FRI”` sebagai argumen bernama `weekday`.
            CreateEvent(secondPlayerId, "JumatBerkah", """{"amount":4}""", dayIndex: 12, weekday: "FRI"),
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”BagikanTieBreaker”` sebagai argumen
            // ke `CreateEvent`; Meneruskan nilai literal `”””{”number”:4}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `0` sebagai argumen
            // bernama `dayIndex`; Meneruskan nilai literal `”SUN”` sebagai argumen bernama `weekday`.
            CreateEvent(playerId, "BagikanTieBreaker", """{"number":4}""", dayIndex: 0, weekday: "SUN"),
            // Meneruskan `secondPlayerId` (nilai second pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”BagikanTieBreaker”`
            // sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”””{”number”:2}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `0`
            // sebagai argumen bernama `dayIndex`; Meneruskan nilai literal `”SUN”` sebagai argumen bernama `weekday`.
            CreateEvent(secondPlayerId, "BagikanTieBreaker", """{"number":2}""", dayIndex: 0, weekday: "SUN"),
            // Meneruskan `thirdPlayerId` (nilai third pemain identitas) sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”BagikanTieBreaker”`
            // sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `”””{”number”:3}”””` sebagai argumen ke `CreateEvent`; Meneruskan nilai literal `0`
            // sebagai argumen bernama `dayIndex`; Meneruskan nilai literal `”SUN”` sebagai argumen bernama `weekday`.
            CreateEvent(thirdPlayerId, "BagikanTieBreaker", """{"number":3}""", dayIndex: 0, weekday: "SUN")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
        }).ToList();

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new DonationGameplayCalculator().Compute` dengan `playerEvents`,
        // `allEvents`, `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new DonationGameplayCalculator().Compute(playerEvents, allEvents, coinsNetEndGame: 20);

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `metrics.DonationRankPerFriday`, `item => { Assert.Equal(5, item.DayIndex);
        // Assert.Equal(3, item.Rank); }`, `item => { Assert.Equal(12, item.DayIndex); Assert.Equal(1, item.Rank); }`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
        Assert.Collection(metrics.DonationRankPerFriday,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
                Assert.Equal(5, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `item.Rank`); pengujian gagal jika
                // keduanya berbeda dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
                Assert.Equal(3, item.Rank);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
                Assert.Equal(12, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.Rank`); pengujian gagal jika
                // keduanya berbeda dalam Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
                Assert.Equal(1, item.Rank);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
            });
    // Menutup scope metode Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON; Parameter `dayIndex` bertipe `int` membawa nilai hari index; Parameter `weekday` bertipe `string`
    // membawa nilai weekday.
    private static EventDb CreateEvent(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
        string payload,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
        int dayIndex,
        // Parameter `weekday` bertipe `string` membawa nilai weekday.
        string weekday)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            SessionId = Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan `dayIndex` (nilai hari index) dalam CreateEvent.
            DayIndex = dayIndex,
            // Memperbarui `Weekday` menggunakan `weekday` (nilai weekday) dalam CreateEvent.
            Weekday = weekday,
            // Memperbarui `ActionSlot` menggunakan `dayIndex` (nilai hari index) dalam CreateEvent.
            ActionSlot = dayIndex,
            // Memperbarui `SequenceNumber` menggunakan `dayIndex` (nilai hari index) dalam CreateEvent.
            SequenceNumber = dayIndex,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }
// Menutup scope tipe AnalyticsDonationGameplayCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
