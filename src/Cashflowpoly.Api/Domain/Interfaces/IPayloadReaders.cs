// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IPayloadReaders.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IAnalyticsPayloadReader`.
public interface IAnalyticsPayloadReader
// Membuka scope tipe IAnalyticsPayloadReader; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`; operasi ini menangani try read transaction. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `direction` bertipe `string` membawa nilai direction; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `category` bertipe `string` membawa nilai
    // category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category);
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`; operasi ini menangani try read nominal. Masukan: Parameter `payloadJson`
    // bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam
    // operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadAmount(string payloadJson, out double amount);
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`; operasi ini menangani try read emas trade. Masukan: Parameter `payloadJson`
    // bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode.
    bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty);
    // Mendefinisikan metode `IsActionEvent` dengan hasil bertipe `bool`; operasi ini menangani berstatus aksi event. Masukan: Parameter `actionType`
    // bertipe `string` membawa nilai aksi jenis.
    bool IsActionEvent(string actionType);
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`; operasi ini menangani try read aksi used. Masukan: Parameter `payloadJson`
    // bertipe `string` membawa nilai payload JSON; Parameter `used` bertipe `int` membawa nilai used; out mengembalikan nilai melalui parameter dan
    // harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode.
    bool TryReadActionUsed(string payloadJson, out int used, out int remaining);
    // Mendefinisikan metode `TryReadGoldTradeDetailed` dengan hasil bertipe `bool`; operasi ini menangani try read emas trade detailed. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int` membawa nilai unit harga; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadGoldTradeDetailed(string payloadJson, out string tradeType, out int qty, out int unitPrice, out int amount);
    // Mendefinisikan metode `TryReadIngredientPurchaseDetailed` dengan hasil bertipe `bool`; operasi ini menangani try read bahan pembelian detailed.
    // Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `ingredientName` bertipe `string` membawa nilai bahan nama; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadIngredientPurchaseDetailed(string payloadJson, out string cardId, out string ingredientName, out int amount);
    // Mendefinisikan metode `TryReadSavingDeposit` dengan hasil bertipe `bool`; operasi ini menangani try read tabungan deposit. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa nilai target identitas; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount);
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`; operasi ini menangani try read bahan pembelian. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount);
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`; operasi ini menangani try read kebutuhan pembelian. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang
    // dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cardId` bertipe `string` membawa nilai
    // kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points);
    // Mendefinisikan metode `TryReadSoldNeed` dengan hasil bertipe `bool`; operasi ini menangani try read terjual kebutuhan. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadSoldNeed(string payloadJson, out string cardId);
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`; operasi ini menangani try read misi assigned. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `missionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `targetTertiaryCardId` bertipe `string` membawa nilai target
    // tertiary kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int` membawa
    // nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `requirePrimary` bertipe `bool` membawa
    // nilai require primary; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `requireSecondary` bertipe `bool` membawa
    // nilai require secondary; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadMissionAssigned(string payloadJson, out string missionId, out string targetTertiaryCardId, out int penaltyPoints, out bool requirePrimary, out bool requireSecondary);
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`; operasi ini menangani try read tie breaker. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `number` bertipe `int` membawa nilai number; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    bool TryReadTieBreaker(string payloadJson, out int number);
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`; operasi ini menangani try read rank awarded. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `rank` bertipe `int` membawa nilai rank; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus
    // diisi oleh metode.
    bool TryReadRankAwarded(string payloadJson, out int rank, out int points);
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`; operasi ini menangani try read poin awarded. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    bool TryReadPointsAwarded(string payloadJson, out int points);
    // Mendefinisikan metode `TryReadSavingGoalAchievedDetailed` dengan hasil bertipe `bool`; operasi ini menangani try read tabungan target achieved
    // detailed. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa nilai target
    // identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    bool TryReadSavingGoalAchievedDetailed(string payloadJson, out string goalId, out int points, out int cost);
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`; operasi ini menangani try read tabungan target achieved. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    bool TryReadSavingGoalAchieved(string payloadJson, out int points);
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`; operasi ini menangani try read pinjaman taken. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints);
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`; operasi ini menangani try read pinjaman repay. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount);
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`; operasi ini menangani try read urutan/pesanan claim. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `requiredCards` bertipe `List<string>` membawa nilai required kartu; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income` bertipe `int` membawa nilai pemasukan; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income);
// Menutup scope tipe IAnalyticsPayloadReader; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventPayloadReader`.
public interface IEventPayloadReader
// Membuka scope tipe IEventPayloadReader; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ReadPayload` dengan hasil bertipe `JsonElement`; operasi ini menangani read payload. Masukan: Parameter `payload` bertipe
    // `string` membawa muatan detail event dalam format JSON.
    JsonElement ReadPayload(string payload);
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`; operasi ini menangani try get string. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter
    // `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryGetString(JsonElement payload, string propertyName, out string value);
    // Mendefinisikan metode `TryGetOptionalString` dengan hasil bertipe `bool`; operasi ini menangani try get optional string. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `string?` membawa nilai nilai; nilai null diizinkan ketika data opsional belum tersedia; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    bool TryGetOptionalString(JsonElement payload, string propertyName, out string? value);
    // Mendefinisikan metode `TryGetInt32` dengan hasil bertipe `bool`; operasi ini menangani try get int 32. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter
    // `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `required` bertipe
    // `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    bool TryGetInt32(JsonElement payload, string propertyName, out int value, bool required = true);
    // Mendefinisikan metode `TryGetDouble` dengan hasil bertipe `bool`; operasi ini menangani try get double. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter
    // `value` bertipe `double` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `required` bertipe
    // `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    bool TryGetDouble(JsonElement payload, string propertyName, out double value, bool required = true);
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`; operasi ini menangani try read transaction. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `direction` bertipe `string` membawa nilai direction; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `category` bertipe `string` membawa
    // nilai category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `counterparty` bertipe `string?` membawa nilai
    // counterparty; nilai null diizinkan ketika data opsional belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty);
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`; operasi ini menangani try read nominal. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi yang
    // dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadAmount(JsonElement payload, out double amount);
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`; operasi ini menangani try read emas trade. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int` membawa nilai unit harga; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount);
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`; operasi ini menangani try read aksi used. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `used` bertipe `int` membawa nilai used; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    bool TryReadActionUsed(JsonElement payload, out int used, out int remaining);
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`; operasi ini menangani try read bahan pembelian. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount);
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`; operasi ini menangani try read urutan/pesanan claim. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `requiredCards` bertipe `List<string>` membawa nilai
    // required kartu; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income` bertipe `int` membawa nilai pemasukan;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income);
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`; operasi ini menangani try read kebutuhan pembelian. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai
    // poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadNeedPurchase(JsonElement payload, out string cardId, out int amount, out int points);
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`; operasi ini menangani try read misi assigned. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `missionId` bertipe `string` membawa identitas misi
    // koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `targetCardId` bertipe `string` membawa
    // nilai target kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int`
    // membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadMissionAssigned(JsonElement payload, out string missionId, out string targetCardId, out int penaltyPoints);
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`; operasi ini menangani try read tie breaker. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `number` bertipe `int` membawa nilai number; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadTieBreaker(JsonElement payload, out int number);
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`; operasi ini menangani try read rank awarded. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `rank` bertipe `int` membawa nilai rank; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan
    // harus diisi oleh metode.
    bool TryReadRankAwarded(JsonElement payload, out int rank, out int points);
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`; operasi ini menangani try read poin awarded. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadPointsAwarded(JsonElement payload, out int points);
    // Mendefinisikan metode `TryReadSavingDeposit` dengan hasil bertipe `bool`; operasi ini menangani try read tabungan deposit. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe `string` membawa nilai target
    // identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau
    // nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount);
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`; operasi ini menangani try read tabungan target achieved. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe `string` membawa nilai target
    // identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    bool TryReadSavingGoalAchieved(JsonElement payload, out string goalId, out int points, out int cost);
    // Mendefinisikan metode `TryReadRiskLife` dengan hasil bertipe `bool`; operasi ini menangani try read risiko life. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskId` bertipe `string` membawa nilai risiko identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe `string` membawa nilai direction; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadRiskLife(JsonElement payload, out string riskId, out string direction, out int amount);
    // Mendefinisikan metode `TryReadInsuranceUsed` dengan hasil bertipe `bool`; operasi ini menangani try read asuransi used. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string` membawa nilai risiko
    // event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadInsuranceUsed(JsonElement payload, out string riskEventId);
    // Mendefinisikan metode `TryReadEmergencyOption` dengan hasil bertipe `bool`; operasi ini menangani try read emergency option. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string` membawa nilai risiko
    // event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `optionType` bertipe `string` membawa nilai
    // option jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe `string` membawa nilai
    // direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau
    // nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadEmergencyOption(JsonElement payload, out string riskEventId, out string optionType, out string direction, out int amount);
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`; operasi ini menangani try read pinjaman taken. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `repaymentAmount` bertipe `int` membawa nilai repayment nominal; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode; Parameter `duration` bertipe `int` membawa nilai duration; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    bool TryReadLoanTaken(JsonElement payload, out string loanId, out int principal, out int repaymentAmount, out int duration, out int penaltyPoints);
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`; operasi ini menangani try read pinjaman repay. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadLoanRepay(JsonElement payload, out string loanId, out int amount);
    // Mendefinisikan metode `TryReadInsurance` dengan hasil bertipe `bool`; operasi ini menangani try read asuransi. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `premium` bertipe `int` membawa nilai premium; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode.
    bool TryReadInsurance(JsonElement payload, out int premium);
// Menutup scope tipe IEventPayloadReader; bagian berikut berada di luar batas blok tersebut.
}
