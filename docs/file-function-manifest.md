# Manifest Fungsi File

Baseline manifest: 18 Juni 2026.

Dokumen ini merangkum fungsi file dan family file aktif pada repository.
File generated/build output (`bin/`, `obj/`, `TestResults/`) tidak
dimanifestkan. Entry untuk file UI yang sudah dihapus tidak dicantumkan lagi.

## Root, Konfigurasi, dan Infrastruktur
| Path | Kategori | Fungsi |
|---|---|---|
| `README.md` | Root | Entry point dokumentasi repository, setup, endpoint utama, dan tautan dokumen desain. |
| `Cashflowpoly.sln` | Root | Solution .NET untuk API, UI, dan test project. |
| `.gitattributes` | Root | Aturan atribut Git lintas platform. |
| `.gitignore` | Root | Daftar file/folder yang tidak dilacak Git. |
| `config/env/.env.example` | Konfigurasi | Template environment fallback/legacy. |
| `config/env/.env.dev.example` | Konfigurasi | Template environment development Docker Compose. |
| `config/env/.env.prod.example` | Konfigurasi | Template environment production. |
| `infra/docker/docker-compose.yml` | Infrastruktur | Definisi service dasar `db`, `api`, dan `ui`. |
| `infra/docker/docker-compose.watch.yml` | Infrastruktur | Override development/watch. |
| `infra/docker/docker-compose.prod.yml` | Infrastruktur | Override production, Nginx, dan Cloudflare Tunnel. |
| `infra/nginx/default.conf` | Infrastruktur | Reverse proxy Nginx untuk UI, API, Swagger, health, dan static asset. |
| `infra/cloudflared/config.yml` | Infrastruktur | Konfigurasi Cloudflare Tunnel. |

## Database dan Integrasi
| Path | Kategori | Fungsi |
|---|---|---|
| `database/00_create_schema.sql` | Database | DDL kanonis schema PostgreSQL event-first. |
| `database/01_seed_default_rulesets_components.sql` | Database | Seed ruleset default dan katalog komponen gameplay. |
| `database/02_seed_simulation_sessions_events.sql` | Database | Seed simulasi manual untuk sesi dan event contoh. |
| `postman/Cashflowpoly.postman_collection.json` | Integrasi | Collection Postman untuk smoke/API/RBAC flow. |
| `postman/Cashflowpoly.local.postman_environment.json` | Integrasi | Environment lokal Postman. |

## Dokumentasi
| Path | Kategori | Fungsi |
|---|---|---|
| `docs/00-ringkasan-rulebook-cashflowpoly.md` | Dokumen | Ringkasan rulebook Cashflowpoly untuk referensi cepat. |
| `docs/01-ringkasan-proposal-tugas-akhir.md` | Dokumen | Ringkasan proposal tugas akhir dan konteks penelitian. |
| `docs/00-Panduan/00-01-panduan-setup-lingkungan.md` | Panduan | Setup Windows, .NET, PostgreSQL, JWT, bootstrap DB, dan Tailwind. |
| `docs/00-Panduan/00-02-manual-pengguna-dan-skenario-operasional.md` | Panduan | Manual Instruktur/Player, alur IDN/API/Web, dan skenario operasional. |
| `docs/00-Panduan/00-03-panduan-menjalankan-sistem.md` | Panduan | Cara menjalankan API/UI, Docker Compose, Swagger, dan troubleshooting. |
| `docs/00-Panduan/00-04-status-kesesuaian-implementasi.md` | Panduan | Status kesesuaian implementasi terhadap spesifikasi aktif. |
| `docs/00-Panduan/00-05-panduan-deployment-production.md` | Panduan | Deployment production dengan Docker Compose, Nginx, dan Cloudflare Tunnel. |
| `docs/00-Panduan/00-06-matriks-alur-dan-hak-akses.md` | Panduan | Matriks alur pengguna, endpoint, UI route, RBAC, ruleset mutability, dan scope Player. |
| `docs/01-Spesifikasi/01-01-spesifikasi-kebutuhan-sistem.md` | Spesifikasi | SRS, kebutuhan fungsional/non-fungsional, data, dan aktor. |
| `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md` | Spesifikasi | Kontrak event, endpoint API, request/response, status code, dan RBAC teknis. |
| `docs/01-Spesifikasi/01-03-spesifikasi-ruleset-dan-validasi.md` | Spesifikasi | Definition ruleset, validasi, lifecycle versi, dan aturan mutability. |
| `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md` | Spesifikasi | Kontrak integrasi IDN, auth, retry, timeout, NFR, observability, dan audit. |
| `docs/01-Spesifikasi/01-05-kebutuhan-diagram-uml.md` | Spesifikasi | Kebutuhan UML dan daftar aktor/entitas/sequence yang harus digambar. |
| `docs/01-Spesifikasi/01-06-skenario-simulasi-permainan.md` | Spesifikasi | Skenario simulasi pemula/mahir dan pemetaan narasi ke event API. |
| `docs/02-Perancangan/02-01-rencana-implementasi-dan-struktur-solution-dotnet.md` | Perancangan | Struktur solution dan urutan implementasi. |
| `docs/02-Perancangan/02-02-rancangan-model-data-dan-basis-data.md` | Perancangan | Rancangan schema database aktual dan alur event-first. |
| `docs/02-Perancangan/02-03-definisi-metrik-dan-agregasi.md` | Perancangan | Definisi metric snapshot, rumus agregasi, dan validasi metrik. |
| `docs/02-Perancangan/02-04-metrik-gameplay-fisik-dan-turunan.md` | Perancangan | Variabel gameplay fisik dan metrik turunan. |
| `docs/02-Perancangan/02-05-rancangan-dashboard-analitika-mvc.md` | Perancangan | Rancangan dashboard MVC, route UI, endpoint mapping, dan error state. |
| `docs/02-Perancangan/02-06-spesifikasi-ui-mvc-dan-rancangan-viewmodel.md` | Perancangan | Struktur UI MVC, DTO/ViewModel, route, dan otorisasi UI. |
| `docs/02-Perancangan/02-07-normalisasi-schema-event-first.md` | Perancangan | Normalisasi schema event-first dan daftar reduksi tabel historis. |
| `docs/03-Pengujian/03-01-rencana-pengujian-fungsional-dan-validasi.md` | Pengujian | Rencana test API, integrasi event, UI, RBAC, smoke, dan evidence. |
| `docs/03-Pengujian/03-02-laporan-hasil-pengujian.md` | Pengujian | Laporan hasil pengujian baseline. |
| `docs/file-function-manifest.md` | Dokumen | Manifest fungsi file repository. |
| `docs/Img/RuleBook/*.png` | Aset dokumen | Scan/gambar halaman rulebook untuk lampiran dokumen. |

## API - Project dan Kontrak
| Path | Kategori | Fungsi |
|---|---|---|
| `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj` | API | Project file API .NET. |
| `src/Cashflowpoly.Api/appsettings.json` | API | Konfigurasi dasar API. |
| `src/Cashflowpoly.Api/appsettings.Development.json` | API | Konfigurasi development API. |
| `src/Cashflowpoly.Api/Cashflowpoly.Api.http` | API | Contoh request HTTP lokal. |
| `src/Cashflowpoly.Api/Program.cs` | API | Bootstrap dependency injection, middleware, auth, rate limit, OpenAPI, health, dan metrics. |
| `src/Cashflowpoly.Api/Dockerfile` | API | Build image production API. |
| `src/Cashflowpoly.Api/Dockerfile.dev` | API | Build image development API. |
| `src/Cashflowpoly.Api/Contracts/Dtos.cs` | API | DTO request/response API, termasuk session, players, events, analytics, auth, audit, dan state. |
| `src/Cashflowpoly.Api/Contracts/RulesetDefinitionDtos.cs` | API | DTO strongly typed untuk `definition` ruleset. |
| `src/Cashflowpoly.Api/Contracts/ErrorResponse.cs` | API | Format error standar. |
| `src/Cashflowpoly.Api/Properties/launchSettings.json` | API | Profil launch development. |
| `src/Cashflowpoly.Api/Properties/AssemblyInfo.cs` | API | Metadata assembly. |

## API - Controllers
| Path | Fungsi |
|---|---|
| `src/Cashflowpoly.Api/Controllers/AuthController.cs` | Login/register dan penerbitan JWT. |
| `src/Cashflowpoly.Api/Controllers/SessionsController.cs` | List/create/start/end session, read state, dan state-write disabled guard. |
| `src/Cashflowpoly.Api/Controllers/PlayersController.cs` | Create/list akun Player dan tambah Player ke sesi. |
| `src/Cashflowpoly.Api/Controllers/RulesetsController.cs` | CRUD ruleset, versi, activation, detail, components, defaults, dan sections. |
| `src/Cashflowpoly.Api/Controllers/EventsController.cs` | Ingest event tunggal/batch dan list event sesi. |
| `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` | Recompute, analytics sesi, transaksi, gameplay metrics, dan summary ruleset. |
| `src/Cashflowpoly.Api/Controllers/ObservabilityController.cs` | Summary observability yang menunjuk ke `/metrics`. |
| `src/Cashflowpoly.Api/Controllers/SecurityAuditController.cs` | Query audit log keamanan. |

## API - Data Access
| Path | Fungsi |
|---|---|
| `src/Cashflowpoly.Api/Data/AppDbContext.cs` | Mapping EF Core untuk schema PostgreSQL. |
| `src/Cashflowpoly.Api/Data/AppDbContextFactory.cs` | Factory design-time/runtime untuk DbContext. |
| `src/Cashflowpoly.Api/Data/DbRecords.cs` | Record/row model untuk repository. |
| `src/Cashflowpoly.Api/Data/UserRepository.cs` | Akses data akun, login lookup, create user, dan display name. |
| `src/Cashflowpoly.Api/Data/SessionRepository.cs` | Akses session, status, owner scope, dan list berdasarkan role. |
| `src/Cashflowpoly.Api/Data/PlayerRepository.cs` | Akses akun Player dan `session_participants`. |
| `src/Cashflowpoly.Api/Data/RulesetRepository.cs` | Persist/read ruleset, versi, components, defaults, dan definition normalization. |
| `src/Cashflowpoly.Api/Data/EventRepository.cs` | Persist/read event, idempotensi, sequence, dan projection dependencies. |
| `src/Cashflowpoly.Api/Data/EventActionIdResolver.cs` | Resolve `action_type` ke `ruleset_actions.ruleset_action_id`. |
| `src/Cashflowpoly.Api/Data/SessionEventProjector.cs` | Membangun projection session/participant dari event valid. |
| `src/Cashflowpoly.Api/Data/SessionStateRepository.cs` | Read/init state session dan final score projection. |
| `src/Cashflowpoly.Api/Data/MetricsRepository.cs` | Query/insert metric snapshots, gameplay JSON, transaksi, dan violations. |
| `src/Cashflowpoly.Api/Data/SecurityAuditRepository.cs` | Persist dan query audit keamanan. |

## API - Domain dan Services
| Path/Family | Fungsi |
|---|---|
| `src/Cashflowpoly.Api/Services/EventIngestionService.cs` | Pipeline ingest event, validasi, persist event, projection, dan error handling. |
| `src/Cashflowpoly.Api/Services/AnalyticsService.cs` | Recompute dan read model analytics. |
| `src/Cashflowpoly.Api/Services/I*.cs` | Interface service untuk DI/test. |
| `src/Cashflowpoly.Api/Domain/RulesetConfig.cs` | Runtime model ruleset untuk validasi dan gameplay rules. |
| `src/Cashflowpoly.Api/Domain/RulesetDefinitionMapper.cs` | Mapper `definition` DTO ke struktur domain runtime yang sudah dinormalisasi. |
| `src/Cashflowpoly.Api/Domain/GameActionCatalog.cs` | Katalog action gameplay yang dikenali. |
| `src/Cashflowpoly.Api/Domain/NeedTierClassifier.cs` | Klasifikasi tier kebutuhan. |
| `src/Cashflowpoly.Api/Domain/PlayerIdentityPolicy.cs` | Kebijakan validasi identitas Player dan scope request. |
| `src/Cashflowpoly.Api/Domain/SessionRules.cs` | Helper aturan status/lifecycle session. |
| `src/Cashflowpoly.Api/Domain/Event*Validator.cs` | Validator shape/domain event per area gameplay. |
| `src/Cashflowpoly.Api/Domain/Event*Calculator.cs` | Kalkulator state/projection event seperti balance, derived state, dan insurance offset. |
| `src/Cashflowpoly.Api/Domain/EventRecordMapper.cs` | Mapper request event ke record database. |
| `src/Cashflowpoly.Api/Domain/EventPayloadReader.cs` | Reader payload event yang aman terhadap tipe JSON. |
| `src/Cashflowpoly.Api/Domain/EventCashflowProjectionBuilder.cs` | Builder projection transaksi arus kas dari event. |
| `src/Cashflowpoly.Api/Domain/EventValidationDetailsSerializer.cs` | Serialisasi detail validasi ke format log/error. |
| `src/Cashflowpoly.Api/Domain/Analytics*Calculator.cs` | Kalkulator metrik analytics per domain. |
| `src/Cashflowpoly.Api/Domain/AnalyticsGameplaySnapshotBuilder.cs` | Builder response gameplay metrics. |
| `src/Cashflowpoly.Api/Domain/AnalyticsMetricSnapshotBuilder.cs` | Builder row `metric_snapshots`. |
| `src/Cashflowpoly.Api/Domain/Interfaces/*.cs` | Interface domain calculator/validator/mapper. |

## API - Infrastructure dan Security
| Path | Fungsi |
|---|---|
| `src/Cashflowpoly.Api/Infrastructure/ApiErrorHelper.cs` | Builder error response standar. |
| `src/Cashflowpoly.Api/Infrastructure/DatabaseInitialization.cs` | Bootstrap schema dan seed saat startup. |
| `src/Cashflowpoly.Api/Infrastructure/DatabaseHealthCheck.cs` | Health check database. |
| `src/Cashflowpoly.Api/Infrastructure/StandardResponseOperationFilter.cs` | OpenAPI operation filter untuk response standar. |
| `src/Cashflowpoly.Api/Infrastructure/Telemetry/AppMetrics.cs` | Definisi metrics OpenTelemetry/Prometheus. |
| `src/Cashflowpoly.Api/Security/AuthOptions.cs` | Opsi auth bootstrap. |
| `src/Cashflowpoly.Api/Security/JwtOptions.cs` | Opsi JWT dan signing keys. |
| `src/Cashflowpoly.Api/Security/JwtSigningKeyProvider.cs` | Provider signing key dan rotasi `kid`. |
| `src/Cashflowpoly.Api/Security/JwtTokenService.cs` | Pembuatan JWT. |
| `src/Cashflowpoly.Api/Security/RateLimitPolicyHelper.cs` | Policy rate limit API. |
| `src/Cashflowpoly.Api/Security/SecurityAuditService.cs` | Service audit keamanan. |
| `src/Cashflowpoly.Api/wwwroot/index.html` | Landing page API. |
| `src/Cashflowpoly.Api/wwwroot/api-assets/*` | Aset landing page API. |

## UI - Project, Controllers, dan Kontrak
| Path | Fungsi |
|---|---|
| `src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj` | Project file UI MVC. |
| `src/Cashflowpoly.Ui/appsettings.json` | Konfigurasi dasar UI. |
| `src/Cashflowpoly.Ui/appsettings.Development.json` | Konfigurasi development UI. |
| `src/Cashflowpoly.Ui/Program.cs` | Bootstrap MVC, session, localization, health, dan route mapping. |
| `src/Cashflowpoly.Ui/Dockerfile` | Build image production UI. |
| `src/Cashflowpoly.Ui/Dockerfile.dev` | Build image development UI. |
| `src/Cashflowpoly.Ui/Contracts/Dtos.cs` | DTO client untuk response/request API. |
| `src/Cashflowpoly.Ui/Contracts/RulesetDefinitionDtos.cs` | DTO `definition` ruleset pada UI. |
| `src/Cashflowpoly.Ui/Contracts/ErrorResponse.cs` | DTO error response API. |
| `src/Cashflowpoly.Ui/Controllers/AuthController.cs` | Login/register/logout UI. |
| `src/Cashflowpoly.Ui/Controllers/HomeController.cs` | Home, privacy, dan rulebook. |
| `src/Cashflowpoly.Ui/Controllers/LanguageController.cs` | Set preferensi bahasa. |
| `src/Cashflowpoly.Ui/Controllers/SessionsController.cs` | Daftar/detail sesi dan timeline partial. |
| `src/Cashflowpoly.Ui/Controllers/PlayersController.cs` | Detail Player pada sesi. |
| `src/Cashflowpoly.Ui/Controllers/PlayerDirectoryController.cs` | Direktori Player. |
| `src/Cashflowpoly.Ui/Controllers/RulesetsController.cs` | List/create/edit/detail/delete/activate ruleset dan default components. |
| `src/Cashflowpoly.Ui/Controllers/AnalyticsController.cs` | Route legacy analytics redirect. |

## UI - Domain, Infrastructure, Models, Views, Assets
| Path/Family | Fungsi |
|---|---|
| `src/Cashflowpoly.Ui/Domain/RulesetDefinitionMapper.cs` | Mapper definition ruleset untuk tampilan/form UI. |
| `src/Cashflowpoly.Ui/Infrastructure/ApiAuthHelper.cs` | Helper auth/error API. |
| `src/Cashflowpoly.Ui/Infrastructure/AuthSessionExtensions.cs` | Extension session untuk token/role/user display. |
| `src/Cashflowpoly.Ui/Infrastructure/BearerTokenHandler.cs` | Handler `HttpClient` untuk Bearer token. |
| `src/Cashflowpoly.Ui/Infrastructure/HttpContentExtensions.cs` | Helper baca content/error response. |
| `src/Cashflowpoly.Ui/Infrastructure/HttpsRedirectionPolicy.cs` | Policy redirect HTTPS UI. |
| `src/Cashflowpoly.Ui/Infrastructure/PlayerMetric*.cs` | Builder/mapper/formatter payload chart metric Player. |
| `src/Cashflowpoly.Ui/Infrastructure/RulebookContent.cs` | Konten rulebook yang dirender UI. |
| `src/Cashflowpoly.Ui/Infrastructure/RulesetFormHelper.cs` | Helper form create/edit ruleset dan error mapping. |
| `src/Cashflowpoly.Ui/Infrastructure/SessionTimelineMapper.cs` | Mapper event API ke timeline session UI. |
| `src/Cashflowpoly.Ui/Infrastructure/UiText*.cs` | Lexicon bilingual/label UI. |
| `src/Cashflowpoly.Ui/Models/*.cs` | ViewModel auth, analytics, home, rulebook, ruleset, dan error. |
| `src/Cashflowpoly.Ui/Views/Auth/*.cshtml` | View login/register. |
| `src/Cashflowpoly.Ui/Views/Home/*.cshtml` | View home, privacy, dan rulebook. |
| `src/Cashflowpoly.Ui/Views/Sessions/*.cshtml` | View daftar/detail sesi dan partial journey/timeline. |
| `src/Cashflowpoly.Ui/Views/Players/*.cshtml` | View direktori dan detail Player. |
| `src/Cashflowpoly.Ui/Views/Rulesets/*.cshtml` | View list/detail/create/edit ruleset dan komponen ruleset. |
| `src/Cashflowpoly.Ui/Views/Shared/*.cshtml` | Layout, error, dan shared view UI. |
| `src/Cashflowpoly.Ui/wwwroot/css/*` | CSS site dan Tailwind output/input. |
| `src/Cashflowpoly.Ui/wwwroot/js/site.js` | JavaScript dasar UI. |
| `src/Cashflowpoly.Ui/wwwroot/js/player-detail-charts.js` | JavaScript chart detail Player. |
| `src/Cashflowpoly.Ui/wwwroot/images/component/*.jpg` | Foto komponen permainan untuk halaman/detail ruleset. |
| `src/Cashflowpoly.Ui/wwwroot/images/rulebook/*.jpg` | Ilustrasi rulebook pada UI. |
| `src/Cashflowpoly.Ui/wwwroot/images/players/*.png` | Avatar Player. |
| `src/Cashflowpoly.Ui/package.json` | Script dan dependency frontend. |
| `src/Cashflowpoly.Ui/package-lock.json` | Lock dependency npm. |
| `src/Cashflowpoly.Ui/postcss.config.js` | Konfigurasi PostCSS. |
| `src/Cashflowpoly.Ui/tailwind.config.js` | Konfigurasi Tailwind. |

## Tests
| Path/Family | Fungsi |
|---|---|
| `tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj` | Project test API. |
| `tests/Cashflowpoly.Api.Tests/Infrastructure/*.cs` | Fixture Testcontainers dan WebApplicationFactory. |
| `tests/Cashflowpoly.Api.Tests/Auth*Tests.cs` | Test auth, JWT, response auth, dan RBAC. |
| `tests/Cashflowpoly.Api.Tests/Event*Tests.cs` | Test mapper, validator, projection, ingestion, dan analytics event. |
| `tests/Cashflowpoly.Api.Tests/Analytics*Tests.cs` | Test kalkulator analytics dan snapshot. |
| `tests/Cashflowpoly.Api.Tests/RulesetDefinitionMapperTests.cs` | Test mapping definition ruleset. |
| `tests/Cashflowpoly.Api.Tests/SessionStateApiIntegrationTests.cs` | Test API state session dan guard write-disabled. |
| `tests/Cashflowpoly.Api.Tests/DatabaseStartupIntegrationTests.cs` | Test bootstrap database. |
| `tests/Cashflowpoly.Api.Tests/ObservabilitySecurityIntegrationTests.cs` | Test observability/security audit. |
| `tests/Cashflowpoly.Api.Tests/*Asset*Tests.cs` | Test ketersediaan asset/API landing/deployment. |
| `tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj` | Project test UI. |
| `tests/Cashflowpoly.Ui.Tests/*Controller*Tests.cs` | Test controller UI. |
| `tests/Cashflowpoly.Ui.Tests/*Layout*Tests.cs` | Test struktur layout halaman UI. |
| `tests/Cashflowpoly.Ui.Tests/PlayerMetric*Tests.cs` | Test builder/mapper/formatter metric Player. |
| `tests/Cashflowpoly.Ui.Tests/Ruleset*Tests.cs` | Test form, detail, index, dan action ruleset. |
| `tests/Cashflowpoly.Ui.Tests/Session*Tests.cs` | Test halaman/session timeline/detail/list. |
| `tests/Cashflowpoly.Ui.Tests/UiText*Tests.cs` | Test struktur lexicon dan guard localization. |
