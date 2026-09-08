// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionJourneyPlayerFeedTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `SessionJourneyPlayerFeedTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionJourneyPlayerFeedTests
// Membuka scope tipe SessionJourneyPlayerFeedTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout` dengan hasil bertipe `void`; operasi ini menangani sesi
    // journey partial should use readable mixed actor feed layout.
    public void SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout()
    // Membuka scope metode SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneySection.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”, ”Cashflowpoly.Ui”,
        // ”wwwroot”, ”css”, ”tailwind.input.css”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "tailwind.input.css"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-filter”`, `viewContent`
        // dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("session-journey-filter", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-feed”`, `viewContent`
        // dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("session-journey-feed", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_mixed_subtitle”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_mixed_subtitle", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_filter_label”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_filter_label", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-filter-label”`,
        // `viewContent`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("session-journey-filter-label", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-filter-all-mark”`,
        // `viewContent`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("session-journey-filter-all-mark", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_filter_all”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_filter_all", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_filter_players”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_filter_players", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_filter_system”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_filter_system", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessions.timeline_active_players”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("sessions.timeline_active_players", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-journey-filter-btn--turn-1”`,
        // `css`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter-btn--turn-1", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-journey-filter-btn--turn-2”`,
        // `css`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter-btn--turn-2", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-journey-filter-btn--turn-3”`,
        // `css`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter-btn--turn-3", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-journey-filter-btn--turn-4”`,
        // `css`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter-btn--turn-4", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”.session-journey-filter-btn--player.is-active”`, `css`, `StringComparison.Ordinal` dalam
        // SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter-btn--player.is-active", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-journey-filter {”`, `css`,
        // `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains(".session-journey-filter {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”justify-content: center;”`, `css`,
        // `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.Contains("justify-content: center;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-board-track”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.DoesNotContain("session-board-track", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-legend”`,
        // `viewContent` dalam SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
        Assert.DoesNotContain("session-journey-legend", viewContent);
    // Menutup scope metode SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile` dengan hasil bertipe `void`; operasi ini menangani
    // sesi rincian should size journey insights berdasarkan kartu jumlah dan stack on mobile.
    public void SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile()
    // Membuka scope metode SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”Details.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "Details.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns: minmax(0,
        // 1fr);”`, `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains("grid-template-columns: minmax(0, 1fr);", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-count-grid:has(>
        // :nth-child(2):last-child)”`, `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains(".session-count-grid:has(> :nth-child(2):last-child)", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-count-grid:has(>
        // :nth-child(3):last-child)”`, `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains(".session-count-grid:has(> :nth-child(3):last-child)", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.session-count-grid:has(>
        // :nth-child(4):last-child)”`, `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains(".session-count-grid:has(> :nth-child(4):last-child)", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns: repeat(3,
        // minmax(0, 1fr));”`, `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains("grid-template-columns: repeat(3, minmax(0, 1fr));", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-stats session-count-grid”`,
        // `viewContent` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains("ruleset-stats session-count-grid", viewContent);

        // Menyiapkan variabel lokal `journeySectionPath` untuk nilai journey section path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Views”`, `”Sessions”`, `”_SessionJourneySection.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var journeySectionPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-insight-grid
        // session-count-grid”`, `File.ReadAllText(journeySectionPath)` dalam SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
        Assert.Contains("session-journey-insight-grid session-count-grid", File.ReadAllText(journeySectionPath));
    // Menutup scope metode SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile; bagian berikut berada di luar batas blok tersebut dalam
    // SessionDetails_ShouldSizeJourneyInsightsByCardCountAndStackOnMobile.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents` dengan hasil bertipe `void`; operasi ini menangani sesi
    // journey script should render pemain dan important system event.
    public void SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents()
    // Membuka scope metode SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-feed”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("session-journey-feed", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”selectedTimelineFilter”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("selectedTimelineFilter", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-filter-avatar”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("session-journey-filter-avatar", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”session-journey-filter-btn--turn-${playerOrder}”`, `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("session-journey-filter-btn--turn-${playerOrder}", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”isImportantSystemEvent”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("isImportantSystemEvent", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”umumkanjuaradonasi”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("umumkanjuaradonasi", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”actorBucket(item.actorType) ===
        // \”SYSTEM\””`, `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("actorBucket(item.actorType) === \"SYSTEM\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.actionType !== \”AkhirGiliran\””`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("item.actionType !== \"AkhirGiliran\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-feed-item-system”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.Contains("session-journey-feed-item-system", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”renderJourneyBoard”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.DoesNotContain("renderJourneyBoard", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-board-node”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
        Assert.DoesNotContain("session-board-node", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldNotRenderSequenceOrdinal` dengan hasil bertipe `void`; operasi ini menangani sesi journey
    // script should not render sequence ordinal.
    public void SessionJourneyScript_ShouldNotRenderSequenceOrdinal()
    // Membuka scope metode SessionJourneyScript_ShouldNotRenderSequenceOrdinal; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”feedTimeline.map((item) =>”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
        Assert.Contains("feedTimeline.map((item) =>", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”displaySequenceNumber”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
        Assert.DoesNotContain("displaySequenceNumber", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”seqLabel”`, `scriptContent` dalam
        // SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
        Assert.DoesNotContain("seqLabel", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session-journey-feed-seq”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
        Assert.DoesNotContain("session-journey-feed-seq", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldNotRenderSequenceOrdinal; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldNotRenderSequenceOrdinal.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing` dengan hasil bertipe `void`; operasi ini menangani sesi
    // journey script should render hari zero as go tanpa incrementing.
    public void SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing()
    // Membuka scope metode SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”displayDayNumber”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.Contains("displayDayNumber", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”dayPositionLabel”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.Contains("dayPositionLabel", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”dayNumber === 0 ? \”GO\””`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.Contains("dayNumber === 0 ? \"GO\"", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”dayIndex + 1”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.DoesNotContain("dayIndex + 1", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”${dayPositionLabel(item.dayIndex)}”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.Contains("${dayPositionLabel(item.dayIndex)}", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Math.max(0, toNumber(readValue(item,
        // \”actionSlot\”, \”ActionSlot\”), 0))”`, `scriptContent` dalam SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
        Assert.Contains("Math.max(0, toNumber(readValue(item, \"actionSlot\", \"ActionSlot\"), 0))", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldKeepEmptyBoardDaySelected` dengan hasil bertipe `void`; operasi ini menangani sesi journey
    // script should keep empty board hari selected.
    public void SessionJourneyScript_ShouldKeepEmptyBoardDaySelected()
    // Membuka scope metode SessionJourneyScript_ShouldKeepEmptyBoardDaySelected; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SessionJourneyScript_ShouldKeepEmptyBoardDaySelected.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”const selectedDayIsAvailable =
        // selectedTimelineFilter === \”all\””`, `scriptContent` dalam SessionJourneyScript_ShouldKeepEmptyBoardDaySelected.
        Assert.Contains("const selectedDayIsAvailable = selectedTimelineFilter === \"all\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”selectedDay >= 0 && selectedDay <=
        // finishMarkerDay”`, `scriptContent` dalam SessionJourneyScript_ShouldKeepEmptyBoardDaySelected.
        Assert.Contains("selectedDay >= 0 && selectedDay <= finishMarkerDay", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”dayEventCountEl.textContent =
        // dayFeedTimeline.length”`, `scriptContent` dalam SessionJourneyScript_ShouldKeepEmptyBoardDaySelected.
        Assert.Contains("dayEventCountEl.textContent = dayFeedTimeline.length", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldKeepEmptyBoardDaySelected; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldKeepEmptyBoardDaySelected.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyControls_ShouldExposeSelectionAndDisabledStates` dengan hasil bertipe `void`; operasi ini menangani sesi
    // journey controls should expose selection dan disabled states.
    public void SessionJourneyControls_ShouldExposeSelectionAndDisabledStates()
    // Membuka scope metode SessionJourneyControls_ShouldExposeSelectionAndDisabledStates; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `sectionPath` untuk nilai section path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneySection.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sectionPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `sectionContent` untuk nilai section content dengan memanggil `File.ReadAllText` dengan `sectionPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var sectionContent = File.ReadAllText(sectionPath);
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-pressed=\”true\””`,
        // `sectionContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("aria-pressed=\"true\"", sectionContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-pressed=\”${selectedTimelineFilter
        // === filterKey ? \”true\” : \”false\”}\””`, `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("aria-pressed=\"${selectedTimelineFilter === filterKey ? \"true\" : \"false\"}\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”tabindex=\”${hasEvents ? \”0\” :
        // \”-1\”}\””`, `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("tabindex=\"${hasEvents ? \"0\" : \"-1\"}\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-disabled=\”${hasEvents ? \”false\”
        // : \”true\”}\””`, `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("aria-disabled=\"${hasEvents ? \"false\" : \"true\"}\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-label=\”${escapeHtml(`${dayLabel}
        // ${d}`)}\””`, `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("aria-label=\"${escapeHtml(`${dayLabel} ${d}`)}\"", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”aria-label=\”${escapeHtml(finishLabel)}\””`, `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.Contains("aria-label=\"${escapeHtml(finishLabel)}\"", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-label=\”Day ${d}\””`,
        // `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.DoesNotContain("aria-label=\"Day ${d}\"", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aria-label=\”Finish\””`,
        // `scriptContent` dalam SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
        Assert.DoesNotContain("aria-label=\"Finish\"", scriptContent);
    // Menutup scope metode SessionJourneyControls_ShouldExposeSelectionAndDisabledStates; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyControls_ShouldExposeSelectionAndDisabledStates.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence` dengan hasil bertipe `void`; operasi ini menangani sesi
    // journey script should permintaan only event after the last sequence.
    public void SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence()
    // Membuka scope metode SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”let timelineCursor = null;”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
        Assert.Contains("let timelineCursor = null;", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”encodeURIComponent(timelineCursor)”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
        Assert.Contains("encodeURIComponent(timelineCursor)", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”encodeURIComponent(lastSequence)”`, `scriptContent` dalam SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
        Assert.DoesNotContain("encodeURIComponent(lastSequence)", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”response.redirected &&
        // response.url.includes(\”/auth/login\”)”`, `scriptContent` dalam SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
        Assert.Contains("response.redirected && response.url.includes(\"/auth/login\")", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents` dengan hasil bertipe `void`; operasi ini menangani
    // sesi journey script should use mapped aksi slot labels untuk pemain event.
    public void SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents()
    // Membuka scope metode SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”actionSlotLabelText”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
        Assert.Contains("actionSlotLabelText", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.actionSlotLabel”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
        Assert.Contains("item.actionSlotLabel", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”readValue(item, \”actionSlotLabel\”,
        // \”ActionSlotLabel\”)”`, `scriptContent` dalam SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
        Assert.Contains("readValue(item, \"actionSlotLabel\", \"ActionSlotLabel\")", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”? `${dayLabel}
        // ${displayDayNumber(item.dayIndex)} | ${actionSlotLabel} ${item.actionSlot}`”`, `scriptContent` dalam
        // SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
        Assert.DoesNotContain("? `${dayLabel} ${displayDayNumber(item.dayIndex)} | ${actionSlotLabel} ${item.actionSlot}`", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”? `${dayLabel}
        // ${displayDayNumber(latestItem.dayIndex)} | ${actionSlotLabel} ${latestItem.actionSlot}`”`, `scriptContent` dalam
        // SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
        Assert.DoesNotContain("? `${dayLabel} ${displayDayNumber(latestItem.dayIndex)} | ${actionSlotLabel} ${latestItem.actionSlot}`", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards` dengan hasil bertipe `void`; operasi ini
    // menangani sesi journey script should nest related pemain event inside primary aksi kartu.
    public void SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards()
    // Membuka scope metode SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `scriptPath` untuk nilai script path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Sessions”`, `”_SessionJourneyScript.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        // Menyiapkan variabel lokal `scriptContent` untuk nilai script content dengan memanggil `File.ReadAllText` dengan `scriptPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var scriptContent = File.ReadAllText(scriptPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”isPrimaryTimelineItem”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("isPrimaryTimelineItem", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”buildFeedTimeline”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("buildFeedTimeline", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”relatedEvents”`, `scriptContent` dalam
        // SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("relatedEvents", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pendingRelatedEvents”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("pendingRelatedEvents", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”renderRelatedEvents”`, `scriptContent`
        // dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("renderRelatedEvents", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”const feedTimeline =
        // buildFeedTimeline(visibleTimeline);”`, `scriptContent` dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("const feedTimeline = buildFeedTimeline(visibleTimeline);", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”renderJourneyInsights(feedTimeline);”`,
        // `scriptContent` dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("renderJourneyInsights(feedTimeline);", scriptContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”renderJourneyFeed(baseTimeline,
        // feedTimeline);”`, `scriptContent` dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.Contains("renderJourneyFeed(baseTimeline, feedTimeline);", scriptContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”renderJourneyFeed(baseTimeline,
        // visibleTimeline);”`, `scriptContent` dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
        Assert.DoesNotContain("renderJourneyFeed(baseTimeline, visibleTimeline);", scriptContent);
    // Menutup scope metode SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards; bagian berikut berada di luar batas blok
    // tersebut dalam SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard` dengan hasil bertipe `void`; operasi ini
    // menangani sesi journey lexicon should use activity terminology untuk latest timeline kartu.
    public void SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard()
    // Membuka scope metode SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `lexiconPath` untuk nilai lexicon path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Infrastructure”`, `”UiTextLexicon.Sessions.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexiconPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Sessions.cs");
        // Menyiapkan variabel lokal `lexiconContent` untuk nilai lexicon content dengan memanggil `File.ReadAllText` dengan `lexiconPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var lexiconContent = File.ReadAllText(lexiconPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Hari dan Aktivitas Terakhir”`,
        // `lexiconContent` dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.Contains("Hari dan Aktivitas Terakhir", lexiconContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Aksi”`, `lexiconContent` dalam
        // SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.Contains("Aksi", lexiconContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”slot aksi”`, `lexiconContent` dalam
        // SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.Contains("slot aksi", lexiconContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Hari dan Aksi Terakhir”`,
        // `lexiconContent` dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.DoesNotContain("Hari dan Aksi Terakhir", lexiconContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Hari dan Giliran Terakhir”`,
        // `lexiconContent` dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.DoesNotContain("Hari dan Giliran Terakhir", lexiconContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”Giliran\””`, `lexiconContent`
        // dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
        Assert.DoesNotContain("\"Giliran\"", lexiconContent);
    // Menutup scope metode SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard; bagian berikut berada di luar batas blok tersebut
    // dalam SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard.
    }

    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`; operasi ini menangani resolve repositori root.
    private static string ResolveRepositoryRoot()
    // Membuka scope metode ResolveRepositoryRoot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
    {
        // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan objek baru bertipe `DirectoryInfo` dengan argumen (AppContext.BaseDirectory).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        // Mengulangi blok selama hasil pencocokan `current` dengan pola `not null`; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // ResolveRepositoryRoot.
        while (current is not null)
        // Membuka scope loop selama `current is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
        {
            // Memeriksa memanggil `File.Exists` dengan `Path.Combine(current.FullName, ”Cashflowpoly.sln”)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveRepositoryRoot.
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            // Membuka scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ResolveRepositoryRoot.
            {
                // Mengembalikan `current.FullName` (nilai full nama) kepada pemanggil dalam ResolveRepositoryRoot; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return current.FullName;
            // Menutup scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveRepositoryRoot.
            }

            // Memperbarui `current` menggunakan `current.Parent` (nilai parent) dalam ResolveRepositoryRoot.
            current = current.Parent;
        // Menutup scope loop selama `current is not null`; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Repository root tidak ditemukan.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException("Repository root tidak ditemukan.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe SessionJourneyPlayerFeedTests; bagian berikut berada di luar batas blok tersebut.
}
