using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class RulebookContent
{
    public static RulebookPageViewModel Build()
    {
        return new RulebookPageViewModel
        {
            Title = "Cashflowpoly - Entrepreneur Edition",
            Subtitle = "Panduan pemain: setup, alur aksi, dan perhitungan poin (A-M)",
            Sections = new List<RulebookSectionViewModel>
            {
                new()
                {
                    Heading = "A. Komponen permainan",
                    Description = "Daftar komponen mode pemula dan mode mahir.",
                    Points = new List<string>
                    {
                        "Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.",
                        "Mode pemula: token Mr.Cashflowpoly, token aksi pemain, tent card tujuan permainan, layar pemain, papan donasi, papan investasi emas, papan kalender kerja, papan pesanan, papan kebutuhan, papan bahan.",
                        "Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi, emas, kebutuhan, pesanan, bahan.",
                        "Mode mahir menambah papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi dan bank syariah.",
                        "Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.",
                        "Koin menggunakan denominasi 1, 5, dan 10."
                    }
                },
                new()
                {
                    Heading = "B. Setup awal mode pemula",
                    Description = "Checklist setup meja permainan.",
                    Points = new List<string>
                    {
                        "Dirikan tent card tujuan permainan.",
                        "Susun papan 3 baris: poin (donasi, emas), penghasilan (kalender kerja, pesanan), pengeluaran (kebutuhan, bahan).",
                        "Letakkan kartu juara donasi dan juara dana pensiun pada papan donasi.",
                        "Letakkan kartu emas dan dek harga emas pada papan investasi.",
                        "Letakkan token Mr.Cashflowpoly pada kotak GO.",
                        "Buka 5 kartu pesanan, 5 kartu kebutuhan primer, dan 5 kartu bahan.",
                        "Aturan slot bahan: maksimum 2 kartu sejenis di slot terbuka; kartu sejenis ke-3 dibuang dan diganti.",
                        "Siapkan bank koin dan simpan komponen yang tidak dipakai."
                    }
                },
                new()
                {
                    Heading = "C. Setup awal mode mahir",
                    Description = "Tambahan setup meja dari mode pemula.",
                    Points = new List<string>
                    {
                        "Gunakan setup mode pemula sebagai dasar.",
                        "Tambahkan papan tabungan tujuan keuangan, papan risiko kehidupan, dan papan asuransi dan bank syariah.",
                        "Letakkan 5 kartu tujuan keuangan pada papan tabungan.",
                        "Kocok lalu letakkan dek risiko kehidupan pada papan risiko.",
                        "Letakkan kartu pinjaman syariah pada papan asuransi dan bank syariah."
                    }
                },
                new()
                {
                    Heading = "D. Setup pemain mode pemula",
                    Description = "Persiapan awal untuk setiap pemain.",
                    Points = new List<string>
                    {
                        "Tentukan pemain pertama dengan kartu tie breaker.",
                        "Bagikan layar pemain sesuai identitas kartu tie breaker.",
                        "Setiap pemain menerima 2 token aksi.",
                        "Kas awal pemain: 20 koin.",
                        "Bagikan acak 1 kartu bahan ke setiap pemain; pemain langsung membayar harga bahan ke bank.",
                        "Setiap pemain menerima 1 kartu emas awal.",
                        "Bagikan 1 kartu misi koleksi tertutup pada setiap pemain."
                    }
                },
                new()
                {
                    Heading = "E. Setup pemain mode mahir",
                    Description = "Penyesuaian setup pemain pada mode mahir.",
                    Points = new List<string>
                    {
                        "Gunakan setup pemain mode pemula sebagai dasar.",
                        "Kas awal pemain menjadi 10 koin.",
                        "Kartu tie breaker dibalik sebagai kartu asuransi aktif 1x gratis.",
                        "Setiap pemain menerima 1 kartu pinjaman syariah bernilai 10 koin."
                    }
                },
                new()
                {
                    Heading = "F. Cara bermain",
                    Description = "Alur satu putaran sampai permainan selesai.",
                    Points = new List<string>
                    {
                        "Setiap pemain menjalankan 2 aksi per giliran; aksi boleh sama atau berbeda.",
                        "Setelah semua pemain selesai, token Mr.Cashflowpoly maju 1 tanggal.",
                        "Semua pemain mengambil kembali 2 token aksi di akhir putaran.",
                        "Hari khusus: Jumat = peduli donasi, Sabtu = investasi emas, Minggu = libur.",
                        "Permainan selesai saat token melewati garis finish, lalu hitung poin kebahagiaan."
                    }
                },
                new()
                {
                    Heading = "G. Pilihan aksi mode pemula",
                    Description = "Empat aksi utama yang menghabiskan token aksi.",
                    Points = new List<string>
                    {
                        "Beli 1 kartu bahan: letakkan token aksi di papan bahan, bayar harga kartu, ambil kartu bahan.",
                        "Klaim 1 kartu pesanan: letakkan token di papan pesanan, serahkan bahan sesuai syarat, ambil kartu pesanan, terima penghasilan.",
                        "Beli 1 kartu kebutuhan: letakkan token di papan kebutuhan, beli primer lebih dulu sebelum sekunder/tersier, bayar harga kartu.",
                        "Kerja lepas 1x: letakkan token di papan kalender kerja, terima 1 koin dari bank.",
                        "Slot kosong papan diisi ulang setelah pemain menyelesaikan 2 aksi."
                    }
                },
                new()
                {
                    Heading = "H. Pilihan aksi mode mahir",
                    Description = "Aksi tambahan risiko, asuransi, tabungan, dan pinjaman.",
                    Points = new List<string>
                    {
                        "Ambil kartu risiko kehidupan saat melakukan aksi klaim pesanan, lalu jalankan instruksi kartu.",
                        "Jika kartu risiko menimbulkan biaya, pemain dapat pakai asuransi aktif atau membayar ke bank.",
                        "Jika koin tidak cukup, pemain dapat menjual kebutuhan (setengah harga), menjual emas (harga aktif), atau mengambil pinjaman syariah.",
                        "Aktifkan asuransi multi risiko: letakkan 1 token aksi di papan asuransi dan bank syariah, bayar premi 1 koin, lalu balik kartu tie breaker ke sisi asuransi.",
                        "Menabung tujuan keuangan: letakkan token aksi di papan tabungan, setor maksimum 15 koin per aksi, dan ambil kartu tujuan jika nominal terpenuhi.",
                        "Bayar atau pinjam syariah: gunakan 1 token aksi untuk melunasi atau mengambil pinjaman.",
                        "Pinjaman belum lunas di akhir permainan membuat kartu tujuan keuangan hangus dan terkena penalti poin."
                    }
                },
                new()
                {
                    Heading = "I. Aksi bebas (tanpa token aksi)",
                    Description = "Aksi khusus yang dipicu oleh hari kalender.",
                    Points = new List<string>
                    {
                        "Peduli donasi (Jumat): semua pemain donasi tertutup minimal 1 koin, buka bersamaan, lalu tentukan peringkat.",
                        "Poin donasi: juara 1 = 7 poin, juara 2 = 5 poin, juara 3 = 2 poin.",
                        "Jika donasi sama, gunakan angka tie breaker terbesar.",
                        "Investasi emas (Sabtu): buka kartu harga emas, lalu pemain bebas beli atau jual emas sesuai harga aktif.",
                        "Jumlah transaksi emas pada hari Sabtu tidak dibatasi per pemain."
                    }
                },
                new()
                {
                    Heading = "J. Catatan arus kas (opsional)",
                    Description = "Format sederhana pencatatan pemasukan dan pengeluaran.",
                    Points = new List<string>
                    {
                        "Isi tanggal permainan dan nama pemain.",
                        "Catat setiap pemasukan pada kolom (+) dan pengeluaran pada kolom (-).",
                        "Catat donasi Jumat setelah aksi donasi selesai.",
                        "Catat transaksi emas hari Sabtu: jual = (+), beli = (-).",
                        "Lakukan rekap akhir: saldo awal, total pemasukan, total pengeluaran, saldo akhir."
                    }
                },
                new()
                {
                    Heading = "K. Perhitungan poin kebahagiaan",
                    Description = "Komponen skor, penalti, dan aturan tie breaker.",
                    Points = new List<string>
                    {
                        "Jumlahkan seluruh poin kartu aneka kebutuhan.",
                        "Tambahkan bonus set kebutuhan: 3 jenis berbeda = +4 poin, 3 jenis sama = +2 poin.",
                        "Tambahkan poin juara donasi dan poin investasi emas.",
                        "Tambahkan poin juara dana pensiun berdasarkan sisa koin akhir.",
                        "Tambahkan poin kartu tujuan keuangan pada mode mahir hanya jika pinjaman lunas.",
                        "Kurangi penalti misi koleksi gagal sebesar 10 poin.",
                        "Kurangi penalti pinjaman syariah belum lunas sebesar 15 poin.",
                        "Jika total poin sama, pemenang ditentukan oleh angka tie breaker terbesar."
                    }
                },
                new()
                {
                    Heading = "L. Ucapan terima kasih",
                    Description = "Apresiasi kontributor.",
                    Points = new List<string>
                    {
                        "Daftar lengkap ucapan terima kasih mengikuti dokumen rulebook halaman 28.",
                        "Mencakup keluarga, mentor, komunitas, akademisi, dan kolaborator board game."
                    }
                },
                new()
                {
                    Heading = "M. Segera hadir",
                    Description = "Cashflowpoly: Investor Edition.",
                    Points = new List<string>
                    {
                        "Fokus tema: investor (deposito, obligasi, saham, mata uang kripto).",
                        "Tujuan: mendukung keuangan keluarga untuk mencapai kebahagiaan.",
                        "Video tutorial: Instagram @CASHFLOWPOLY.",
                        "Kontak kerja sama: adhicipta.playground@gmail.com."
                    }
                }
            },
            Scoring = new List<RulebookScoreItemViewModel>
            {
                new() { Category = "Poin Kebutuhan", Rule = "Jumlah poin seluruh kartu aneka kebutuhan." },
                new() { Category = "Bonus Set Kebutuhan", Rule = "3 jenis berbeda = +4 poin, 3 jenis sama = +2 poin." },
                new() { Category = "Poin Juara Donasi", Rule = "Akumulasi poin dari kartu juara donasi." },
                new() { Category = "Poin Investasi Emas", Rule = "1 emas = 3, 2 emas = 5, 3 emas = 8, 4 emas = 12." },
                new() { Category = "Poin Juara Dana Pensiun", Rule = "Juara 1 = +5, juara 2 = +3, juara 3 = +1." },
                new() { Category = "Poin Tujuan Keuangan", Rule = "Berlaku jika tidak ada pinjaman syariah yang tersisa." },
                new() { Category = "Minus Misi Koleksi", Rule = "Penalti tetap -10 poin jika target koleksi tidak terpenuhi." },
                new() { Category = "Minus Pinjaman Syariah", Rule = "Penalti tetap -15 poin untuk pinjaman yang belum lunas." }
            }
        };
    }
}
