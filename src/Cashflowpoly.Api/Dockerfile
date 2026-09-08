# Fungsi file: Membangun dan menjalankan image production API .NET secara multi-stage.
# build
# Uraian baris: Memulai tahap image dari mcr.microsoft.com/dotnet/sdk:10.0.200 AS build. Alias tahap dapat dirujuk oleh COPY agar hanya hasil build/aset yang dipindahkan ke tahap berikutnya.
FROM mcr.microsoft.com/dotnet/sdk:10.0.200 AS build
# Uraian baris: Menetapkan direktori kerja /src di dalam image; jalur relatif pada COPY, RUN, dan proses berikutnya diinterpretasikan dari lokasi ini.
WORKDIR /src

# Uraian baris: Menyalin Cashflowpoly.sln ./. Menyediakan sumber atau aset pada tahap build sesuai jalur tujuan instruksi.
COPY Cashflowpoly.sln ./
# Uraian baris: Menyalin src/Cashflowpoly.Api/Cashflowpoly.Api.csproj src/Cashflowpoly.Api/. Definisi dependensi disalin lebih dahulu supaya layer restore/install dapat dipakai ulang ketika hanya kode aplikasi berubah.
COPY src/Cashflowpoly.Api/Cashflowpoly.Api.csproj src/Cashflowpoly.Api/
# Uraian baris: Memulihkan paket NuGet untuk proyek yang ditentukan; hasil restore dipakai compiler pada langkah publish berikutnya.
RUN dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj

# Uraian baris: Menyalin . .. Menyediakan sumber atau aset pada tahap build sesuai jalur tujuan instruksi.
COPY . .
# Uraian baris: Mengompilasi proyek dalam konfigurasi Release dan meletakkan hasil publish di /app/publish untuk disalin ke image runtime.
RUN dotnet publish src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release -o /app/publish

# runtime
# Uraian baris: Memulai tahap image dari mcr.microsoft.com/dotnet/aspnet:10.0.4 AS runtime. Alias tahap dapat dirujuk oleh COPY agar hanya hasil build/aset yang dipindahkan ke tahap berikutnya.
FROM mcr.microsoft.com/dotnet/aspnet:10.0.4 AS runtime
# Uraian baris: Menetapkan direktori kerja /app di dalam image; jalur relatif pada COPY, RUN, dan proses berikutnya diinterpretasikan dari lokasi ini.
WORKDIR /app
# Uraian baris: Bagian 1 dari 5 baris instruksi berikut. Memperbarui indeks paket Debian sebelum instalasi utilitas pada instruksi RUN yang sama; backslash meneruskan perintah ke baris sesudahnya.
# Uraian baris: Bagian 2 dari 5 baris instruksi berikut. Memasang curl untuk permintaan HTTP health check tanpa paket rekomendasi tambahan; operator && meneruskan eksekusi hanya jika langkah sebelumnya berhasil.
# Uraian baris: Bagian 3 dari 5 baris instruksi berikut. Menghapus indeks paket sementara setelah instalasi agar tidak ikut memperbesar image; path ini milik filesystem image saat build.
# Uraian baris: Bagian 4 dari 5 baris instruksi berikut. Membuat direktori yang disebutkan untuk output build, cache paket, atau kunci Data Protection. Opsi -p juga membuat direktori induk yang belum tersedia.
# Uraian baris: Bagian 5 dari 5 baris instruksi berikut. Memberikan kepemilikan direktori kerja/cache/kunci kepada app:app secara rekursif agar proses non-root dapat menulis ke lokasi yang diperlukan.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet
# Uraian baris: Menyalin --from=build --chown=app:app /app/publish .. Memindahkan hasil publish ke image runtime dan menetapkan kepemilikan berkas kepada pengguna app.
COPY --from=build --chown=app:app /app/publish .
# Uraian baris: Mengalihkan pengguna default container ke app; instruksi/proses selanjutnya berjalan dengan hak pengguna tersebut.
USER app
# Uraian baris: Mendokumentasikan port HTTP internal 8080 pada image; publikasi port host tetap ditentukan oleh Docker Compose atau docker run.
EXPOSE 8080
# Uraian baris: Menetapkan dotnet dan assembly Cashflowpoly.Api.dll sebagai proses utama container; argumen tambahan diteruskan kepada aplikasi saat dijalankan.
ENTRYPOINT ["dotnet", "Cashflowpoly.Api.dll"]
