// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui SessionTimelineMapper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis untuk memetakan data event transaksi gameplay menjadi
/// deskripsi timeline yang mudah dibaca pengguna dalam dua bahasa (Indonesia/Inggris).
/// </summary>
// Mendefinisikan tipe class `SessionTimelineMapper`.
public static class SessionTimelineMapper
{
    /// <summary>
    /// Mengonversi daftar event mentah menjadi item timeline terurut berdasarkan
    /// waktu dan nomor urut, dengan label dan deskripsi bilingual.
    /// </summary>
    /// <param name="events">Daftar event dari API, boleh null.</param>
    /// <param name="language">Kode bahasa ("id" atau "en"); default bahasa Indonesia.</param>
    /// <returns>Daftar item timeline yang siap ditampilkan di UI.</returns>
    // Mendefinisikan metode `MapTimeline` dengan hasil bertipe `List<SessionTimelineEventViewModel>`. Mengonversi daftar event mentah menjadi item
    // timeline terurut berdasarkan waktu dan nomor urut, dengan label dan deskripsi bilingual. Masukan: Parameter `events` bertipe
    // `List<EventRequest>?` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; nilai null diizinkan ketika data
    // opsional belum tersedia; Parameter `language` bertipe `string?` membawa nilai language; nilai null diizinkan ketika data opsional belum tersedia;
    // bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    public static List<SessionTimelineEventViewModel> MapTimeline(List<EventRequest>? events, string? language = null)
    {
        if (events is null || events.Count == 0)
        {
            return new List<SessionTimelineEventViewModel>();
        }

        var normalizedLanguage = UiText.NormalizeLanguage(language);

        return events
            .OrderBy(item => item.Timestamp)
            .ThenBy(item => item.SequenceNumber)
            .Select(item =>
            {
                var actionSlot = Math.Max(0, item.ActionSlot);
                var actionSlotRole = ResolveActionSlotRole(item.ActionType, item.Payload);

                return new SessionTimelineEventViewModel
                {
                    Timestamp = item.Timestamp,
                    SequenceNumber = item.SequenceNumber,
                    IsSealed = item.Payload.ValueKind == JsonValueKind.Object
                        && item.Payload.TryGetProperty("status", out var status) && status.ValueKind == JsonValueKind.String && status.GetString() == "SEALED",
                    DayIndex = ResolveJourneyDayIndex(item),
                    Weekday = ResolveWeekdayLabel(item.Weekday, normalizedLanguage),
                    ActionSlot = actionSlot,
                    ActorType = item.ActorType,
                    PlayerId = item.UserId,
                    ActionType = item.ActionType,
                    ActionSlotRole = actionSlotRole,
                    ActionSlotLabel = BuildActionSlotLabel(actionSlotRole, actionSlot, normalizedLanguage),
                    FlowLabel = ResolveFlowLabel(item.ActionType, normalizedLanguage),
                    FlowDescription = BuildFlowDescription(item.ActionType, item.Payload, normalizedLanguage)
                };
            })
            .ToList();
    }

    private static int ResolveJourneyDayIndex(EventRequest item)
    {
        if (string.Equals(item.ActionType, "MulaiSesi", StringComparison.OrdinalIgnoreCase) ||
            item.ActionType.StartsWith("Setup", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        return item.Payload.ValueKind == JsonValueKind.Object &&
               item.Payload.TryGetProperty("setup", out var setup) &&
               setup.ValueKind == JsonValueKind.String &&
               string.Equals(setup.GetString(), "INITIAL", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: 0 dalam ResolveJourneyDayIndex.
            ? 0
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: item.DayIndex; dalam ResolveJourneyDayIndex.
            : item.DayIndex;
    }

    /// <summary>
    /// Menetapkan nama tampilan pemain pada setiap item timeline berdasarkan
    /// pemetaan PlayerId ke display name yang diberikan.
    /// </summary>
    /// <param name="timeline">Koleksi item timeline yang akan diperbarui.</param>
    /// <param name="playerDisplayNames">Pemetaan GUID pemain ke nama tampilan.</param>
    // Mendefinisikan metode `ApplyPlayerDisplayNames` dengan hasil bertipe `void`. Menetapkan nama tampilan pemain pada setiap item timeline
    // berdasarkan pemetaan PlayerId ke display name yang diberikan. Masukan: Parameter `timeline` bertipe `IEnumerable<SessionTimelineEventViewModel>`
    // membawa nilai timeline; Parameter `playerDisplayNames` bertipe `IReadOnlyDictionary<Guid, string>` membawa nilai pemain display nama.
    public static void ApplyPlayerDisplayNames(
        // Parameter `timeline` bertipe `IEnumerable<SessionTimelineEventViewModel>` membawa nilai timeline.
        IEnumerable<SessionTimelineEventViewModel> timeline,
        // Parameter `playerDisplayNames` bertipe `IReadOnlyDictionary<Guid, string>` membawa nilai pemain display nama.
        IReadOnlyDictionary<Guid, string> playerDisplayNames)
    {
        // Mengulangi setiap elemen `timeline.Where(item => item.PlayerId.HasValue)`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses
        // oleh badan loop dalam ApplyPlayerDisplayNames.
        foreach (var item in timeline.Where(item => item.PlayerId.HasValue))
        {
            if (item.PlayerId.HasValue &&
                playerDisplayNames.TryGetValue(item.PlayerId.Value, out var displayName) &&
                !string.IsNullOrWhiteSpace(displayName))
            {
                item.PlayerDisplayName = displayName;
            }
        }
    }

    /// <summary>
    /// Menentukan label kategori alur (flow label) berdasarkan prefiks actionType,
    /// misalnya "Setup", "Event Harian", "Risiko", dll.
    /// </summary>
    /// <param name="actionType">Tipe aksi event dari payload API.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Label kategori alur dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `ResolveFlowLabel` dengan hasil bertipe `string`. Menentukan label kategori alur (flow label) berdasarkan prefiks
    // actionType, misalnya ”Setup”, ”Event Harian”, ”Risiko”, dll. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string ResolveFlowLabel(string actionType, string language)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return L(language, "Aktivitas", "Activity");
        }

        if (actionType is "MulaiSesi" or "AkhiriSesi" or "BagikanTieBreaker" or
            "AmbilKartuDariDeck" or "KartuMasukDiscard" or "IsiUlangPasar")
        {
            return L(language, "Setup", "Setup");
        }

        if (actionType is "JumatBerkah" or "InvestasiEmas" or "JualEmas" or "LewatiTransaksiEmas" or "HariMingguLibur")
        {
            return L(language, "Event Harian", "Daily Event");
        }

        if (actionType is "PoinPeringkatDonasi" or "UmumkanJuaraDonasi")
        {
            return L(language, "Peduli Donasi", "Donation Care");
        }

        if (actionType is "RisikoKehidupan" or "BayarRisiko" or "GunakanOpsiDarurat")
        {
            return L(language, "Risiko", "Risk");
        }

        if (actionType is "PinjamanSyariah" or "BayarPinjaman" or "Asuransi")
        {
            return L(language, "Pembiayaan", "Financing");
        }

        if (actionType is "Menabung" or "TarikTabungan" or "TujuanFinansial")
        {
            return L(language, "Tabungan", "Saving");
        }

        if (actionType is "SetupMisiAwal")
        {
            return L(language, "Misi", "Mission");
        }

        if (actionType is "JualMasakan" or "LewatiOrder")
        {
            return L(language, "Pesanan", "Order");
        }

        if (actionType is "BahanMasakan" or "Kebutuhan")
        {
            return L(language, "Pembelian", "Purchase");
        }

        return L(language, "Aktivitas", "Activity");
    }

    private static string ResolveActionSlotRole(string actionType, JsonElement payload)
    {
        return actionType switch
        {
            // Untuk pola `”RisikoKehidupan”`, menghasilkan nilai literal `”effect”` sebagai hasil switch.
            "RisikoKehidupan" => "effect",
            // Untuk pola `”BayarRisiko”`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "BayarRisiko" => "response",
            // Untuk pola `”GunakanOpsiDarurat”`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "GunakanOpsiDarurat" => "response",
            // Untuk pola `”Asuransi”` dengan syarat tambahan `IsInsuranceUse(payload)`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "Asuransi" when IsInsuranceUse(payload) => "response",
            // Untuk pola `_`, menghasilkan nilai literal `”action”` sebagai hasil switch.
            _ => "action"
        };
    }

    private static string BuildActionSlotLabel(string actionSlotRole, int actionSlot, string language)
    {
        return actionSlotRole switch
        {
            // Untuk pola `”effect”`, menghasilkan memanggil `L` dengan `language`, `$”Efek Aksi {actionSlot}”`, `$”Action Effect {actionSlot}”` sebagai hasil
            // switch.
            "effect" => L(language, $"Efek Aksi {actionSlot}", $"Action Effect {actionSlot}"),
            // Untuk pola `”response”`, menghasilkan memanggil `L` dengan `language`, `$”Respons Aksi {actionSlot}”`, `$”Action Response {actionSlot}”` sebagai
            // hasil switch.
            "response" => L(language, $"Respons Aksi {actionSlot}", $"Action Response {actionSlot}"),
            // Untuk pola `_`, menghasilkan memanggil `L` dengan `language`, `$”Aksi {actionSlot}”`, `$”Action {actionSlot}”` sebagai hasil switch.
            _ => L(language, $"Aksi {actionSlot}", $"Action {actionSlot}")
        };
    }

    /// <summary>
    /// Membangun deskripsi naratif untuk setiap event berdasarkan actionType,
    /// mendelegasikan ke fungsi deskripsi spesifik sesuai jenis aksi.
    /// </summary>
    /// <param name="actionType">Tipe aksi event (misal "CatatTransaksi").</param>
    /// <param name="payload">Data payload JSON dari event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi naratif event dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `BuildFlowDescription` dengan hasil bertipe `string`. Membangun deskripsi naratif untuk setiap event berdasarkan
    // actionType, mendelegasikan ke fungsi deskripsi spesifik sesuai jenis aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi
    // jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa
    // nilai language.
    private static string BuildFlowDescription(string actionType, JsonElement payload, string language)
    {
        var text = actionType switch
        {
            // Untuk pola `”CatatTransaksi”`, menghasilkan memanggil `DescribeTransaction` dengan `payload`, `language` sebagai hasil switch.
            "MulaiSesi" => L(language, "Sesi dimulai.", "Session started."),
            "AkhiriSesi" => L(language, "Sesi diakhiri.", "Session ended."),
            "CatatTransaksi" => DescribeTransaction(payload, language),
            // Untuk pola `”JumatBerkah”`, menghasilkan memanggil `DescribeFridayDonation` dengan `payload`, `language` sebagai hasil switch.
            "JumatBerkah" => DescribeFridayDonation(payload, language),
            // Untuk pola `”InvestasiEmas” or ”JualEmas”`, menghasilkan memanggil `DescribeGoldTrade` dengan `payload`, `language` sebagai hasil switch.
            "InvestasiEmas" or "JualEmas" => DescribeGoldTrade(payload, language),
            // Untuk pola `”BahanMasakan”`, menghasilkan memanggil `DescribeIngredientPurchase` dengan `payload`, `language` sebagai hasil switch.
            "BahanMasakan" => DescribeIngredientPurchase(payload, language),
            // Untuk pola `”BuangBahanMasakan”`, menghasilkan memanggil `DescribeIngredientDiscard` dengan `payload`, `language` sebagai hasil switch.
            "BuangBahanMasakan" => DescribeIngredientDiscard(payload, language),
            // Untuk pola `”JualMasakan”`, menghasilkan memanggil `DescribeOrderClaim` dengan `payload`, `language` sebagai hasil switch.
            "JualMasakan" => DescribeOrderClaim(payload, language),
            // Untuk pola `”LewatiOrder”`, menghasilkan memanggil `DescribeOrderPassed` dengan `payload`, `language` sebagai hasil switch.
            "LewatiOrder" => DescribeOrderPassed(payload, language),
            // Untuk pola `”KerjaLepas”`, menghasilkan memanggil `DescribeFreelance` dengan `payload`, `language` sebagai hasil switch.
            "KerjaLepas" => DescribeFreelance(payload, language),
            // Untuk pola `”Kebutuhan”`, menghasilkan memanggil `DescribeNeedPurchase` dengan `payload`, `language`, `ResolveNeedLabel(payload, language)`,
            // `ResolveNeedLabel(payload, language)` sebagai hasil switch.
            "Kebutuhan" => DescribeNeedPurchase(payload, language, ResolveNeedLabel(payload, language), ResolveNeedLabel(payload, language)),
            // Untuk pola `”Menabung”`, menghasilkan memanggil `DescribeSavingDeposit` dengan `payload`, `language`, `false` sebagai hasil switch.
            "Menabung" => DescribeSavingDeposit(payload, language, isWithdrawn: false),
            // Untuk pola `”TarikTabungan”`, menghasilkan memanggil `DescribeSavingDeposit` dengan `payload`, `language`, `true` sebagai hasil switch.
            "TarikTabungan" => DescribeSavingDeposit(payload, language, isWithdrawn: true),
            // Untuk pola `”TujuanFinansial”`, menghasilkan memanggil `DescribeSavingGoalAchieved` dengan `payload`, `language` sebagai hasil switch.
            "TujuanFinansial" => DescribeSavingGoalAchieved(payload, language),
            // Untuk pola `”PinjamanSyariah”`, menghasilkan memanggil `DescribeLoanTaken` dengan `payload`, `language` sebagai hasil switch.
            "PinjamanSyariah" => DescribeLoanTaken(payload, language),
            // Untuk pola `”BayarPinjaman”`, menghasilkan memanggil `DescribeLoanRepaid` dengan `payload`, `language` sebagai hasil switch.
            "BayarPinjaman" => DescribeLoanRepaid(payload, language),
            // Untuk pola `”RisikoKehidupan”`, menghasilkan memanggil `DescribeRiskLife` dengan `payload`, `language` sebagai hasil switch.
            "RisikoKehidupan" => DescribeRiskLife(payload, language),
            // Untuk pola `”GunakanOpsiDarurat”`, menghasilkan memanggil `DescribeRiskEmergency` dengan `payload`, `language` sebagai hasil switch.
            "GunakanOpsiDarurat" => DescribeRiskEmergency(payload, language),
            // Untuk pola `”Asuransi”`, menghasilkan hasil pemilihan bersyarat: ketika `IsInsuranceUse(payload)` benar gunakan `DescribeInsuranceUse(payload,
            // language)`, jika tidak gunakan `DescribeInsurancePurchase(payload, language)` sebagai hasil switch.
            "Asuransi" => IsInsuranceUse(payload) ? DescribeInsuranceUse(payload, language) : DescribeInsurancePurchase(payload, language),
            // Untuk pola `”PoinPeringkatDonasi”`, menghasilkan memanggil `DescribeRankAward` dengan `payload`, `language`, `”donasi”`, `”donation”` sebagai
            // hasil switch.
            "PoinPeringkatDonasi" => DescribeRankAward(payload, language, "donasi", "donation"),
            // Untuk pola `”UmumkanJuaraDonasi”`, menghasilkan memanggil `DescribeDonationWinnersAnnouncement` dengan `payload`, `language` sebagai hasil
            // switch.
            "UmumkanJuaraDonasi" => DescribeDonationWinnersAnnouncement(payload, language),
            // Untuk pola `”PoinPeringkatPensiun”`, menghasilkan memanggil `DescribeRankAward` dengan `payload`, `language`, `”dana pensiun”`, `”pension”`
            // sebagai hasil switch.
            "PoinPeringkatPensiun" => DescribeRankAward(payload, language, "dana pensiun", "pension"),
            // Untuk pola `”PoinEmas”`, menghasilkan memanggil `DescribePointsAward` dengan `payload`, `language`, `”emas”`, `”gold”` sebagai hasil switch.
            "PoinEmas" => DescribePointsAward(payload, language, "emas", "gold"),
            // Untuk pola `”AkhirGiliran”`, menghasilkan memanggil `DescribeTurnAction` dengan `payload`, `language` sebagai hasil switch.
            "AkhirGiliran" => DescribeTurnAction(payload, language),
            // Untuk pola `_`, menghasilkan memanggil `BuildGenericDescription` dengan `actionType`, `payload`, `language` sebagai hasil switch.
            _ => BuildGenericDescription(actionType, payload, language)
        };

        return text;
    }

    /// <summary>
    /// Mendeskripsikan event transaksi kas masuk/keluar, termasuk nominal, kategori, dan pihak terkait.
    /// </summary>
    /// <param name="payload">Data payload JSON event transaksi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi transaksi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeTransaction` dengan hasil bertipe `string`. Mendeskripsikan event transaksi kas masuk/keluar, termasuk nominal,
    // kategori, dan pihak terkait. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeTransaction(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "direction", out var direction))
        {
            return BuildGenericDescription("CatatTransaksi", payload, language);
        }

        var directionLabel = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Kas masuk”, ”Cash in”) dalam DescribeTransaction.
            ? L(language, "Kas masuk", "Cash in")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeTransaction.
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Kas keluar”, ”Cash out”) dalam DescribeTransaction.
                ? L(language, "Kas keluar", "Cash out")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.ToUpperInvariant(); dalam DescribeTransaction.
                : direction.ToUpperInvariant();

        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeTransaction.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeTransaction.
            : L(language, "nominal tidak diketahui", "unknown amount");

        var categoryText = TryGetString(payload, "category", out var category)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: category dalam DescribeTransaction.
            ? category
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”kategori tidak diketahui”, ”unknown category”); dalam
            // DescribeTransaction.
            : L(language, "kategori tidak diketahui", "unknown category");

        var counterpartyText = TryGetString(payload, "counterparty", out var counterparty)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, $” dengan pihak {counterparty}”, $” with counterparty
            // {counterparty}”) dalam DescribeTransaction.
            ? L(language, $" dengan pihak {counterparty}", $" with counterparty {counterparty}")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Empty; dalam DescribeTransaction.
            : string.Empty;

        return L(
            language,
            $"{directionLabel} sebesar {amountText} pada kategori {categoryText}{counterpartyText}.",
            $"{directionLabel} {amountText} in category {categoryText}{counterpartyText}.");
    }

    /// <summary>
    /// Mendeskripsikan event donasi Jumat, termasuk nominal yang didonasikan.
    /// </summary>
    /// <param name="payload">Data payload JSON event donasi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi donasi Jumat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeFridayDonation` dengan hasil bertipe `string`. Mendeskripsikan event donasi Jumat, termasuk nominal yang
    // didonasikan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeFridayDonation(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("JumatBerkah", payload, language);
        }

        return L(
            language,
            $"Melakukan donasi Jumat sebesar {FormatNumber(amount)}.",
            $"Made a Friday donation of {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Mendeskripsikan event perdagangan emas hari Sabtu, termasuk jenis transaksi (beli/jual),
    /// jumlah unit, harga per unit, dan total nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event perdagangan emas.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi perdagangan emas dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeGoldTrade` dengan hasil bertipe `string`. Mendeskripsikan event perdagangan emas hari Sabtu, termasuk jenis
    // transaksi (beli/jual), jumlah unit, harga per unit, dan total nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail
    // event dalam format JSON; Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeGoldTrade(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "trade_type", out var tradeType) ||
            !TryGetInt(payload, "qty", out var qty) ||
            !TryGetInt(payload, "unit_price", out var unitPrice))
        {
            return BuildGenericDescription("TransaksiEmas", payload, language);
        }

        var amount = TryGetInt(payload, "amount", out var parsedAmount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: parsedAmount dalam DescribeGoldTrade.
            ? parsedAmount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: qty * unitPrice; dalam DescribeGoldTrade.
            : qty * unitPrice;
        var action = tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Membeli”, ”Bought”) dalam DescribeGoldTrade.
            ? L(language, "Membeli", "Bought")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeGoldTrade.
            : tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Menjual”, ”Sold”) dalam DescribeGoldTrade.
                ? L(language, "Menjual", "Sold")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”Melakukan transaksi”, ”Executed trade”); dalam
                // DescribeGoldTrade.
                : L(language, "Melakukan transaksi", "Executed trade");

        return L(
            language,
            $"{action} emas {qty} unit x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).",
            $"{action} {qty} gold unit(s) x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).");
    }

    /// <summary>
    /// Mendeskripsikan event pembelian kartu bahan, termasuk ID kartu dan biaya.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian bahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembelian bahan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeIngredientPurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian kartu bahan, termasuk ID kartu
    // dan biaya. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeIngredientPurchase(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("BahanMasakan", payload, language);
        }

        var label = PlayerMetricLabelFormatter.HumanizeMetricKey(cardId, key => UiText.Translate(language, key));
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return L(language, $"Membeli bahan {label}. Biaya belum tercatat.", $"Purchased ingredient {label}. Cost not recorded.");
        }

        return L(
            language,
            $"Membeli bahan {label} dengan biaya {FormatNumber(amount)} koin.",
            $"Purchased ingredient {label} for {FormatNumber(amount)} {(amount == 1 ? "coin" : "coins")}.");
    }

    /// <summary>
    /// Mendeskripsikan event pembuangan kartu bahan saat slot penuh, termasuk ID kartu dan jumlah.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembuangan bahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembuangan bahan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeIngredientDiscard` dengan hasil bertipe `string`. Mendeskripsikan event pembuangan kartu bahan saat slot penuh,
    // termasuk ID kartu dan jumlah. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeIngredientDiscard(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("BuangBahanMasakan", payload, language);
        }

        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeIngredientDiscard.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”jumlah tidak diketahui”, ”unknown quantity”); dalam
            // DescribeIngredientDiscard.
            : L(language, "jumlah tidak diketahui", "unknown quantity");

        return L(
            language,
            $"Membuang bahan {cardId} sebanyak {amountText}.",
            $"Discarded ingredient {cardId} with quantity {amountText}.");
    }

    /// <summary>
    /// Mendeskripsikan event klaim pesanan, termasuk jumlah bahan yang digunakan dan pemasukan yang diterima.
    /// </summary>
    /// <param name="payload">Data payload JSON event klaim pesanan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi klaim pesanan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeOrderClaim` dengan hasil bertipe `string`. Mendeskripsikan event klaim pesanan, termasuk jumlah bahan yang
    // digunakan dan pemasukan yang diterima. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeOrderClaim(JsonElement payload, string language)
    {
        var incomeText = TryGetNumber(payload, "income", out var income)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(income) dalam DescribeOrderClaim.
            ? FormatNumber(income)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeOrderClaim.
            : L(language, "nominal tidak diketahui", "unknown amount");
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: count.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeOrderClaim.
            ? count.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeOrderClaim.
            : L(language, "?", "?");

        return L(
            language,
            $"Menyelesaikan order dengan {ingredientCountText} bahan dan menerima pemasukan {incomeText}.",
            $"Claimed an order using {ingredientCountText} ingredient(s) and received {incomeText} income.");
    }

    /// <summary>
    /// Mendeskripsikan event pesanan yang dilewati (tidak diklaim), termasuk potensi pemasukan.
    /// </summary>
    /// <param name="payload">Data payload JSON event pesanan dilewati.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pesanan yang dilewati dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeOrderPassed` dengan hasil bertipe `string`. Mendeskripsikan event pesanan yang dilewati (tidak diklaim), termasuk
    // potensi pemasukan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeOrderPassed(JsonElement payload, string language)
    {
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: count.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeOrderPassed.
            ? count.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeOrderPassed.
            : L(language, "?", "?");
        var incomeText = TryGetNumber(payload, "income", out var income)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(income) dalam DescribeOrderPassed.
            ? FormatNumber(income)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeOrderPassed.
            : L(language, "nominal tidak diketahui", "unknown amount");

        return L(
            language,
            $"Melewati order yang membutuhkan {ingredientCountText} bahan (potensi pemasukan {incomeText}).",
            $"Passed an order requiring {ingredientCountText} ingredient(s) (potential income {incomeText}).");
    }

    /// <summary>
    /// Mendeskripsikan event pekerjaan freelance, termasuk nominal pemasukan yang diterima.
    /// </summary>
    /// <param name="payload">Data payload JSON event freelance.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pekerjaan freelance dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeFreelance` dengan hasil bertipe `string`. Mendeskripsikan event pekerjaan freelance, termasuk nominal pemasukan
    // yang diterima. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeFreelance(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("KerjaLepas", payload, language);
        }

        return L(
            language,
            $"Menyelesaikan pekerjaan freelance dengan pemasukan {FormatNumber(amount)}.",
            $"Completed freelance work and earned {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Mendeskripsikan event pembelian kartu kebutuhan (primer/sekunder/tersier),
    /// termasuk ID kartu, biaya, dan poin yang diperoleh.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian kebutuhan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="needTypeId">Label jenis kebutuhan dalam bahasa Indonesia.</param>
    /// <param name="needTypeEn">Label jenis kebutuhan dalam bahasa Inggris.</param>
    /// <returns>Deskripsi pembelian kebutuhan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeNeedPurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian kartu kebutuhan
    // (primer/sekunder/tersier), termasuk ID kartu, biaya, dan poin yang diperoleh. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan
    // detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai language; Parameter `needTypeId` bertipe `string` membawa
    // nilai kebutuhan jenis identitas; Parameter `needTypeEn` bertipe `string` membawa nilai kebutuhan jenis en.
    private static string DescribeNeedPurchase(JsonElement payload, string language, string needTypeId, string needTypeEn)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("Kebutuhan", payload, language);
        }

        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeNeedPurchase.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeNeedPurchase.
            : L(language, "nominal tidak diketahui", "unknown amount");
        var pointsText = TryGetNumber(payload, "points", out var points)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(points) dalam DescribeNeedPurchase.
            ? FormatNumber(points)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness
            // points”); dalam DescribeNeedPurchase.
            : L(language, "poin kebahagiaan tidak diketahui", "unknown happiness points");

        return L(
            language,
            $"Membeli {needTypeId} {cardId} (biaya {amountText}, poin kebahagiaan {pointsText}).",
            $"Purchased {needTypeEn} card {cardId} (cost {amountText}, happiness points {pointsText}).");
    }

    private static string ResolveNeedLabel(JsonElement payload, string language)
    {
        var fallback = L(language, "kebutuhan", "need");
        if (!TryGetString(payload, "need_tier", out var tier) &&
            !TryGetString(payload, "tier", out tier) &&
            !TryGetString(payload, "need_type", out tier))
        {
            return fallback;
        }

        return tier.ToUpperInvariant() switch
        {
            // Untuk pola `”PRIMARY” or ”PRIMER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan primer”`, `”primary need”` sebagai hasil switch.
            "PRIMARY" or "PRIMER" => L(language, "kebutuhan primer", "primary need"),
            // Untuk pola `”SECONDARY” or ”SEKUNDER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan sekunder”`, `”secondary need”` sebagai hasil
            // switch.
            "SECONDARY" or "SEKUNDER" => L(language, "kebutuhan sekunder", "secondary need"),
            // Untuk pola `”TERTIARY” or ”TERSIER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan tersier”`, `”tertiary need”` sebagai hasil
            // switch.
            "TERTIARY" or "TERSIER" => L(language, "kebutuhan tersier", "tertiary need"),
            // Untuk pola `_`, menghasilkan `fallback` (nilai fallback) sebagai hasil switch.
            _ => fallback
        };
    }

    /// <summary>
    /// Mendeskripsikan event setoran atau penarikan tabungan tujuan keuangan,
    /// termasuk ID tujuan dan nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event tabungan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="isWithdrawn">True jika penarikan, false jika setoran.</param>
    /// <returns>Deskripsi setoran/penarikan tabungan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeSavingDeposit` dengan hasil bertipe `string`. Mendeskripsikan event setoran atau penarikan tabungan tujuan
    // keuangan, termasuk ID tujuan dan nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language; Parameter `isWithdrawn` bertipe `bool` membawa nilai berstatus withdrawn.
    private static string DescribeSavingDeposit(JsonElement payload, string language, bool isWithdrawn)
    {
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription(isWithdrawn ? "TarikTabungan" : "Menabung", payload, language);
        }

        return isWithdrawn
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L( dalam DescribeSavingDeposit.
            ? L(
                language,
                $"Menarik {FormatNumber(amount)} koin dari tabungan.",
                $"Withdrew {FormatNumber(amount)} {(amount == 1 ? "coin" : "coins")} from savings.")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L( dalam DescribeSavingDeposit.
            : L(
                language,
                $"Menyetor {FormatNumber(amount)} koin ke tabungan.",
                $"Deposited {FormatNumber(amount)} {(amount == 1 ? "coin" : "coins")} to savings.");
    }

    /// <summary>
    /// Mendeskripsikan event tercapainya target tabungan tujuan keuangan,
    /// termasuk ID tujuan, poin, dan biaya.
    /// </summary>
    /// <param name="payload">Data payload JSON event pencapaian target tabungan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pencapaian target tabungan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeSavingGoalAchieved` dengan hasil bertipe `string`. Mendeskripsikan event tercapainya target tabungan tujuan
    // keuangan, termasuk ID tujuan, poin, dan biaya. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeSavingGoalAchieved(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "goal_id", out var goalId))
        {
            return BuildGenericDescription("TujuanFinansial", payload, language);
        }

        var pointsText = TryGetNumber(payload, "points", out var points)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(points) dalam DescribeSavingGoalAchieved.
            ? FormatNumber(points)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness
            // points”); dalam DescribeSavingGoalAchieved.
            : L(language, "poin kebahagiaan tidak diketahui", "unknown happiness points");
        var costText = TryGetNumber(payload, "cost", out var cost)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(cost) dalam DescribeSavingGoalAchieved.
            ? FormatNumber(cost)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”biaya tidak diketahui”, ”unknown cost”); dalam
            // DescribeSavingGoalAchieved.
            : L(language, "biaya tidak diketahui", "unknown cost");

        return L(
            language,
            $"Kartu tujuan finansial {goalId} dibeli (poin kebahagiaan {pointsText}, biaya {costText}).",
            $"Financial goal card {goalId} purchased (happiness points {pointsText}, cost {costText}).");
    }

    /// <summary>
    /// Mendeskripsikan event pengambilan pinjaman syariah,
    /// termasuk ID pinjaman, pokok, cicilan, dan durasi.
    /// </summary>
    /// <param name="payload">Data payload JSON event pengambilan pinjaman.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pengambilan pinjaman dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeLoanTaken` dengan hasil bertipe `string`. Mendeskripsikan event pengambilan pinjaman syariah, termasuk ID
    // pinjaman, pokok, cicilan, dan durasi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeLoanTaken(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "loan_id", out var loanId))
        {
            return BuildGenericDescription("PinjamanSyariah", payload, language);
        }

        var principalText = TryGetNumber(payload, "principal", out var principal)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(principal) dalam DescribeLoanTaken.
            ? FormatNumber(principal)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeLoanTaken.
            : L(language, "nominal tidak diketahui", "unknown amount");
        var repaymentText = TryGetNumber(payload, "repayment_amount", out var repaymentAmount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(repaymentAmount) dalam DescribeLoanTaken.
            ? FormatNumber(repaymentAmount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: TryGetNumber(payload, ”installment”, out var installment) dalam
            // DescribeLoanTaken.
            : TryGetNumber(payload, "installment", out var installment)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(installment) dalam DescribeLoanTaken.
                ? FormatNumber(installment)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal pelunasan tidak diketahui”, ”unknown repayment
                // amount”); dalam DescribeLoanTaken.
                : L(language, "nominal pelunasan tidak diketahui", "unknown repayment amount");
        var durationText = TryGetInt(payload, "duration_turn", out var durationTurn)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: durationTurn.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeLoanTaken.
            ? durationTurn.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeLoanTaken.
            : L(language, "?", "?");

        return L(
            language,
            $"Mengambil pinjaman {loanId} (pokok {principalText}, pelunasan {repaymentText}, durasi {durationText} turn).",
            $"Took loan {loanId} (principal {principalText}, repayment {repaymentText}, duration {durationText} turns).");
    }

    /// <summary>
    /// Mendeskripsikan event pelunasan pinjaman syariah, termasuk ID pinjaman dan nominal pembayaran.
    /// </summary>
    /// <param name="payload">Data payload JSON event pelunasan pinjaman.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pelunasan pinjaman dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeLoanRepaid` dengan hasil bertipe `string`. Mendeskripsikan event pelunasan pinjaman syariah, termasuk ID pinjaman
    // dan nominal pembayaran. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language.
    private static string DescribeLoanRepaid(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "loan_id", out var loanId) || !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("BayarPinjaman", payload, language);
        }

        return L(
            language,
            $"Membayar pinjaman {loanId} sebesar {FormatNumber(amount)}.",
            $"Repaid loan {loanId} by {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Mendeskripsikan event kartu risiko kehidupan yang ditarik.
    /// </summary>
    /// <param name="payload">Data payload JSON event risiko kehidupan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi event risiko kehidupan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRiskLife` dengan hasil bertipe `string`. Mendeskripsikan event kartu risiko kehidupan yang ditarik. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai
    // language.
    private static string DescribeRiskLife(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "risk_id", out var riskId))
        {
            return BuildGenericDescription("RisikoKehidupan", payload, language);
        }

        if (!TryGetString(payload, "direction", out var direction) ||
            !TryGetNumber(payload, "amount", out var amount))
        {
            return L(
                language,
                $"Kartu risiko {riskId} aktif.",
                $"Risk card {riskId} triggered.");
        }

        var directionText = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”dampak positif”, ”positive impact”) dalam
            // DescribeRiskLife.
            ? L(language, "dampak positif", "positive impact")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeRiskLife.
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”dampak negatif”, ”negative impact”) dalam
                // DescribeRiskLife.
                ? L(language, "dampak negatif", "negative impact")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.ToUpperInvariant(); dalam DescribeRiskLife.
                : direction.ToUpperInvariant();

        return L(
            language,
            $"Kartu risiko {riskId} aktif dengan {directionText} sebesar {FormatNumber(amount)}.",
            $"Risk card {riskId} triggered with {directionText} of {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan opsi darurat akibat risiko,
    /// termasuk jenis opsi, arah dampak, dan nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event opsi darurat.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan opsi darurat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRiskEmergency` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan opsi darurat akibat risiko,
    // termasuk jenis opsi, arah dampak, dan nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeRiskEmergency(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "option_type", out var optionType) ||
            !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("GunakanOpsiDarurat", payload, language);
        }

        var directionText = L(language, "menambah saldo", "adds balance");

        return L(
            language,
            $"Menggunakan opsi darurat {optionType} ({directionText} {FormatNumber(amount)}).",
            $"Used emergency option {optionType} ({directionText} {FormatNumber(amount)}).");
    }

    /// <summary>
    /// Mendeskripsikan event pembelian asuransi multirisk, termasuk nominal premi.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian asuransi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembelian asuransi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeInsurancePurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian asuransi multirisk, termasuk
    // nominal premi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeInsurancePurchase(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "premium", out var premium))
        {
            return BuildGenericDescription("Asuransi", payload, language);
        }

        return L(
            language,
            $"Membeli asuransi multirisk dengan premi {FormatNumber(premium)}.",
            $"Purchased multirisk insurance with premium {FormatNumber(premium)}.");
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan klaim asuransi multirisk untuk event risiko tertentu.
    /// </summary>
    /// <param name="payload">Data payload JSON event klaim asuransi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan asuransi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeInsuranceUse` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan klaim asuransi multirisk untuk event
    // risiko tertentu. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeInsuranceUse(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "risk_event_id", out var riskEventId))
        {
            return BuildGenericDescription("Asuransi", payload, language);
        }

        return L(
            language,
            $"Mengaktifkan perlindungan asuransi untuk event risiko {riskEventId}.",
            $"Activated insurance protection for risk event {riskEventId}.");
    }

    private static bool IsInsuranceUse(JsonElement payload)
    {
        return TryGetString(payload, "risk_event_id", out _);
    }

    /// <summary>
    /// Mendeskripsikan event pemberian peringkat (donasi/pensiun), termasuk peringkat dan poin.
    /// </summary>
    /// <param name="payload">Data payload JSON event peringkat.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="topicId">Label topik peringkat dalam bahasa Indonesia.</param>
    /// <param name="topicEn">Label topik peringkat dalam bahasa Inggris.</param>
    /// <returns>Deskripsi pemberian peringkat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRankAward` dengan hasil bertipe `string`. Mendeskripsikan event pemberian peringkat (donasi/pensiun), termasuk
    // peringkat dan poin. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language; Parameter `topicId` bertipe `string` membawa nilai topic identitas; Parameter `topicEn` bertipe `string`
    // membawa nilai topic en.
    private static string DescribeRankAward(JsonElement payload, string language, string topicId, string topicEn)
    {
        if (!TryGetInt(payload, "rank", out var rank) || !TryGetNumber(payload, "points", out var points))
        {
            return BuildGenericDescription("rank.awarded", payload, language);
        }

        return L(
            language,
            $"Mendapat peringkat {rank} pada kategori {topicId} (poin kebahagiaan {FormatNumber(points)}).",
            $"Received rank {rank} in {topicEn} category (happiness points {FormatNumber(points)}).");
    }

    private static string DescribeDonationWinnersAnnouncement(JsonElement payload, string language)
    {
        var summary = TryGetString(payload, "summary", out var summaryText)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: summaryText.Trim().TrimEnd('.') dalam
            // DescribeDonationWinnersAnnouncement.
            ? summaryText.Trim().TrimEnd('.')
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: BuildDonationWinnerSummary(payload, language); dalam
            // DescribeDonationWinnersAnnouncement.
            : BuildDonationWinnerSummary(payload, language);

        if (string.IsNullOrWhiteSpace(summary))
        {
            return BuildGenericDescription("UmumkanJuaraDonasi", payload, language);
        }

        return L(
            language,
            $"Sistem menentukan Juara Donasi: {summary}.",
            $"System determined Donation Winners: {summary}.");
    }

    private static string BuildDonationWinnerSummary(JsonElement payload, string language)
    {
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty("winners", out var winners) ||
            winners.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        // Mengulangi setiap elemen `winners.EnumerateArray()`; elemen saat ini disimpan sebagai `winner` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildDonationWinnerSummary.
        foreach (var winner in winners.EnumerateArray())
        {
            if (!TryGetString(winner, "player_name", out var playerName) ||
                !TryGetInt(winner, "rank", out var rank))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationWinnerSummary.
                continue;
            }

            parts.Add(L(language, $"{playerName} Juara {rank}", $"{playerName} Rank {rank}"));
        }

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Mendeskripsikan event pemberian bonus poin (misalnya poin emas), termasuk jumlah poin.
    /// </summary>
    /// <param name="payload">Data payload JSON event bonus poin.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="topicId">Label topik poin dalam bahasa Indonesia.</param>
    /// <param name="topicEn">Label topik poin dalam bahasa Inggris.</param>
    /// <returns>Deskripsi bonus poin dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribePointsAward` dengan hasil bertipe `string`. Mendeskripsikan event pemberian bonus poin (misalnya poin emas),
    // termasuk jumlah poin. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language; Parameter `topicId` bertipe `string` membawa nilai topic identitas; Parameter `topicEn` bertipe `string`
    // membawa nilai topic en.
    private static string DescribePointsAward(JsonElement payload, string language, string topicId, string topicEn)
    {
        if (!TryGetNumber(payload, "points", out var points))
        {
            return BuildGenericDescription("points.awarded", payload, language);
        }

        return L(
            language,
            $"Mendapat bonus poin kebahagiaan {topicId} sebesar {FormatNumber(points)}.",
            $"Received {topicEn} bonus happiness points of {FormatNumber(points)}.");
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan aksi turn, termasuk jumlah aksi terpakai dan sisa.
    /// </summary>
    /// <param name="payload">Data payload JSON event aksi turn.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan aksi turn dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeTurnAction` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan aksi turn, termasuk jumlah aksi
    // terpakai dan sisa. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeTurnAction(JsonElement payload, string language)
    {
        if (!TryGetInt(payload, "used", out var used) || !TryGetInt(payload, "remaining", out var remaining))
        {
            return BuildGenericDescription("AkhirGiliran", payload, language);
        }

        return L(
            language,
            $"Menggunakan {used} aksi, sisa aksi turn ini {remaining}.",
            $"Used {used} action(s), remaining actions this turn: {remaining}.");
    }

    /// <summary>
    /// Membangun deskripsi generik untuk actionType yang tidak memiliki handler spesifik,
    /// dengan menyertakan ringkasan field utama dari payload.
    /// </summary>
    /// <param name="actionType">Tipe aksi event.</param>
    /// <param name="payload">Data payload JSON event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi generik event dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `BuildGenericDescription` dengan hasil bertipe `string`. Membangun deskripsi generik untuk actionType yang tidak memiliki
    // handler spesifik, dengan menyertakan ringkasan field utama dari payload. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi
    // jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa
    // nilai language.
    private static string BuildGenericDescription(string actionType, JsonElement payload, string language)
    {
        var actionLabel = string.IsNullOrWhiteSpace(actionType) ? L(language, "aktivitas", "activity") : actionType;
        var baseText = L(
            language,
            $"Aksi {actionLabel} dieksekusi pada sesi.",
            $"Action {actionLabel} was executed in this session.");
        var payloadSummary = BuildPayloadSummary(payload, language);
        if (string.IsNullOrWhiteSpace(payloadSummary))
        {
            return baseText;
        }

        return L(language, $"{baseText} Detail: {payloadSummary}", $"{baseText} Details: {payloadSummary}");
    }

    /// <summary>
    /// Merangkum hingga 5 field utama dari payload JSON menjadi string ringkas
    /// dengan label yang diterjemahkan sesuai bahasa aktif.
    /// </summary>
    /// <param name="payload">Data payload JSON event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>String ringkasan payload, atau kosong jika payload bukan objek.</returns>
    // Mendefinisikan metode `BuildPayloadSummary` dengan hasil bertipe `string`. Merangkum hingga 5 field utama dari payload JSON menjadi string
    // ringkas dengan label yang diterjemahkan sesuai bahasa aktif. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam
    // format JSON; Parameter `language` bertipe `string` membawa nilai language.
    private static string BuildPayloadSummary(JsonElement payload, string language)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        var keyOrder = new[]
        {
            "amount",
            "direction",
            "category",
            "trade_type",
            "qty",
            "unit_price",
            "card_id",
            "goal_id",
            "loan_id",
            "risk_id",
            "option_type",
            "premium",
            "rank",
            "points"
        };

        var parts = new List<string>();
        // Mengulangi setiap elemen `keyOrder`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildPayloadSummary.
        foreach (var key in keyOrder)
        {
            if (!payload.TryGetProperty(key, out var value))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildPayloadSummary.
                continue;
            }

            parts.Add($"{ResolvePayloadKeyLabel(key, language)}: {JsonElementToInlineText(value, key, language)}");
            if (parts.Count == 5)
            {
                break;
            }
        }

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Menerjemahkan nama field payload JSON (misalnya "amount", "direction")
    /// menjadi label yang mudah dibaca dalam bahasa yang sesuai.
    /// </summary>
    /// <param name="key">Nama field payload.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Label field yang diterjemahkan.</returns>
    // Mendefinisikan metode `ResolvePayloadKeyLabel` dengan hasil bertipe `string`. Menerjemahkan nama field payload JSON (misalnya ”amount”,
    // ”direction”) menjadi label yang mudah dibaca dalam bahasa yang sesuai. Masukan: Parameter `key` bertipe `string` membawa nilai kunci; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string ResolvePayloadKeyLabel(string key, string language)
    {
        return key switch
        {
            // Untuk pola `”amount”`, menghasilkan memanggil `L` dengan `language`, `”nominal”`, `”amount”` sebagai hasil switch.
            "amount" => L(language, "nominal", "amount"),
            // Untuk pola `”direction”`, menghasilkan memanggil `L` dengan `language`, `”arah”`, `”direction”` sebagai hasil switch.
            "direction" => L(language, "arah", "direction"),
            // Untuk pola `”category”`, menghasilkan memanggil `L` dengan `language`, `”kategori”`, `”category”` sebagai hasil switch.
            "category" => L(language, "kategori", "category"),
            // Untuk pola `”trade_type”`, menghasilkan memanggil `L` dengan `language`, `”jenis transaksi”`, `”trade type”` sebagai hasil switch.
            "trade_type" => L(language, "jenis transaksi", "trade type"),
            // Untuk pola `”qty”`, menghasilkan memanggil `L` dengan `language`, `”jumlah”`, `”qty”` sebagai hasil switch.
            "qty" => L(language, "jumlah", "qty"),
            // Untuk pola `”unit_price”`, menghasilkan memanggil `L` dengan `language`, `”harga per unit”`, `”unit price”` sebagai hasil switch.
            "unit_price" => L(language, "harga per unit", "unit price"),
            // Untuk pola `”card_id”`, menghasilkan memanggil `L` dengan `language`, `”kartu”`, `”card id”` sebagai hasil switch.
            "card_id" => L(language, "kartu", "card id"),
            // Untuk pola `”goal_id”`, menghasilkan memanggil `L` dengan `language`, `”tujuan tabungan”`, `”saving goal”` sebagai hasil switch.
            "goal_id" => L(language, "tujuan tabungan", "saving goal"),
            // Untuk pola `”loan_id”`, menghasilkan memanggil `L` dengan `language`, `”pinjaman”`, `”loan id”` sebagai hasil switch.
            "loan_id" => L(language, "pinjaman", "loan id"),
            // Untuk pola `”risk_id”`, menghasilkan memanggil `L` dengan `language`, `”risiko”`, `”risk id”` sebagai hasil switch.
            "risk_id" => L(language, "risiko", "risk id"),
            // Untuk pola `”option_type”`, menghasilkan memanggil `L` dengan `language`, `”opsi”`, `”option”` sebagai hasil switch.
            "option_type" => L(language, "opsi", "option"),
            // Untuk pola `”premium”`, menghasilkan memanggil `L` dengan `language`, `”premi”`, `”premium”` sebagai hasil switch.
            "premium" => L(language, "premi", "premium"),
            // Untuk pola `”rank”`, menghasilkan memanggil `L` dengan `language`, `”peringkat”`, `”rank”` sebagai hasil switch.
            "rank" => L(language, "peringkat", "rank"),
            // Untuk pola `”points”`, menghasilkan memanggil `L` dengan `language`, `”poin kebahagiaan”`, `”happiness points”` sebagai hasil switch.
            "points" => L(language, "poin kebahagiaan", "happiness points"),
            // Untuk pola `_`, menghasilkan `key` (nilai kunci) sebagai hasil switch.
            _ => key
        };
    }

    /// <summary>
    /// Mengonversi nilai JsonElement menjadi teks inline yang mudah dibaca,
    /// dengan penanganan khusus untuk field "direction" dan "trade_type".
    /// </summary>
    /// <param name="value">Elemen JSON yang akan dikonversi.</param>
    /// <param name="key">Nama field asal untuk konteks penerjemahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Representasi teks inline dari nilai JSON.</returns>
    // Mendefinisikan metode `JsonElementToInlineText` dengan hasil bertipe `string`. Mengonversi nilai JsonElement menjadi teks inline yang mudah
    // dibaca, dengan penanganan khusus untuk field ”direction” dan ”trade_type”. Masukan: Parameter `value` bertipe `JsonElement` membawa nilai nilai;
    // Parameter `key` bertipe `string` membawa nilai kunci; Parameter `language` bertipe `string` membawa nilai language.
    private static string JsonElementToInlineText(JsonElement value, string key, string language)
    {
        if (key == "direction" && value.ValueKind == JsonValueKind.String)
        {
            var direction = value.GetString() ?? string.Empty;
            if (direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "IN (masuk)", "IN");
            }

            if (direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "OUT (keluar)", "OUT");
            }
        }

        if (key == "trade_type" && value.ValueKind == JsonValueKind.String)
        {
            var tradeType = value.GetString() ?? string.Empty;
            if (tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "BUY (beli)", "BUY");
            }

            if (tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "SELL (jual)", "SELL");
            }
        }

        return value.ValueKind switch
        {
            // Untuk pola `JsonValueKind.String`, menghasilkan `value.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti
            // sebagai hasil switch.
            JsonValueKind.String => value.GetString() ?? string.Empty,
            // Untuk pola `JsonValueKind.Number`, menghasilkan memanggil `FormatNumber` dengan `value.GetDouble()` sebagai hasil switch.
            JsonValueKind.Number => FormatNumber(value.GetDouble()),
            // Untuk pola `JsonValueKind.True`, menghasilkan memanggil `L` dengan `language`, `”ya”`, `”true”` sebagai hasil switch.
            JsonValueKind.True => L(language, "ya", "true"),
            // Untuk pola `JsonValueKind.False`, menghasilkan memanggil `L` dengan `language`, `”tidak”`, `”false”` sebagai hasil switch.
            JsonValueKind.False => L(language, "tidak", "false"),
            // Untuk pola `JsonValueKind.Array`, menghasilkan memanggil `L` dengan `language`, `$”[{value.GetArrayLength()} item]”`,
            // `$”[{value.GetArrayLength()} item(s)]”` sebagai hasil switch.
            JsonValueKind.Array => L(language, $"[{value.GetArrayLength()} item]", $"[{value.GetArrayLength()} item(s)]"),
            // Untuk pola `JsonValueKind.Object`, menghasilkan nilai literal `”{...}”` sebagai hasil switch.
            JsonValueKind.Object => "{...}",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };
    }

    /// <summary>
    /// Mencoba mengambil nilai string dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai string hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai string tidak kosong.</returns>
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`. Mencoba mengambil nilai string dari properti payload JSON. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetString(JsonElement payload, string propertyName, out string value)
    {
        value = string.Empty;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = property.GetString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Mencoba mengambil nilai integer dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai integer hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai numerik.</returns>
    // Mendefinisikan metode `TryGetInt` dengan hasil bertipe `bool`. Mencoba mengambil nilai integer dari properti payload JSON. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetInt(JsonElement payload, string propertyName, out int value)
    {
        value = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        if (property.TryGetInt32(out value))
        {
            return true;
        }

        value = (int)Math.Round(property.GetDouble());
        return true;
    }

    /// <summary>
    /// Mencoba mengambil nilai numerik (double) dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai double hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai numerik.</returns>
    // Mendefinisikan metode `TryGetNumber` dengan hasil bertipe `bool`. Mencoba mengambil nilai numerik (double) dari properti payload JSON. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai
    // property nama; Parameter `value` bertipe `double` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetNumber(JsonElement payload, string propertyName, out double value)
    {
        value = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        value = property.GetDouble();
        return true;
    }

    /// <summary>
    /// Mencoba mengambil jumlah elemen dari properti array dalam payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti array yang dicari.</param>
    /// <param name="count">Jumlah elemen dalam array.</param>
    /// <returns>True jika properti ditemukan dan bertipe array.</returns>
    // Mendefinisikan metode `TryGetArrayCount` dengan hasil bertipe `bool`. Mencoba mengambil jumlah elemen dari properti array dalam payload JSON.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string`
    // membawa nilai property nama; Parameter `count` bertipe `int` membawa nilai jumlah; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode.
    private static bool TryGetArrayCount(JsonElement payload, string propertyName, out int count)
    {
        count = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        count = property.GetArrayLength();
        return true;
    }

    /// <summary>
    /// Memformat angka double menjadi string dengan maksimal 2 desimal menggunakan kultur invariant.
    /// </summary>
    /// <param name="value">Nilai numerik yang akan diformat.</param>
    /// <returns>String angka yang diformat.</returns>
    // Mendefinisikan metode `FormatNumber` dengan hasil bertipe `string`. Memformat angka double menjadi string dengan maksimal 2 desimal menggunakan
    // kultur invariant. Masukan: Parameter `value` bertipe `double` membawa nilai nilai. Nilai hasil langsung berasal dari mengubah `value` menjadi
    // teks memakai format `”0.##”`, `CultureInfo.InvariantCulture`.
    private static string FormatNumber(double value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    /// <summary>
    /// Menerjemahkan kode weekday event menjadi nama hari sesuai bahasa aktif.
    /// </summary>
    /// <param name="weekday">Kode hari dari API, misalnya MON atau TUE.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Nama hari terlokalisasi atau nilai asal jika kode tidak dikenal.</returns>
    // Mendefinisikan metode `ResolveWeekdayLabel` dengan hasil bertipe `string`. Menerjemahkan kode weekday event menjadi nama hari sesuai bahasa
    // aktif. Masukan: Parameter `weekday` bertipe `string` membawa nilai weekday; Parameter `language` bertipe `string` membawa nilai language.
    private static string ResolveWeekdayLabel(string weekday, string language)
    {
        return (weekday ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            // Untuk pola `”MON”`, menghasilkan memanggil `L` dengan `language`, `”Senin”`, `”Monday”` sebagai hasil switch.
            "MON" => L(language, "Senin", "Monday"),
            // Untuk pola `”TUE”`, menghasilkan memanggil `L` dengan `language`, `”Selasa”`, `”Tuesday”` sebagai hasil switch.
            "TUE" => L(language, "Selasa", "Tuesday"),
            // Untuk pola `”WED”`, menghasilkan memanggil `L` dengan `language`, `”Rabu”`, `”Wednesday”` sebagai hasil switch.
            "WED" => L(language, "Rabu", "Wednesday"),
            // Untuk pola `”THU”`, menghasilkan memanggil `L` dengan `language`, `”Kamis”`, `”Thursday”` sebagai hasil switch.
            "THU" => L(language, "Kamis", "Thursday"),
            // Untuk pola `”FRI”`, menghasilkan memanggil `L` dengan `language`, `”Jumat”`, `”Friday”` sebagai hasil switch.
            "FRI" => L(language, "Jumat", "Friday"),
            // Untuk pola `”SAT”`, menghasilkan memanggil `L` dengan `language`, `”Sabtu”`, `”Saturday”` sebagai hasil switch.
            "SAT" => L(language, "Sabtu", "Saturday"),
            // Untuk pola `”SUN”`, menghasilkan memanggil `L` dengan `language`, `”Minggu”`, `”Sunday”` sebagai hasil switch.
            "SUN" => L(language, "Minggu", "Sunday"),
            // Untuk pola `_`, menghasilkan `weekday` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti sebagai hasil switch.
            _ => weekday ?? string.Empty
        };
    }

    /// <summary>
    /// Helper bilingual: mengembalikan teks bahasa Indonesia atau Inggris sesuai kode bahasa aktif.
    /// </summary>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="id">Teks dalam bahasa Indonesia.</param>
    /// <param name="en">Teks dalam bahasa Inggris.</param>
    /// <returns>Teks sesuai bahasa yang dipilih.</returns>
    // Mendefinisikan metode `L` dengan hasil bertipe `string`. Helper bilingual: mengembalikan teks bahasa Indonesia atau Inggris sesuai kode bahasa
    // aktif. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter `id` bertipe `string` membawa nilai identitas; Parameter
    // `en` bertipe `string` membawa nilai en. Nilai hasil langsung berasal dari hasil pemilihan bersyarat: ketika `string.Equals(language,
    // AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)` benar gunakan `en`, jika tidak gunakan `id`.
    private static string L(string language, string id, string en)
        => string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase) ? en : id;
}
