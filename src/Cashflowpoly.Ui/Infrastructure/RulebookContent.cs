using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class RulebookContent
{
    public static RulebookPageViewModel Build()
    {
        return new RulebookPageViewModel
        {
            Title = "Cashflowpoly - Entrepreneur Edition",
            Subtitle = "Panduan permainan (versi ringkas terstruktur A-M)",
            Sections = new List<RulebookSectionViewModel>
            {
                new()
                {
                    Heading = "A. KOMPONEN Permainan",
                    Description = "Komponen inti dan ekspansi.",
                    Points = new List<string>
                    {
                        "Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.",
                        "Mode pemula: token Mr.Cashflowpoly, token aksi pemain, papan kalender kerja, papan pesanan, papan kebutuhan, papan bahan, papan donasi, papan investasi emas.",
                        "Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi, emas, kebutuhan, pesanan, bahan.",
                        "Komponen mode mahir: papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi dan bank syariah.",
                        "Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.",
                        "Koin: denominasi 1, 5, 10."
                    }
                },
                new()
                {
                    Heading = "B. Setup awal MODE PEMULA",
                    Description = "Susunan papan dan kartu awal.",
                    Points = new List<string>
                    {
                        "Dirikan tent card tujuan permainan.",
                        "Baris 1: papan juara peduli donasi, papan investasi emas.",
                        "Baris 2: papan kalender kerja, papan pesanan masakan.",
                        "Baris 3: papan aneka kebutuhan, papan bahan masakan.",
                        "Tempatkan kartu juara donasi dan juara dana pensiun pada papan donasi.",
                        "Tempatkan kartu emas dan dek harga emas pada papan investasi.",
                        "Letakkan token Mr.Cashflowpoly pada kotak GO.",
                        "Buka 5 kartu pesanan, 5 kartu kebutuhan primer, 5 kartu bahan.",
                        "Aturan slot bahan: maksimal 2 kartu bahan sejenis pada slot terbuka; kartu ketiga sejenis dibuang ke discard dan ganti.",
                        "Siapkan bank koin dan simpan komponen yang tidak dipakai."
                    }
                },
                new()
                {
                    Heading = "C. Setup awal MODE MAHIR",
                    Description = "Tambahan setup dari mode pemula.",
                    Points = new List<string>
                    {
                        "Gunakan setup mode pemula sebagai dasar.",
                        "Tambahkan papan tabungan tujuan keuangan pada baris poin.",
                        "Tambahkan papan risiko kehidupan pada baris penghasilan.",
                        "Tambahkan papan asuransi dan bank syariah pada baris pengeluaran.",
                        "Letakkan 5 kartu tujuan keuangan pada papan tabungan.",
                        "Kocok dan letakkan dek risiko kehidupan pada papan risiko.",
                        "Letakkan kartu pinjaman syariah pada papan asuransi dan bank syariah."
                    }
                },
                new()
                {
                    Heading = "D. Setup pemain MODE PEMULA",
                    Description = "Persiapan tiap pemain.",
                    Points = new List<string>
                    {
                        "Tentukan pemain pertama dengan kartu tie breaker.",
                        "Bagikan layar pemain sesuai identitas kartu tie breaker.",
                        "Setiap pemain menerima 2 token aksi.",
                        "Kas awal pemain: 20 koin.",
                        "Bagikan acak 1 kartu bahan ke tiap pemain; pemain membayar harga bahan ke bank.",
                        "Setiap pemain menerima 1 kartu emas awal.",
                        "Bagikan 1 kartu misi koleksi tertutup pada tiap pemain."
                    }
                },
                new()
                {
                    Heading = "E. Setup pemain MODE MAHIR",
                    Description = "Penyesuaian setup pemain mode mahir.",
                    Points = new List<string>
                    {
                        "Gunakan setup pemain mode pemula dengan tambahan mode mahir.",
                        "Kas awal pemain: 10 koin.",
                        "Kartu tie breaker dibalik sebagai kartu asuransi aktif 1x gratis.",
                        "Setiap pemain menerima 1 kartu pinjaman syariah bernilai 10 koin."
                    }
                },
                new()
                {
                    Heading = "F. Cara bermain",
                    Description = "Alur giliran dan akhir permainan.",
                    Points = new List<string>
                    {
                        "Giliran pemain: lakukan 2 aksi (boleh sama atau berbeda).",
                        "Setelah semua pemain selesai, token Mr.Cashflowpoly maju 1 tanggal (kecuali pola khusus pada kalender).",
                        "Pemain mengambil kembali 2 token aksi di akhir putaran.",
                        "Jumat: fase peduli donasi.",
                        "Sabtu: fase investasi emas.",
                        "Minggu: libur, lanjut hari berikutnya.",
                        "Permainan selesai saat token melewati garis finish, lanjut hitung poin kebahagiaan."
                    }
                },
                new()
                {
                    Heading = "G. Pilihan Aksi MODE PEMULA",
                    Description = "Empat aksi utama.",
                    Points = new List<string>
                    {
                        "Aksi beli 1 kartu bahan masakan: bayar ke bank sesuai harga kartu, patuhi batas maksimal kepemilikan bahan.",
                        "Aksi klaim 1 kartu pesanan masakan: serahkan bahan sesuai syarat, terima penghasilan dari bank.",
                        "Aksi beli 1 kartu aneka kebutuhan: wajib primer dulu sebelum sekunder/tersier, bayar ke bank sesuai harga.",
                        "Aksi kerja lepas 1x: terima 1 koin dari bank.",
                        "Slot kosong papan diisi ulang setelah pemain menyelesaikan 2 aksi."
                    }
                },
                new()
                {
                    Heading = "H. Pilihan Aksi MODE MAHIR",
                    Description = "Aksi tambahan mode mahir.",
                    Points = new List<string>
                    {
                        "Aksi risiko kehidupan: paket dengan aksi klaim pesanan pada mode mahir.",
                        "Dampak risiko: pemasukan atau pengeluaran sesuai kartu risiko.",
                        "Mitigasi risiko: gunakan asuransi aktif atau bayar biaya ke bank.",
                        "Jika koin tidak cukup: jual kebutuhan (setengah harga), jual emas (harga aktif), atau ambil pinjaman syariah.",
                        "Aktifkan asuransi: bayar 1 koin lalu balik kartu tie breaker ke sisi asuransi.",
                        "Menabung tujuan keuangan: setor maksimal 15 koin per aksi hingga mencapai harga kartu tujuan.",
                        "Bayar atau pinjam syariah: 1 aksi untuk pilih salah satu.",
                        "Pinjaman belum lunas di akhir: kartu tujuan keuangan hangus dan terkena minus poin."
                    }
                },
                new()
                {
                    Heading = "I. AKSI BEBAS (TANPA TOKEN AKSI)",
                    Description = "Aksi khusus hari kalender.",
                    Points = new List<string>
                    {
                        "Peduli donasi (Jumat): semua pemain donasi tertutup, buka bersama, tentukan juara donasi.",
                        "Poin juara donasi: juara 1 = 7 poin, juara 2 = 5 poin, juara 3 = 2 poin.",
                        "Tie donasi: gunakan angka terbesar kartu tie breaker.",
                        "Investasi emas (Sabtu): buka kartu harga emas, pemain bebas beli/jual emas sesuai harga aktif.",
                        "Investasi emas juga bisa muncul dari kartu risiko kehidupan."
                    }
                },
                new()
                {
                    Heading = "J. CATATAN ARUS KAS (OPSIONAL)",
                    Description = "Pencatatan transaksi selama permainan.",
                    Points = new List<string>
                    {
                        "Isi tanggal permainan dan nama pemain.",
                        "Catat setiap pemasukan (+) dan pengeluaran (-) per tanggal transaksi.",
                        "Donasi Jumat dicatat setelah aksi peduli donasi.",
                        "Investasi emas Sabtu: jual dicatat (+), beli dicatat (-).",
                        "Rekap akhir: saldo awal, total pemasukan, total pengeluaran, saldo akhir.",
                        "Jika lembar habis, unduh template dari profil Instagram @CASHFLOWPOLY."
                    }
                },
                new()
                {
                    Heading = "K. Perhitungan poin kebahagiaan",
                    Description = "Komponen skor dan penalti akhir.",
                    Points = new List<string>
                    {
                        "Siapkan lembar skor dan isi data sesi bermain.",
                        "Jumlahkan poin kebutuhan dari seluruh kartu aneka kebutuhan.",
                        "Tambahkan bonus set kebutuhan: 3 beda jenis = +4, 3 jenis sama = +2.",
                        "Tambahkan poin kartu juara peduli donasi.",
                        "Tambahkan poin investasi emas sesuai jumlah kartu emas.",
                        "Tambahkan poin juara dana pensiun berdasarkan sisa koin akhir.",
                        "Tambahkan poin kartu tujuan keuangan (mode mahir) jika pinjaman lunas.",
                        "Kurangi minus poin misi koleksi gagal.",
                        "Kurangi minus poin pinjaman syariah tidak lunas (mode mahir).",
                        "Tie total poin: gunakan angka terbesar kartu tie breaker."
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
                    Heading = "M. Segera hadir!",
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
                new() { Category = "Poin Juara Dana Pensiun", Rule = "Juara 1 = +5, Juara 2 = +3, Juara 3 = +1." },
                new() { Category = "Poin Tujuan Keuangan", Rule = "Berlaku jika tidak ada pinjaman syariah yang tersisa." },
                new() { Category = "Minus Misi Koleksi", Rule = "Diberikan jika target koleksi kebutuhan tidak terpenuhi." },
                new() { Category = "Minus Pinjaman Syariah", Rule = "Diberikan untuk kartu pinjaman yang belum lunas." }
            }
        };
    }
}
