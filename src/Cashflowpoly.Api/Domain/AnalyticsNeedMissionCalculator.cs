// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsNeedMissionCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsNeedMissionMetrics`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record AnalyticsNeedMissionMetrics(
    // Parameter `NeedCardsPurchased` bertipe `int` membawa nilai kebutuhan kartu dibeli.
    int NeedCardsPurchased,
    // Parameter `NeedCardsOwnedCurrent` bertipe `int` membawa nilai kebutuhan kartu dimiliki saat ini.
    int NeedCardsOwnedCurrent,
    // Parameter `PrimaryNeeds` bertipe `int` membawa nilai primary kebutuhan.
    int PrimaryNeeds,
    // Parameter `SecondaryNeeds` bertipe `int` membawa nilai secondary kebutuhan.
    int SecondaryNeeds,
    // Parameter `TertiaryNeeds` bertipe `int` membawa nilai tertiary kebutuhan.
    int TertiaryNeeds,
    // Parameter `HasBasicNeedProfile` bertipe `bool` membawa nilai memiliki basic kebutuhan profile.
    bool HasBasicNeedProfile,
    // Parameter `IsCollectorNeedProfile` bertipe `bool` membawa nilai berstatus collector kebutuhan profile.
    bool IsCollectorNeedProfile,
    // Parameter `IsSpecialistNeedProfile` bertipe `bool` membawa nilai berstatus specialist kebutuhan profile.
    bool IsSpecialistNeedProfile,
    // Parameter `SpecificTertiaryAcquired` bertipe `bool?` membawa nilai specific tertiary acquired; nilai null diizinkan ketika data opsional belum
    // tersedia.
    bool? SpecificTertiaryAcquired,
    // Parameter `CollectionMissionComplete` bertipe `bool?` membawa nilai collection misi complete; nilai null diizinkan ketika data opsional belum
    // tersedia.
    bool? CollectionMissionComplete,
    // Parameter `NeedCoinsSpent` bertipe `int` membawa nilai kebutuhan coins spent.
    int NeedCoinsSpent,
    // Parameter `FulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; nilai null diizinkan
    // ketika data opsional belum tersedia.
    double? FulfillmentDiversity,
    // Parameter `FulfillmentDiversityDocumentFormula` bertipe `double?` membawa nilai pemenuhan keberagaman document formula; nilai null diizinkan
    // ketika data opsional belum tersedia.
    double? FulfillmentDiversityDocumentFormula,
    // Parameter `MissionAchievement` bertipe `int?` membawa nilai misi achievement; nilai null diizinkan ketika data opsional belum tersedia.
    int? MissionAchievement);

// Mendefinisikan tipe class `NeedMissionCalculator` yang mewarisi atau menerapkan `INeedMissionCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class NeedMissionCalculator : INeedMissionCalculator
// Membuka scope tipe NeedMissionCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsNeedMissionMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
    public AnalyticsNeedMissionMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
        IEnumerable<CashflowProjectionDb> playerProjections)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `activeNeeds` untuk nilai aktif kebutuhan dengan objek baru bertipe `List<NeedCard>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeNeeds = new List<NeedCard>();
        // Menyiapkan variabel lokal `purchasedNeeds` untuk nilai dibeli kebutuhan dengan objek baru bertipe `List<NeedCard>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchasedNeeds = new List<NeedCard>();
        // Menyiapkan variabel lokal `missions` untuk nilai misi dengan objek baru bertipe `List<MissionAssignment>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missions = new List<MissionAssignment>();
        // Menyiapkan variabel lokal `needCardsPurchased` untuk nilai kebutuhan kartu dibeli dengan nilai literal `0`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var needCardsPurchased = 0;

        // Mengulangi setiap elemen `playerEvents.OrderBy(e => e.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam Compute.
        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        // Membuka scope loop setiap evt dari `playerEvents.OrderBy(e => e.SequenceNumber)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Compute.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”Kebutuhan”` dan
            // `_payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == "Kebutuhan" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadNeedPurchase` dengan `evt.Payload`, `_`, `var cardId`, `_` dalam Compute.
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”Kebutuhan” && _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId,
            // out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menyiapkan variabel lokal `purchasedNeed` untuk nilai dibeli kebutuhan dengan objek baru bertipe `NeedCard` dengan argumen (cardId,
                // NeedTierClassifier.FromPayloadJson(evt.Payload)). Tipe variabel disimpulkan dari ekspresi nilai awal.
                var purchasedNeed = new NeedCard(cardId, NeedTierClassifier.FromPayloadJson(evt.Payload));
                // Menjalankan menambahkan `purchasedNeed` ke `activeNeeds` dalam Compute.
                activeNeeds.Add(purchasedNeed);
                // Menjalankan menambahkan `purchasedNeed` ke `purchasedNeeds` dalam Compute.
                purchasedNeeds.Add(purchasedNeed);
                // Menjalankan `needCardsPurchased++` dalam Compute.
                needCardsPurchased++;
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”Kebutuhan” && _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId,
            // out _)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”GunakanOpsiDarurat”` dan
            // `_payloadReader.TryReadSoldNeed(evt.Payload, out var soldNeedCardId)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == "GunakanOpsiDarurat" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSoldNeed` dengan `evt.Payload`, `var soldNeedCardId` dalam Compute.
                _payloadReader.TryReadSoldNeed(evt.Payload, out var soldNeedCardId))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”GunakanOpsiDarurat” && _payloadReader.TryReadSoldNeed(evt.Payload, out var
            // soldNeedCardId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menyiapkan variabel lokal `soldIndex` untuk nilai terjual index dengan memanggil `activeNeeds.FindIndex` dengan `need =>
                // string.Equals(need.CardId, soldNeedCardId, StringComparison.OrdinalIgnoreCase)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var soldIndex = activeNeeds.FindIndex(need =>
                    // Meneruskan `need.CardId` (nilai kartu identitas) sebagai argumen ke `string.Equals`; Meneruskan `soldNeedCardId` (nilai terjual kebutuhan kartu
                    // identitas) sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
                    // `string.Equals`.
                    string.Equals(need.CardId, soldNeedCardId, StringComparison.OrdinalIgnoreCase));
                // Memeriksa pemeriksaan lebih besar atau sama antara `soldIndex` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
                if (soldIndex >= 0)
                // Membuka scope cabang if untuk kondisi `soldIndex >= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Menjalankan menghapus elemen dari `activeNeeds` berdasarkan `soldIndex` dalam Compute.
                    activeNeeds.RemoveAt(soldIndex);
                // Menutup scope cabang if untuk kondisi `soldIndex >= 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
                }
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”GunakanOpsiDarurat” && _payloadReader.TryReadSoldNeed(evt.Payload, out var
            // soldNeedCardId)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(evt.ActionType, GameActionCatalog.SetupMisiAwal,
            // StringComparison.OrdinalIgnoreCase)` dan `_payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var
            // penaltyPoints, out var requirePrimary, out var requireSecondary)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam Compute.
            if (string.Equals(evt.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadMissionAssigned` dengan `evt.Payload`, `var missionId`, `var targetCardId`, `var
                // penaltyPoints`, `var requirePrimary`, `var requireSecondary` dalam Compute.
                _payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            // Membuka scope cabang if untuk kondisi `string.Equals(evt.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
            // _payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute.
            {
                // Menjalankan menambahkan `new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary)` ke `missions` dalam
                // Compute.
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            // Menutup scope cabang if untuk kondisi `string.Equals(evt.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
            // _payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out...`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope loop setiap evt dari `playerEvents.OrderBy(e => e.SequenceNumber)`; bagian berikut berada di luar batas blok tersebut dalam
        // Compute.
        }

        // Menyiapkan variabel lokal `primaryNeeds` untuk nilai primary kebutuhan dengan memanggil `activeNeeds.Count` dengan `need => need.Tier ==
        // NeedTier.Primary`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var primaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Primary);
        // Menyiapkan variabel lokal `secondaryNeeds` untuk nilai secondary kebutuhan dengan memanggil `activeNeeds.Count` dengan `need => need.Tier ==
        // NeedTier.Secondary`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Secondary);
        // Menyiapkan variabel lokal `tertiaryNeeds` untuk nilai tertiary kebutuhan dengan memanggil `activeNeeds.Count` dengan `need => need.Tier ==
        // NeedTier.Tertiary`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tertiaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Tertiary);
        // Menyiapkan variabel lokal `distinctNeedCardIds` untuk nilai distinct kebutuhan kartu identitas dengan membentuk himpunan nilai unik dari
        // `activeNeeds .Select(need => need.CardId) .Where(cardId => !string.IsNullOrWhiteSpace(cardId))` memakai `StringComparer.OrdinalIgnoreCase`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var distinctNeedCardIds = activeNeeds
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(need => need.CardId) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Select(need => need.CardId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(cardId => !string.IsNullOrWhiteSpace(cardId)) dalam Compute; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(cardId => !string.IsNullOrWhiteSpace(cardId))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam Compute; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `purchasedPrimaryNeeds` untuk nilai dibeli primary kebutuhan dengan memanggil `purchasedNeeds.Count` dengan `need =>
        // need.Tier == NeedTier.Primary`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchasedPrimaryNeeds = purchasedNeeds.Count(need => need.Tier == NeedTier.Primary);
        // Menyiapkan variabel lokal `purchasedSecondaryNeeds` untuk nilai dibeli secondary kebutuhan dengan memanggil `purchasedNeeds.Count` dengan `need
        // => need.Tier == NeedTier.Secondary`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchasedSecondaryNeeds = purchasedNeeds.Count(need => need.Tier == NeedTier.Secondary);
        // Menyiapkan variabel lokal `purchasedTertiaryCardIds` untuk nilai dibeli tertiary kartu identitas dengan membentuk himpunan nilai unik dari
        // `purchasedNeeds .Where(need => need.Tier == NeedTier.Tertiary) .Select(need => System.Text.RegularExpressions.Regex.Replace(need.CardId,
        // ”_[0-9]+$”, ””)) .Where(cardId => !stri...` memakai `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchasedTertiaryCardIds = purchasedNeeds
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(need => need.Tier == NeedTier.Tertiary) dalam Compute; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(need => need.Tier == NeedTier.Tertiary)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(need => System.Text.RegularExpressions.Regex.Replace(need.CardId,
            // ”_[0-9]+$”, ””)) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(need => System.Text.RegularExpressions.Regex.Replace(need.CardId, "_[0-9]+$", ""))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(cardId => !string.IsNullOrWhiteSpace(cardId)) dalam Compute; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(cardId => !string.IsNullOrWhiteSpace(cardId))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam Compute; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `needCardsOwnedCurrent` untuk nilai kebutuhan kartu dimiliki saat ini dengan penjumlahan/penggabungan antara
        // `primaryNeeds + secondaryNeeds` dan `tertiaryNeeds`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needCardsOwnedCurrent = primaryNeeds + secondaryNeeds + tertiaryNeeds;
        // Menyiapkan variabel lokal `hasBasicNeedProfile` untuk nilai memiliki basic kebutuhan profile dengan gabungan syarat AND: kedua kondisi wajib
        // benar antara `primaryNeeds > 0 && secondaryNeeds > 0` dan `tertiaryNeeds > 0`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var hasBasicNeedProfile = primaryNeeds > 0 && secondaryNeeds > 0 && tertiaryNeeds > 0;
        // Menyiapkan variabel lokal `isCollectorNeedProfile` untuk nilai berstatus collector kebutuhan profile dengan pemeriksaan lebih besar atau sama
        // antara `distinctNeedCardIds.Count` dan `4`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isCollectorNeedProfile = distinctNeedCardIds.Count >= 4;
        // Menyiapkan variabel lokal `dominantNeedCount` untuk nilai dominant kebutuhan jumlah dengan menentukan nilai terbesar dari `primaryNeeds`,
        // `Math.Max(secondaryNeeds, tertiaryNeeds)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var dominantNeedCount = Math.Max(primaryNeeds, Math.Max(secondaryNeeds, tertiaryNeeds));
        // Menyiapkan variabel lokal `isSpecialistNeedProfile` untuk nilai berstatus specialist kebutuhan profile dengan gabungan syarat AND: kedua kondisi
        // wajib benar antara `needCardsOwnedCurrent > 0` dan `((double)dominantNeedCount / needCardsOwnedCurrent) >= 0.7`; sisi kanan diperiksa hanya jika
        // sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isSpecialistNeedProfile = needCardsOwnedCurrent > 0 && ((double)dominantNeedCount / needCardsOwnedCurrent) >= 0.7;
        // Menyiapkan variabel lokal `needCoinsSpent` untuk nilai kebutuhan coins spent dengan menjumlahkan nilai `playerProjections .Where(p => p.Direction
        // == ”OUT” && (p.Category == ”NEED_PRIMARY” || p.Category == ”NEED_SECONDARY” || p.Category == ”NEED_TERTIARY”))` berdasarkan `p => p.Amount`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var needCoinsSpent = playerProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.Direction == ”OUT” && dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(p => p.Direction == "OUT" &&
                        // Meneruskan fungsi lambda `p => p.Direction == ”OUT” && (p.Category == ”NEED_PRIMARY” || p.Category == ”NEED_SECONDARY” || p.Category ==
                        // ”NEED_TERTIARY”)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `playerProjections .Where`.
                        (p.Category == "NEED_PRIMARY" || p.Category == "NEED_SECONDARY" || p.Category == "NEED_TERTIARY"))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(p => p.Amount); dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Sum(p => p.Amount);

        // Menyiapkan variabel lokal `specificTertiaryAcquired` untuk nilai specific tertiary acquired dengan null, yaitu penanda tidak ada nilai. Tipe yang
        // dipakai adalah `bool?`.
        bool? specificTertiaryAcquired = null;
        // Menyiapkan variabel lokal `collectionMissionComplete` untuk nilai collection misi complete dengan null, yaitu penanda tidak ada nilai. Tipe yang
        // dipakai adalah `bool?`.
        bool? collectionMissionComplete = null;
        // Memeriksa pemeriksaan lebih besar antara `missions.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
        if (missions.Count > 0)
        // Membuka scope cabang if untuk kondisi `missions.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Memperbarui `specificTertiaryAcquired` menggunakan memeriksa apakah `missions` memiliki setidaknya satu elemen yang memenuhi `m =>
            // !string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) && purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId)` dalam Compute.
            specificTertiaryAcquired = missions.Any(m => !string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) &&
                                                        // Meneruskan `m.TargetTertiaryCardId` (nilai target tertiary kartu identitas) sebagai argumen ke `purchasedTertiaryCardIds.Contains`.
                                                        purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId));

            // Menyiapkan variabel lokal `hasPrimary` untuk nilai memiliki primary dengan pemeriksaan lebih besar antara `purchasedPrimaryNeeds` dan `0`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var hasPrimary = purchasedPrimaryNeeds > 0;
            // Menyiapkan variabel lokal `hasSecondary` untuk nilai memiliki secondary dengan pemeriksaan lebih besar antara `purchasedSecondaryNeeds` dan `0`.
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var hasSecondary = purchasedSecondaryNeeds > 0;
            // Memperbarui `collectionMissionComplete` menggunakan memeriksa apakah seluruh elemen `missions` memenuhi `m => { var hasTarget =
            // string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) || purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId); var requirePrimary =
            // !m.RequirePrimary |...`; koleksi kosong menghasilkan true dalam Compute.
            collectionMissionComplete = missions.All(m =>
            // Membuka scope fungsi lambda yang dipasok ke `missions.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menyiapkan variabel lokal `hasTarget` untuk nilai memiliki target dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
                // `string.IsNullOrWhiteSpace(m.TargetTertiaryCardId)` dan `purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId)`; sisi kanan diperiksa hanya
                // jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var hasTarget = string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) ||
                                // Meneruskan `m.TargetTertiaryCardId` (nilai target tertiary kartu identitas) sebagai argumen ke `purchasedTertiaryCardIds.Contains`.
                                purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId);
                // Menyiapkan variabel lokal `requirePrimary` untuk nilai require primary dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
                // `!m.RequirePrimary` dan `hasPrimary`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var requirePrimary = !m.RequirePrimary || hasPrimary;
                // Menyiapkan variabel lokal `requireSecondary` untuk nilai require secondary dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
                // `!m.RequireSecondary` dan `hasSecondary`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var requireSecondary = !m.RequireSecondary || hasSecondary;
                // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `hasTarget && requirePrimary` dan `requireSecondary`; sisi kanan diperiksa
                // hanya jika sisi kiri benar kepada pemanggil dalam Compute; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return hasTarget && requirePrimary && requireSecondary;
            // Menutup scope fungsi lambda yang dipasok ke `missions.All`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            });
        // Menutup scope cabang if untuk kondisi `missions.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `pPrimary` untuk nilai p primary dengan memanggil `SafeRatio` dengan `primaryNeeds`, `needCardsOwnedCurrent`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var pPrimary = SafeRatio(primaryNeeds, needCardsOwnedCurrent);
        // Menyiapkan variabel lokal `pSecondary` untuk nilai p secondary dengan memanggil `SafeRatio` dengan `secondaryNeeds`, `needCardsOwnedCurrent`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pSecondary = SafeRatio(secondaryNeeds, needCardsOwnedCurrent);
        // Menyiapkan variabel lokal `pTertiary` untuk nilai p tertiary dengan memanggil `SafeRatio` dengan `tertiaryNeeds`, `needCardsOwnedCurrent`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var pTertiary = SafeRatio(tertiaryNeeds, needCardsOwnedCurrent);
        // Menyiapkan variabel lokal `fulfillmentDiversity` untuk tingkat keberagaman kategori kebutuhan yang telah dipenuhi dengan hasil pemilihan
        // bersyarat: ketika `pPrimary.HasValue && pSecondary.HasValue && pTertiary.HasValue` benar gunakan `(1 - (Math.Pow(pPrimary.Value, 2) +
        // Math.Pow(pSecondary.Value, 2) + Math.Pow(pTertiary.Value, 2))) / (1 - (1d / 3))`, jika tidak gunakan `(double?)null`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var fulfillmentDiversity =
            // Melanjutkan ekspresi dengan gabungan syarat AND: kedua kondisi wajib benar antara `pPrimary.HasValue` dan `pSecondary.HasValue`; sisi kanan
            // diperiksa hanya jika sisi kiri benar dalam Compute.
            pPrimary.HasValue && pSecondary.HasValue && pTertiary.HasValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) +
                // Math.Pow(pTertiary.Value, 2))) dalam Compute.
                ? (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) + Math.Pow(pTertiary.Value, 2)))
                    // Melengkapi struktur ekspresi DivideExpression melalui / (1 - (1d / 3)) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan
                    // sesudahnya.
                    / (1 - (1d / 3))
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
                : (double?)null;
        // Menyiapkan variabel lokal `fulfillmentDiversityDocumentFormula` untuk nilai pemenuhan keberagaman document formula dengan hasil pemilihan
        // bersyarat: ketika `needCardsOwnedCurrent > 0` benar gunakan `Math.Sqrt( Math.Pow(primaryNeeds, 2) + Math.Pow(secondaryNeeds, 2) +
        // Math.Pow(tertiaryNeeds, 2)) / needCardsOwnedCurrent`, jika tidak gunakan `(double?)null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var fulfillmentDiversityDocumentFormula = needCardsOwnedCurrent > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Math.Sqrt( dalam Compute.
            ? Math.Sqrt(
                // Meneruskan penjumlahan/penggabungan antara `Math.Pow(primaryNeeds, 2) + Math.Pow(secondaryNeeds, 2)` dan `Math.Pow(tertiaryNeeds, 2)` sebagai
                // argumen ke `Math.Sqrt`; Meneruskan `primaryNeeds` (nilai primary kebutuhan) sebagai argumen ke `Math.Pow`; Meneruskan nilai literal `2` sebagai
                // argumen ke `Math.Pow`.
                Math.Pow(primaryNeeds, 2) +
                // Meneruskan `secondaryNeeds` (nilai secondary kebutuhan) sebagai argumen ke `Math.Pow`; Meneruskan nilai literal `2` sebagai argumen ke
                // `Math.Pow`.
                Math.Pow(secondaryNeeds, 2) +
                // Meneruskan `tertiaryNeeds` (nilai tertiary kebutuhan) sebagai argumen ke `Math.Pow`; Meneruskan nilai literal `2` sebagai argumen ke `Math.Pow`.
                Math.Pow(tertiaryNeeds, 2)) / needCardsOwnedCurrent
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
            : (double?)null;
        // Menyiapkan variabel lokal `missionAchievement` untuk nilai misi achievement dengan hasil pemilihan bersyarat: ketika
        // `collectionMissionComplete.HasValue` benar gunakan `(collectionMissionComplete.Value ? 1 : 0)`, jika tidak gunakan `(int?)null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var missionAchievement = collectionMissionComplete.HasValue
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: (collectionMissionComplete.Value ? 1 : 0) dalam Compute.
            ? (collectionMissionComplete.Value ? 1 : 0)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (int?)null; dalam Compute.
            : (int?)null;

        // Mengembalikan objek baru bertipe `AnalyticsNeedMissionMetrics` dengan argumen ( needCardsPurchased, needCardsOwnedCurrent, primaryNeeds,
        // secondaryNeeds, tertiaryNeeds, hasBasicNeedProfile, isCollectorNeedProfile, isSpecialistNeedProfile, ... kepada pemanggil dalam Compute; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsNeedMissionMetrics(
            // Meneruskan `needCardsPurchased` (nilai kebutuhan kartu dibeli) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            needCardsPurchased,
            // Meneruskan `needCardsOwnedCurrent` (nilai kebutuhan kartu dimiliki saat ini) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            needCardsOwnedCurrent,
            // Meneruskan `primaryNeeds` (nilai primary kebutuhan) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            primaryNeeds,
            // Meneruskan `secondaryNeeds` (nilai secondary kebutuhan) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            secondaryNeeds,
            // Meneruskan `tertiaryNeeds` (nilai tertiary kebutuhan) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            tertiaryNeeds,
            // Meneruskan `hasBasicNeedProfile` (nilai memiliki basic kebutuhan profile) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            hasBasicNeedProfile,
            // Meneruskan `isCollectorNeedProfile` (nilai berstatus collector kebutuhan profile) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            isCollectorNeedProfile,
            // Meneruskan `isSpecialistNeedProfile` (nilai berstatus specialist kebutuhan profile) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            isSpecialistNeedProfile,
            // Meneruskan `specificTertiaryAcquired` (nilai specific tertiary acquired) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            specificTertiaryAcquired,
            // Meneruskan `collectionMissionComplete` (nilai collection misi complete) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            collectionMissionComplete,
            // Meneruskan `needCoinsSpent` (nilai kebutuhan coins spent) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            needCoinsSpent,
            // Meneruskan `fulfillmentDiversity` (tingkat keberagaman kategori kebutuhan yang telah dipenuhi) sebagai argumen ke konstruktor
            // `AnalyticsNeedMissionMetrics`.
            fulfillmentDiversity,
            // Meneruskan `fulfillmentDiversityDocumentFormula` (nilai pemenuhan keberagaman document formula) sebagai argumen ke konstruktor
            // `AnalyticsNeedMissionMetrics`.
            fulfillmentDiversityDocumentFormula,
            // Meneruskan `missionAchievement` (nilai misi achievement) sebagai argumen ke konstruktor `AnalyticsNeedMissionMetrics`.
            missionAchievement);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `MissionAssignment`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record MissionAssignment(
        // Parameter `MissionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan.
        string MissionId,
        // Parameter `TargetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas.
        string TargetTertiaryCardId,
        // Parameter `PenaltyPoints` bertipe `int` membawa nilai penalti poin.
        int PenaltyPoints,
        // Parameter `RequirePrimary` bertipe `bool` membawa nilai require primary.
        bool RequirePrimary,
        // Parameter `RequireSecondary` bertipe `bool` membawa nilai require secondary.
        bool RequireSecondary);

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `NeedCard`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record NeedCard(string CardId, NeedTier Tier);
// Menutup scope tipe NeedMissionCalculator; bagian berikut berada di luar batas blok tersebut.
}
