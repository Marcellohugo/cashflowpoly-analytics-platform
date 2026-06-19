create extension if not exists pgcrypto;

begin;

-- ============================================================-- 1. MASTER Action, INGREDIENT, DAN KOMPONEN-- ============================================================
insert into actions (
        action_id,
        action_name,
        behavior_id,
        mode,
        cashflow_direction,
        affects_coin,
        affects_happiness,
        affects_saving,
        affects_inventory,
        is_active
    )
values
    (
        'BahanMasakan',
        'Beli Bahan Masakan',
        'BahanMasakan',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        true,
        true
    ),
    (
        'BuangBahanMasakan',
        'Buang Bahan Masakan',
        'BuangBahanMasakan',
        'BOTH',
        null,
        false,
        false,
        false,
        true,
        true
    ),
    (
        'JualMasakan',
        'Jual Masakan',
        'JualMasakan',
        'BOTH',
        'IN',
        true,
        false,
        false,
        true,
        true
    ),
    (
        'LewatiOrder',
        'Lewati Order',
        'LewatiOrder',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'Kebutuhan',
        'Beli Kebutuhan',
        'Kebutuhan',
        'BOTH',
        'OUT',
        true,
        true,
        false,
        false,
        true
    ),
    (
        'KerjaLepas',
        'Kerja Lepas',
        'KerjaLepas',
        'BOTH',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'CatatTransaksi',
        'Catat Transaksi',
        'CatatTransaksi',
        'BOTH',
        null,
        true,
        false,
        true,
        false,
        true
    ),
    (
        'Menabung',
        'Menabung',
        'Menabung',
        'BOTH',
        'OUT',
        false,
        false,
        true,
        false,
        true
    ),
    (
        'TarikTabungan',
        'Tarik Tabungan',
        'TarikTabungan',
        'BOTH',
        'IN',
        true,
        false,
        true,
        false,
        true
    ),
    (
        'TujuanFinansial',
        'Tujuan Finansial',
        'TujuanFinansial',
        'BOTH',
        'OUT',
        true,
        true,
        true,
        false,
        true
    ),
    (
        'JumatBerkah',
        'Peduli Donasi',
        'JumatBerkah',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'InvestasiEmas',
        'Investasi Emas',
        'InvestasiEmas',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'JualEmas',
        'Jual Emas',
        'JualEmas',
        'BOTH',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'LewatiTransaksiEmas',
        'Lewati Transaksi Emas',
        'LewatiTransaksiEmas',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'HariMingguLibur',
        'Hari Minggu Libur',
        'HariMingguLibur',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'PinjamanSyariah',
        'Pinjaman Syariah',
        'PinjamanSyariah',
        'MAHIR',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'BayarPinjaman',
        'Bayar Pinjaman',
        'BayarPinjaman',
        'MAHIR',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'Asuransi',
        'Asuransi',
        'Asuransi',
        'MAHIR',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'RisikoKehidupan',
        'Risiko Kehidupan',
        'RisikoKehidupan',
        'MAHIR',
        null,
        true,
        false,
        false,
        false,
        true
    ),
    (
        'GunakanOpsiDarurat',
        'Gunakan Opsi Darurat',
        'GunakanOpsiDarurat',
        'MAHIR',
        null,
        true,
        false,
        false,
        false,
        true
    ),
    (
        'PoinPeringkatDonasi',
        'Poin Peringkat Donasi',
        'PoinPeringkatDonasi',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'UmumkanJuaraDonasi',
        'Sistem: Umumkan Juara Donasi',
        'UmumkanJuaraDonasi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'PoinEmas',
        'Poin Emas',
        'PoinEmas',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'PoinPeringkatPensiun',
        'Poin Peringkat Pensiun',
        'PoinPeringkatPensiun',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'BagikanEmasAwal',
        'Sistem: Bagikan Emas Awal',
        'BagikanEmasAwal',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'BagikanTieBreaker',
        'Sistem: Bagikan Tie Breaker',
        'BagikanTieBreaker',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'BagikanMisiKoleksi',
        'Sistem: Bagikan Misi Koleksi',
        'BagikanMisiKoleksi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'AmbilKartuDariDeck',
        'Sistem: Ambil Kartu dari Deck',
        'AmbilKartuDariDeck',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'KartuDiambilDariPasar',
        'Sistem: Kartu Diambil dari Pasar',
        'KartuDiambilDariPasar',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'KartuMasukDiscard',
        'Sistem: Kartu Masuk Discard',
        'KartuMasukDiscard',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'IsiUlangPasar',
        'Sistem: Isi Ulang Pasar',
        'IsiUlangPasar',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'MulaiSesi',
        'Sistem: Mulai Sesi',
        'MulaiSesi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'AkhiriSesi',
        'Sistem: Akhiri Sesi',
        'AkhiriSesi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'AkhirGiliran',
        'Akhir Giliran',
        'AkhirGiliran',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ) on conflict (action_id) do update set action_name = excluded.action_name,
    behavior_id = excluded.behavior_id,
    mode = excluded.mode,
    cashflow_direction = excluded.cashflow_direction,
    affects_coin = excluded.affects_coin,
    affects_happiness = excluded.affects_happiness,
    affects_saving = excluded.affects_saving,
    affects_inventory = excluded.affects_inventory,
    is_active = excluded.is_active;

-- ============================================================-- 3. RULESET DEFAULT BERBASIS JS on INPUT LOKAL-- ============================================================
create temporary table seed_rulesets (
    ruleset_id uuid not null,
    ruleset_version_id uuid not null,
    mode varchar(10) not null,
    ruleset_name varchar(160) not null,
    ruleset_description text not null,
    definition_json jsonb not null
) on commit drop;

insert into seed_rulesets (
        ruleset_id,
        ruleset_version_id,
        mode,
        ruleset_name,
        ruleset_description,
        definition_json
    )
values
    (
        '2f4d94db-2a9f-4d4d-9a8a-53b58c598f71',
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524',
        'PEMULA',
        'Cashflowpoly Default - Mode Pemula',
        'Seed ruleset mode pemula dalam definition_json terpadu dan katalog generik.',
        $json$ { "mode": "PEMULA",
        "actions_per_turn": 2,
        "starting_cash": 20,
        "player_ordering": "PLAYER_ORDER",
        "weekday_rules": { "FRI": { "feature": "DONATION",
        "enabled": true },
        "SAT": { "feature": "GOLD_TRADE",
        "enabled": true },
        "SUN": { "feature": "REST",
        "enabled": true } },
        "constraints": { "cash_min": 0,
        "max_ingredient_total": 6,
        "max_same_ingredient": 3,
        "primary_need_max_per_day": 1,
        "require_primary_before_others": true },
        "donation": { "min_amount": 1,
        "max_amount": 999999 },
        "gold_trade": { "allow_buy": true,
        "allow_sell": true },
        "advanced": { "loan": { "enabled": false },
        "insurance": { "enabled": false },
        "saving_goal": { "enabled": false } },
        "freelance": { "income": 1 },
        "scoring": { "donation_rank_points": [{ "rank": 1, "points": 7 },{ "rank": 2, "points": 5 },{ "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 },{ "qty": 2, "points": 5 },{ "qty": 3, "points": 8 },{ "qty": 4, "points": 12 }],
        "pension_rank_points": [{ "rank": 1, "points": 5 },{ "rank": 2, "points": 3 },{ "rank": 3, "points": 1 }],
        "score_matrix": [{ "score_source": "DONATION", "rank": 1, "points": 7 },{ "score_source": "DONATION", "rank": 2, "points": 5 },{ "score_source": "DONATION", "rank": 3, "points": 2 },{ "score_source": "PENSION", "rank": 1, "points": 5 },{ "score_source": "PENSION", "rank": 2, "points": 3 },{ "score_source": "PENSION", "rank": 3, "points": 1 }] },
        "component_catalog": { "gameConfig": { "initialCoins": 20,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 25,
        "minPlayers": 2,
        "maxPlayers": 4 },
        "bahan": [{ "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1, "cardQty": 5 },{ "id": "sayur", "nama": "Sayur", "hargaBeli": 2, "cardQty": 5 },{ "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3, "cardQty": 5 },{ "id": "telur", "nama": "Telur", "hargaBeli": 4, "cardQty": 5 },{ "id": "daging", "nama": "Daging", "hargaBeli": 5, "cardQty": 5 }],
        "resep": [{ "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "cardQty": 5, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng",
        "nama": "nasi goreng",
        "hargaJual": 15,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur",
        "nama": "tahu campur",
        "hargaJual": 16,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "resep-sayur-bumbu",
        "nama": "sayur tahu tempe",
        "hargaJual": 12,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Sayur", "Tahu Tempe"] },
        { "id": "rawon",
        "nama": "rawon",
        "hargaJual": 24,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Daging", "Telur", "Tahu Tempe"] } ],
        "kebutuhan": [{ "id": "buku", "nama": "buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 4 },{ "id": "sepatu", "nama": "sepatu", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 2, "cardQty": 4 },{ "id": "boneka", "nama": "boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 3, "cardQty": 4 },{ "id": "gameboy", "nama": "gameboy", "tipe": "tersier", "hargaBeli": 8, "poinKebahagiaan": 4, "cardQty": 4 },{ "id": "hiburan", "nama": "hiburan", "tipe": "tersier", "hargaBeli": 5, "poinKebahagiaan": 2, "cardQty": 4 },{ "id": "jam", "nama": "jam", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 3, "cardQty": 5 }],
        "targetKebutuhan": [{"id": "misi_jam","nama": "jam","success_points": 0,"failure_points": -10,"penaltyPoints": 10,"kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "jam" }] },
        { "id": "misi_boneka",
        "nama": "boneka",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "boneka" }] },
        { "id": "misi_gameboy",
        "nama": "gameboy",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "gameboy" }] },
        { "id": "misi_hiburan",
        "nama": "hiburan",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "hiburan" }] } ],
        "tujuanFinansial": [{ "id": "beli_rumah", "nama": "beli rumah", "hargaBeli": 12, "poinKebahagiaan": 6 },{ "id": "beli_motor", "nama": "beli motor", "hargaBeli": 8, "poinKebahagiaan": 4 },{ "id": "dana_darurat", "nama": "dana darurat", "hargaBeli": 6, "poinKebahagiaan": 3 },{ "id": "biaya_kursus", "nama": "biaya kursus", "hargaBeli": 10, "poinKebahagiaan": 5 },{ "id": "modal_usaha", "nama": "modal usaha", "hargaBeli": 15, "poinKebahagiaan": 8 }],
        "narasi": [{"id": "jual_pertama","nama": "jual_pertama","teks": ["Penjualan pertama membuka kepercayaan diri.","Momentum baik harus dijaga."],
        "prerequisiteAksi": [{ "aksi": "JualMasakan", "value": 1 }] } ] } } $json$ :: jsonb
    ),
    (
        'a68f53f9-92a2-446f-9f62-5a4f502a0199',
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d',
        'MAHIR',
        'Cashflowpoly Default - Mode Mahir',
        'Seed ruleset mode mahir dalam definition_json terpadu dan katalog generik.',
        $json$ { "mode": "MAHIR",
        "actions_per_turn": 2,
        "starting_cash": 10,
        "player_ordering": "PLAYER_ORDER",
        "weekday_rules": { "friday": { "feature": "DONATION",
        "enabled": true },
        "saturday": { "feature": "GOLD_TRADE",
        "enabled": true },
        "sunday": { "feature": "REST",
        "enabled": true } },
        "constraints": { "cash_min": 0,
        "max_ingredient_total": 6,
        "max_same_ingredient": 3,
        "primary_need_max_per_day": 1,
        "require_primary_before_others": true },
        "donation": { "min_amount": 1,
        "max_amount": 999999 },
        "gold_trade": { "allow_buy": true,
        "allow_sell": true },
        "advanced": { "loan": { "enabled": true },
        "insurance": { "enabled": true },
        "saving_goal": { "enabled": true } },
        "freelance": { "income": 1 },
        "scoring": { "donation_rank_points": [{ "rank": 1, "points": 7 },{ "rank": 2, "points": 5 },{ "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 },{ "qty": 2, "points": 5 },{ "qty": 3, "points": 8 },{ "qty": 4, "points": 12 }],
        "pension_rank_points": [{ "rank": 1, "points": 5 },{ "rank": 2, "points": 3 },{ "rank": 3, "points": 1 }],
        "score_matrix": [{ "score_source": "DONATION", "rank": 1, "points": 7 },{ "score_source": "DONATION", "rank": 2, "points": 5 },{ "score_source": "DONATION", "rank": 3, "points": 2 },{ "score_source": "PENSION", "rank": 1, "points": 5 },{ "score_source": "PENSION", "rank": 2, "points": 3 },{ "score_source": "PENSION", "rank": 3, "points": 1 }] },
        "component_catalog": { "gameConfig": { "initialCoins": 10,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 25,
        "minPlayers": 2,
        "maxPlayers": 4 },
        "bahan": [{ "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1, "cardQty": 5 },{ "id": "sayur", "nama": "Sayur", "hargaBeli": 2, "cardQty": 5 },{ "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3, "cardQty": 5 },{ "id": "telur", "nama": "Telur", "hargaBeli": 4, "cardQty": 5 },{ "id": "daging", "nama": "Daging", "hargaBeli": 5, "cardQty": 5 }],
        "resep": [{ "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "cardQty": 5, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng",
        "nama": "nasi goreng",
        "hargaJual": 15,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur",
        "nama": "tahu campur",
        "hargaJual": 16,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "resep-sayur-bumbu",
        "nama": "sayur tahu tempe",
        "hargaJual": 12,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Sayur", "Tahu Tempe"] },
        { "id": "rawon",
        "nama": "rawon",
        "hargaJual": 24,
        "poinKebahagiaan": 0,
        "cardQty": 5,
        "bahan": ["Daging", "Telur", "Tahu Tempe"] } ],
        "kebutuhan": [{ "id": "buku", "nama": "buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 4 },{ "id": "sepatu", "nama": "sepatu", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 2, "cardQty": 4 },{ "id": "boneka", "nama": "boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 3, "cardQty": 4 },{ "id": "gameboy", "nama": "gameboy", "tipe": "tersier", "hargaBeli": 8, "poinKebahagiaan": 4, "cardQty": 4 },{ "id": "hiburan", "nama": "hiburan", "tipe": "tersier", "hargaBeli": 5, "poinKebahagiaan": 2, "cardQty": 4 },{ "id": "jam", "nama": "jam", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 3, "cardQty": 5 }],
        "targetKebutuhan": [{"id": "misi_jam","nama": "jam","success_points": 0,"failure_points": -10,"penaltyPoints": 10,"kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "jam" }] },
        { "id": "misi_boneka",
        "nama": "boneka",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "boneka" }] },
        { "id": "misi_gameboy",
        "nama": "gameboy",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "gameboy" }] },
        { "id": "misi_hiburan",
        "nama": "hiburan",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" },{ "order": 2, "type": "TIER", "value": "sekunder" },{ "order": 3, "type": "NAME", "value": "hiburan" }] } ],
        "tujuanFinansial": [{ "id": "beli_rumah", "nama": "beli rumah", "hargaBeli": 12, "poinKebahagiaan": 6 },{ "id": "beli_motor", "nama": "beli motor", "hargaBeli": 8, "poinKebahagiaan": 4 },{ "id": "dana_darurat", "nama": "dana darurat", "hargaBeli": 6, "poinKebahagiaan": 3 },{ "id": "biaya_kursus", "nama": "biaya kursus", "hargaBeli": 10, "poinKebahagiaan": 5 },{ "id": "modal_usaha", "nama": "modal usaha", "hargaBeli": 15, "poinKebahagiaan": 8 }],
        "narasi": [{"id": "jual_pertama","nama": "jual_pertama","teks": ["Penjualan pertama membuka kepercayaan diri.","Momentum baik harus dijaga."],
        "prerequisiteAksi": [{ "aksi": "JualMasakan", "value": 1 }] } ] } } $json$ :: jsonb
    );

insert into rulesets (
        ruleset_id,
        name,
        description,
        instructor_user_id,
        created_at,
        created_by_user_id
    ) select ruleset_id,
    ruleset_name,
    ruleset_description,
    null,
    now(),
    null :: uuid from seed_rulesets on conflict (ruleset_id) do update set name = excluded.name,
    description = excluded.description;

insert into ruleset_versions (
        ruleset_version_id,
        ruleset_id,
        version,
        status,
        mode,
        schema_version,
        config_hash,
        change_note,
        published_at,
        created_at,
        created_by_user_id
    ) select ruleset_version_id,
    ruleset_id,
    1,
    'ACTIVE',
    mode,
    '3.0.0',
    encode(digest(definition_json :: text, 'sha256'), 'hex'),
    'Canonical relational catalog seed',
    now(),
    now(),
    null :: uuid from seed_rulesets on conflict (ruleset_id, version) do update set status = excluded.status,
    mode = excluded.mode,
    schema_version = excluded.schema_version,
    config_hash = excluded.config_hash,
    change_note = excluded.change_note,
    published_at = excluded.published_at;

insert into ruleset_game_settings (
        ruleset_version_id,
        starting_cash,
        starting_happiness,
        starting_saving,
        actions_per_turn,
        finish_day,
        min_players,
        max_players,
        cash_min,
        max_ingredient_total,
        max_same_ingredient,
        primary_need_max_per_day,
        require_primary_before_others,
        donation_min_amount,
        donation_max_amount,
        gold_trade_allow_buy,
        gold_trade_allow_sell,
        loan_enabled,
        insurance_enabled,
        saving_goal_enabled,
        freelance_income
    ) select sr.ruleset_version_id,
    coalesce(
        (sr.definition_json ->> 'starting_cash') :: int,
        0
    ),
    coalesce(
        (
            sr.definition_json #>>'{component_catalog,gameConfig,initialHappiness}')::int, 0),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,initialSaving}')::int, 0),coalesce((sr.definition_json->>'actions_per_turn')::int, 2),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,finishDay}')::int, 25),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,minPlayers}')::int, 2),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,maxPlayers}')::int, 4),coalesce((sr.definition_json#>>'{constraints,cash_min}')::int, 0),coalesce((sr.definition_json#>>'{constraints,max_ingredient_total}')::int, 0),coalesce((sr.definition_json#>>'{constraints,max_same_ingredient}')::int, 0),coalesce((sr.definition_json#>>'{constraints,primary_need_max_per_day}')::int, 0),coalesce((sr.definition_json#>>'{constraints,require_primary_before_others}')::boolean, true),coalesce((sr.definition_json#>>'{donation,min_amount}')::int, 1),coalesce((sr.definition_json#>>'{donation,max_amount}')::int, 1),coalesce((sr.definition_json#>>'{gold_trade,allow_buy}')::boolean, true),coalesce((sr.definition_json#>>'{gold_trade,allow_sell}')::boolean, true),coalesce((sr.definition_json#>>'{advanced,loan,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{advanced,insurance,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{advanced,saving_goal,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{freelance,income}')::int, 1) from seed_rulesets sr on conflict (ruleset_version_id) do update set starting_cash = excluded.starting_cash,starting_happiness = excluded.starting_happiness,starting_saving = excluded.starting_saving,actions_per_turn = excluded.actions_per_turn,finish_day = excluded.finish_day,min_players = excluded.min_players,max_players = excluded.max_players,cash_min = excluded.cash_min,max_ingredient_total = excluded.max_ingredient_total,max_same_ingredient = excluded.max_same_ingredient,primary_need_max_per_day = excluded.primary_need_max_per_day,require_primary_before_others = excluded.require_primary_before_others,donation_min_amount = excluded.donation_min_amount,donation_max_amount = excluded.donation_max_amount,gold_trade_allow_buy = excluded.gold_trade_allow_buy,gold_trade_allow_sell = excluded.gold_trade_allow_sell,loan_enabled = excluded.loan_enabled,insurance_enabled = excluded.insurance_enabled,saving_goal_enabled = excluded.saving_goal_enabled,freelance_income = excluded.freelance_income,updated_at = now();
            insert into ruleset_player_ordering_rules (
                    ruleset_version_id,
                    sort_order,
                    ordering_code,
                    weekday_code,
                    feature_code,
                    is_enabled
                ) select sr.ruleset_version_id,
                1,
                coalesce(
                    sr.definition_json ->> 'player_ordering',
                    'PLAYER_ORDER'
                ),
                null,
                null,
                true from seed_rulesets sr on conflict (ruleset_version_id, sort_order) do update set ordering_code = excluded.ordering_code,
                weekday_code = excluded.weekday_code,
                feature_code = excluded.feature_code,
                is_enabled = excluded.is_enabled;

insert into ruleset_player_ordering_rules (
        ruleset_version_id,
        sort_order,
        ordering_code,
        weekday_code,
        feature_code,
        is_enabled
    ) select sr.ruleset_version_id,
    mapped.sort_order,
    null,
    mapped.weekday_code,
    mapped.feature_code,
    mapped.is_enabled from seed_rulesets sr cross
    join lateral (
        values
            (
                10,
                'FRI' :: varchar(8),
                coalesce(
                    sr.definition_json #>>'{weekday_rules,friday,feature}', sr.definition_json#>>'{weekday_rules,FRI,feature}', 'DONATION')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,friday,enabled}', sr.definition_json#>>'{weekday_rules,FRI,enabled}', 'true')::boolean),(20, 'SAT'::varchar(8), coalesce(sr.definition_json#>>'{weekday_rules,saturday,feature}', sr.definition_json#>>'{weekday_rules,SAT,feature}', 'GOLD_TRADE')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,saturday,enabled}', sr.definition_json#>>'{weekday_rules,SAT,enabled}', 'true')::boolean),(30, 'SUN'::varchar(8), coalesce(sr.definition_json#>>'{weekday_rules,sunday,feature}', sr.definition_json#>>'{weekday_rules,SUN,feature}', 'REST')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,sunday,enabled}', sr.definition_json#>>'{weekday_rules,SUN,enabled}', 'true')::boolean)) mapped(sort_order, weekday_code, feature_code, is_enabled) on conflict (ruleset_version_id, sort_order) do update set ordering_code = excluded.ordering_code,weekday_code = excluded.weekday_code,feature_code = excluded.feature_code,is_enabled = excluded.is_enabled;
                    insert into ruleset_game_assets (
                            ruleset_version_id,
                            asset_type,
                            asset_code,
                            display_name,
                            sort_order,
                            is_active,
                            metadata_json
                        ) with json_assets as (
                            select
                                sr.ruleset_version_id,
                                'INGREDIENT' :: varchar(40) as asset_type,
                                coalesce(
                                    item ->> 'id',
                                    regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
                                ) as asset_code,
                                item ->> 'nama' as display_name,
                                ord :: int as sort_order,
                                true as is_active,
                                jsonb_build_object('source', 'component_catalog.bahan') as metadata_json from seed_rulesets sr cross
                                join lateral jsonb_array_elements(
                                    coalesce(
                                        sr.definition_json -> 'component_catalog' -> 'bahan',
                                        '[]' :: jsonb
                                    )
                                ) with ordinality as x(item, ord)
                            union
                            all select sr.ruleset_version_id,
                            'ORDER' :: varchar(40),
                            item ->> 'id',
                            item ->> 'nama',
                            (100 + ord) :: int,
                            true,
                            jsonb_build_object('source', 'component_catalog.resep')
                            from
                                seed_rulesets sr cross
                                join lateral jsonb_array_elements(
                                    coalesce(
                                        sr.definition_json -> 'component_catalog' -> 'resep',
                                        '[]' :: jsonb
                                    )
                                ) with ordinality as x(item, ord)
                            union
                            all select sr.ruleset_version_id,
                            'NEED' :: varchar(40),
                            item ->> 'id',
                            item ->> 'nama',
                            (200 + ord) :: int,
                            true,
                            jsonb_build_object('source', 'component_catalog.kebutuhan')
                            from
                                seed_rulesets sr cross
                                join lateral jsonb_array_elements(
                                    coalesce(
                                        sr.definition_json -> 'component_catalog' -> 'kebutuhan',
                                        '[]' :: jsonb
                                    )
                                ) with ordinality as x(item, ord)
                            union
                            all select sr.ruleset_version_id,
                            'GOLD' :: varchar(40),
                            concat(
                                'gold_card',
                                coalesce((item ->> 'qty') :: int, ord :: int)
                            ),
                            concat(
                                'Emas ',
                                coalesce((item ->> 'qty') :: int, ord :: int)
                            ),
                            (700 + ord) :: int,
                            true,
                            jsonb_build_object('source', 'scoring.gold_points_by_qty')
                            from
                                seed_rulesets sr cross
                                join lateral jsonb_array_elements(
                                    coalesce(
                                        sr.definition_json -> 'scoring' -> 'gold_points_by_qty',
                                        '[]' :: jsonb
                                    )
                                ) with ordinality as x(item, ord)
                        ),
                        static_assets as (
                            select
                                *
                            from
                                (
                                    values
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE' :: varchar(40),
                                            'gold_price_1' :: varchar(120),
                                            'Harga Emas 1' :: varchar(160),
                                            801,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_2',
                                            'Harga Emas 2',
                                            802,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_3',
                                            'Harga Emas 3',
                                            803,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_4',
                                            'Harga Emas 4',
                                            804,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_5',
                                            'Harga Emas 5',
                                            805,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_6',
                                            'Harga Emas 6',
                                            806,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_1',
                                            'Harga Emas 1',
                                            801,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_2',
                                            'Harga Emas 2',
                                            802,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_3',
                                            'Harga Emas 3',
                                            803,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_4',
                                            'Harga Emas 4',
                                            804,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_5',
                                            'Harga Emas 5',
                                            805,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'GOLD_PRICE',
                                            'gold_price_6',
                                            'Harga Emas 6',
                                            806,
                                            true,
                                            '{"source":"static.gold_price"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_1',
                                            'Tie Breaker 1',
                                            901,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_2',
                                            'Tie Breaker 2',
                                            902,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_3',
                                            'Tie Breaker 3',
                                            903,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_4',
                                            'Tie Breaker 4',
                                            904,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_1',
                                            'Tie Breaker 1',
                                            901,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_2',
                                            'Tie Breaker 2',
                                            902,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_3',
                                            'Tie Breaker 3',
                                            903,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'TIE_BREAKER',
                                            'tie_breaker_4',
                                            'Tie Breaker 4',
                                            904,
                                            true,
                                            '{"source":"static.tie_breaker"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_tunjangan',
                                            'Bonus Tunjangan',
                                            1201,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_biaya_perbaikan',
                                            'Biaya Perbaikan',
                                            1202,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_kompetisi',
                                            'Bonus Kompetisi',
                                            1203,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_tetangga',
                                            'Bonus Tetangga',
                                            1204,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bayar_obat',
                                            'Bayar Obat',
                                            1205,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_proyek',
                                            'Bonus Proyek',
                                            1206,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_sekolah_adik',
                                            'Biaya Sekolah Adik',
                                            1207,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_motor_bocor',
                                            'Motor Bocor',
                                            1208,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_uang_saku_hilang',
                                            'Uang Saku Hilang',
                                            1209,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_tagihan_medis',
                                            'Tagihan Medis',
                                            1210,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_biaya_perjalanan',
                                            'Biaya Perjalanan',
                                            1211,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_peralatan_rusak',
                                            'Peralatan Rusak',
                                            1212,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_kebutuhan_keluarga',
                                            'Kebutuhan Keluarga',
                                            1213,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_servis_sepeda',
                                            'Servis Sepeda',
                                            1214,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_biaya_listrik',
                                            'Biaya Listrik',
                                            1215,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_biaya_kesehatan',
                                            'Biaya Kesehatan',
                                            1216,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_uang_kas',
                                            'Uang Kas Berkurang',
                                            1217,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_biaya_transport',
                                            'Biaya Transport',
                                            1218,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_tagihan_air',
                                            'Tagihan Air',
                                            1219,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_perbaikan_atap',
                                            'Perbaikan Atap',
                                            1220,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_hospital',
                                            'Biaya Rumah Sakit',
                                            1221,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_tabungan',
                                            'Bonus Tabungan',
                                            1222,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_tagihan_internet',
                                            'Tagihan Internet',
                                            1223,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        ),
                                        (
                                            '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                                            'RISK',
                                            'risk_bonus_usaha',
                                            'Bonus Usaha',
                                            1224,
                                            true,
                                            '{"source":"static.risk"}' :: jsonb
                                        )
                                ) as v(
                                    ruleset_version_id,
                                    asset_type,
                                    asset_code,
                                    display_name,
                                    sort_order,
                                    is_active,
                                    metadata_json
                                )
                        )
                    select
                        ruleset_version_id,
                        asset_type,
                        asset_code,
                        display_name,
                        sort_order,
                        is_active,
                        metadata_json from json_assets union all select ruleset_version_id,
                        asset_type,
                        asset_code,
                        display_name,
                        sort_order,
                        is_active,
                        metadata_json from static_assets on conflict (ruleset_version_id, asset_type, asset_code) do update set display_name = excluded.display_name,
                        sort_order = excluded.sort_order,
                        is_active = excluded.is_active,
                        metadata_json = excluded.metadata_json,
                        updated_at = now();

insert into ruleset_orders (
        ruleset_version_id,
        ruleset_game_asset_id,
        order_code,
        item_name,
        sell_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'hargaJual') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 1),
    true,
    jsonb_build_object(
        'hargaJual',
        coalesce((item ->> 'hargaJual') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'resep'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id and rga.asset_type = 'ORDER'
    and rga.asset_code = item ->> 'id' on conflict (ruleset_version_id, order_code) do update set item_name = excluded.item_name,
    sell_price = excluded.sell_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_actions (
        ruleset_version_id,
        action_id,
        behavior_id,
        sort_order,
        is_active
    ) select sr.ruleset_version_id,
    a.action_id,
    a.behavior_id,
    row_number() over (
        partition by sr.ruleset_version_id
        order by
            a.action_id
    ) :: int,
    a.is_active from seed_rulesets sr join actions a on a.is_active and a.mode in ('BOTH', sr.mode) on conflict (ruleset_version_id, action_id) do update set sort_order = excluded.sort_order,
    behavior_id = excluded.behavior_id,
    is_active = excluded.is_active;

insert into ruleset_ingredients (
        ruleset_version_id,
        ruleset_game_asset_id,
        ingredient_code,
        item_name,
        display_name,
        purchase_price,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    coalesce(
        item ->> 'id',
        regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
    ),
    item ->> 'nama',
    item ->> 'nama',
    coalesce((item ->> 'hargaBeli') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 5),
    true,
    jsonb_build_object(
        'asset_code',
        coalesce(
            item ->> 'id',
            regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
        ),
        'display_name',
        item ->> 'nama',
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'bahan'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id and rga.asset_type = 'INGREDIENT'
    and rga.asset_code = coalesce(
        item ->> 'id',
        regexp_replace(lower(item ->> 'nama'), '\s+', '_', 'g')
    ) on conflict (ruleset_version_id, ingredient_code) do update set item_name = excluded.item_name,
    display_name = excluded.display_name,
    purchase_price = excluded.purchase_price,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_order_requirements (
        ruleset_version_id,
        ruleset_order_id,
        requirement_order,
        required_asset_id,
        qty_required,
        payload_json
    ) select sr.ruleset_version_id,
    ro.ruleset_order_id,
    row_number() over (
        partition by sr.ruleset_version_id,
        recipe.recipe ->> 'id'
        order by
            req.ingredient_name
    ) :: int,
    ri.ruleset_game_asset_id,
    req.ingredient_qty,
    jsonb_build_object('ingredient_name', req.ingredient_name)
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'resep'
    ) as recipe(recipe)
    cross join lateral (
        select
            ingredient_name,
            count(*) :: int as ingredient_qty from jsonb_array_elements_text(recipe.recipe -> 'bahan') as z(ingredient_name)
        group by
            ingredient_name
    ) req join ruleset_orders ro on ro.ruleset_version_id = sr.ruleset_version_id and ro.order_code = recipe.recipe ->> 'id'
    join ruleset_ingredients ri on ri.ruleset_version_id = sr.ruleset_version_id and lower(ri.display_name) = lower(req.ingredient_name) on conflict (
        ruleset_order_id,
        requirement_order,
        required_asset_id
    ) do update set qty_required = excluded.qty_required,
    payload_json = excluded.payload_json;

insert into ruleset_needs (
        ruleset_version_id,
        ruleset_game_asset_id,
        need_code,
        item_name,
        need_tier,
        purchase_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce(item ->> 'tipe', 'primer'),
    coalesce((item ->> 'hargaBeli') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 1),
    true,
    jsonb_build_object(
        'tipe',
        coalesce(item ->> 'tipe', 'primer'),
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'kebutuhan'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id and rga.asset_type = 'NEED'
    and rga.asset_code = item ->> 'id' on conflict (ruleset_version_id, need_code) do update set item_name = excluded.item_name,
    need_tier = excluded.need_tier,
    purchase_price = excluded.purchase_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_need_set_bonuses (
        ruleset_version_id,
        pattern_code,
        required_count,
        points,
        sort_order,
        payload_json
    ) select sr.ruleset_version_id,
    bonus.pattern_code,
    bonus.required_count,
    bonus.points,
    bonus.sort_order,
    jsonb_build_object(
        'pattern',
        bonus.pattern_code,
        'required_count',
        bonus.required_count
    )
from
    seed_rulesets sr cross
    join (
        values
            ('THREE_DIFFERENT' :: varchar(40), 3, 4, 1),
            ('THREE_SAME' :: varchar(40), 3, 2, 2)
    ) as bonus(pattern_code, required_count, points, sort_order) on conflict (ruleset_version_id, pattern_code) do update set required_count = excluded.required_count,
    points = excluded.points,
    sort_order = excluded.sort_order,
    payload_json = excluded.payload_json;

insert into ruleset_collection_missions (
        ruleset_version_id,
        mission_code,
        item_name,
        success_points,
        failure_points,
        penalty_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'success_points') :: int, 0),
    coalesce((item ->> 'failure_points') :: int, 0),
    coalesce((item ->> 'penaltyPoints') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'success_points',
        coalesce((item ->> 'success_points') :: int, 0),
        'failure_points',
        coalesce((item ->> 'failure_points') :: int, 0),
        'penaltyPoints',
        coalesce((item ->> 'penaltyPoints') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'targetKebutuhan'
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, mission_code) do update set item_name = excluded.item_name,
    success_points = excluded.success_points,
    failure_points = excluded.failure_points,
    penalty_points = excluded.penalty_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_collection_mission_requirements (
        ruleset_version_id,
        ruleset_collection_mission_id,
        requirement_order,
        requirement_type,
        required_asset_id,
        required_need_tier,
        qty_required,
        payload_json
    ) select sr.ruleset_version_id,
    rcm.ruleset_collection_mission_id,
    coalesce((rule ->> 'order') :: int, ord :: int),
    case when upper(rule ->> 'type') = 'TIER' then 'NEED_TIER'
    else 'ASSET'
end,
case when upper(rule ->> 'type') = 'TIER' then null else required_asset.ruleset_game_asset_id end,
case when upper(rule ->> 'type') = 'TIER' then lower(rule ->> 'value')
else null end,
null,
jsonb_build_object(
    'type',
    rule ->> 'type',
    'value',
    rule ->> 'value'
)
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'targetKebutuhan'
    ) as mission(mission)
    cross join lateral jsonb_array_elements(
        coalesce(
            mission.mission -> 'kebutuhanTarget',
            '[]' :: jsonb
        )
    ) with ordinality as x(rule, ord)
    join ruleset_collection_missions rcm on rcm.ruleset_version_id = sr.ruleset_version_id and rcm.mission_code = mission.mission ->> 'id'
    left join ruleset_game_assets required_asset on required_asset.ruleset_version_id = sr.ruleset_version_id and required_asset.asset_type = 'NEED'
    and (
        lower(required_asset.asset_code) = lower(rule ->> 'value')
        or lower(required_asset.display_name) = lower(rule ->> 'value')
    ) on conflict (ruleset_collection_mission_id, requirement_order) do update set qty_required = excluded.qty_required,
    payload_json = excluded.payload_json;

insert into ruleset_financial_goals (
        ruleset_version_id,
        goal_code,
        item_name,
        purchase_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'hargaBeli') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'tujuanFinansial'
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, goal_code) do update set item_name = excluded.item_name,
    purchase_price = excluded.purchase_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_narratives (
        ruleset_version_id,
        narrative_code,
        item_name,
        repeatable,
        cooldown_turns,
        sort_order,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'repeatable') :: boolean, false),
    nullif(item ->> 'cooldownTurns', '') :: int,
    ord :: int,
    true,
    jsonb_build_object(
        'prerequisiteAksi',
        coalesce(item -> 'prerequisiteAksi', '[]' :: jsonb)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, narrative_code) do update set item_name = excluded.item_name,
    repeatable = excluded.repeatable,
    cooldown_turns = excluded.cooldown_turns,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_narrative_scenes (
        ruleset_version_id,
        ruleset_narrative_id,
        scene_code,
        scene_order,
        text_lines,
        media_json,
        payload_json
    ) select sr.ruleset_version_id,
    rn.ruleset_narrative_id,
    concat(
        narrative.narrative ->> 'id',
        'scene',
        ord :: int
    ),
    ord :: int,
    case when jsonb_typeof(narrative.narrative -> 'teks') = 'array' then narrative.narrative -> 'teks'
    when narrative.narrative ? 'teks' then jsonb_build_array(narrative.narrative ->> 'teks')
    else '[]' :: jsonb end,
    coalesce(narrative.narrative -> 'media', '{}' :: jsonb),
    jsonb_build_object(
        'source',
        'component_catalog.narasi',
        'teks',
        coalesce(narrative.narrative -> 'teks', '[]' :: jsonb)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) with ordinality as narrative(narrative, ord)
    join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id and rn.narrative_code = narrative.narrative ->> 'id' on conflict (ruleset_narrative_id, scene_code) do update set scene_order = excluded.scene_order,
    text_lines = excluded.text_lines,
    media_json = excluded.media_json,
    payload_json = excluded.payload_json;

insert into ruleset_trigger_conditions (
        ruleset_version_id,
        trigger_owner_type,
        ruleset_narrative_id,
        ruleset_action_id,
        reference_asset_id,
        operator,
        threshold_numeric,
        sort_order,
        condition_json,
        is_active
    ) select rn.ruleset_version_id,
    'NARRATIVE',
    rn.ruleset_narrative_id,
    ra.ruleset_action_id,
    null :: uuid,
    'COUNT_GTE',
    greatest(coalesce((prereq ->> 'value') :: int, 1), 1),
    ord :: int,
    jsonb_build_object('source', 'narrative.prerequisiteAksi'),
    true from seed_rulesets sr cross
    join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) as narrative(narrative)
    cross join lateral jsonb_array_elements(
        coalesce(
            narrative.narrative -> 'prerequisiteAksi',
            '[]' :: jsonb
        )
    ) with ordinality as x(prereq, ord)
    join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id and rn.narrative_code = narrative.narrative ->> 'id'
    join ruleset_actions ra on ra.ruleset_version_id = sr.ruleset_version_id and lower(ra.action_id) = lower(prereq ->> 'aksi')
    and ra.is_active on conflict (
        ruleset_version_id,
        ruleset_narrative_id,
        sort_order
    ) do update set ruleset_action_id = excluded.ruleset_action_id,
    reference_asset_id = excluded.reference_asset_id,
    operator = excluded.operator,
    threshold_numeric = excluded.threshold_numeric,
    condition_json = excluded.condition_json,
    is_active = excluded.is_active;

insert into ruleset_rank_points (
        ruleset_version_id,
        rank_type,
        rank_no,
        points,
        sort_order
    ) select sr.ruleset_version_id,
    'DONATION',
    coalesce((item ->> 'rank') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int from seed_rulesets sr cross
    join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'donation_rank_points',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do update set points = excluded.points,
    sort_order = excluded.sort_order;

insert into ruleset_gold_assets (
        ruleset_version_id,
        ruleset_game_asset_id,
        asset_code,
        quantity,
        points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    concat(
        'gold_card_',
        coalesce((item ->> 'qty') :: int, ord :: int)
    ),
    coalesce((item ->> 'qty') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'qty',
        coalesce((item ->> 'qty') :: int, ord :: int),
        'points',
        coalesce((item ->> 'points') :: int, 0)
    )
from
    seed_rulesets sr cross
    join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'gold_points_by_qty',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id and rga.asset_type = 'GOLD'
    and rga.asset_code = concat(
        'gold_card_',
        coalesce((item ->> 'qty') :: int, ord :: int)
    ) on conflict (ruleset_version_id, asset_code) do update set quantity = excluded.quantity,
    points = excluded.points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_rank_points (
        ruleset_version_id,
        rank_type,
        rank_no,
        points,
        sort_order
    ) select sr.ruleset_version_id,
    'PENSION',
    coalesce((item ->> 'rank') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int from seed_rulesets sr cross
    join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'pension_rank_points',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do update set points = excluded.points,
    sort_order = excluded.sort_order;

insert into ruleset_gold_prices (
        ruleset_version_id,
        ruleset_game_asset_id,
        price_code,
        quantity,
        unit_price,
        sort_order,
        card_qty,
        is_active,
        payload_json
    ) select extra.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.price_code,
    extra.quantity,
    extra.unit_price,
    extra.sort_order,
    extra.card_qty,
    true,
    extra.payload_json from (
        values
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_1',
                1,
                3,
                221,
                1,
                '{"qty":1,"price":3}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_2',
                1,
                4,
                222,
                1,
                '{"qty":1,"price":4}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_3',
                1,
                5,
                223,
                1,
                '{"qty":1,"price":5}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_4',
                1,
                6,
                224,
                1,
                '{"qty":1,"price":6}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_5',
                1,
                7,
                225,
                1,
                '{"qty":1,"price":7}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_6',
                1,
                8,
                226,
                1,
                '{"qty":1,"price":8}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_1',
                1,
                3,
                221,
                1,
                '{"qty":1,"price":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_2',
                1,
                4,
                222,
                1,
                '{"qty":1,"price":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_3',
                1,
                5,
                223,
                1,
                '{"qty":1,"price":5}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_4',
                1,
                6,
                224,
                1,
                '{"qty":1,"price":6}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_5',
                1,
                7,
                225,
                1,
                '{"qty":1,"price":7}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_6',
                1,
                8,
                226,
                1,
                '{"qty":1,"price":8}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        price_code,
        quantity,
        unit_price,
        sort_order,
        card_qty,
        payload_json
    )
    join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id and rga.asset_type = 'GOLD_PRICE'
    and rga.asset_code = extra.price_code on conflict (ruleset_version_id, price_code) do update set quantity = excluded.quantity,
    unit_price = excluded.unit_price,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into ruleset_tie_breakers (
        ruleset_version_id,
        ruleset_game_asset_id,
        tie_breaker_code,
        tie_number,
        sort_order,
        card_qty,
        payload_json
    ) select extra.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.tie_breaker_code,
    extra.tie_number,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json from (
        values
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_1',
                1,
                241,
                1,
                '{"number":1}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_2',
                2,
                242,
                1,
                '{"number":2}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_3',
                3,
                243,
                1,
                '{"number":3}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_4',
                4,
                244,
                1,
                '{"number":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_1',
                1,
                241,
                1,
                '{"number":1}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_2',
                2,
                242,
                1,
                '{"number":2}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_3',
                3,
                243,
                1,
                '{"number":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_4',
                4,
                244,
                1,
                '{"number":4}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        tie_breaker_code,
        tie_number,
        sort_order,
        card_qty,
        payload_json
    )
    join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id and rga.asset_type = 'TIE_BREAKER'
    and rga.asset_code = extra.tie_breaker_code on conflict (ruleset_version_id, tie_breaker_code) do update set tie_number = excluded.tie_number,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into ruleset_sharia_loans (
        ruleset_version_id,
        loan_code,
        item_name,
        principal,
        installment,
        duration_days,
        penalty_points,
        sort_order,
        card_qty,
        payload_json
    ) select extra.ruleset_version_id,
    extra.loan_code,
    extra.item_name,
    extra.principal,
    extra.installment,
    extra.duration_days,
    extra.penalty_points,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json from (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'loan_syariah_10',
                'Pinjaman Syariah 10',
                10,
                2,
                5,
                15,
                251,
                8,
                '{"principal":10,"installment":2,"duration":5,"penalty_points":15,"card_supply":8}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        loan_code,
        item_name,
        principal,
        installment,
        duration_days,
        penalty_points,
        sort_order,
        card_qty,
        payload_json
    ) on conflict (ruleset_version_id, loan_code) do update set item_name = excluded.item_name,
    principal = excluded.principal,
    installment = excluded.installment,
    duration_days = excluded.duration_days,
    penalty_points = excluded.penalty_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into ruleset_insurance_products (
        ruleset_version_id,
        product_code,
        item_name,
        premium,
        usage_limit,
        sort_order,
        card_qty,
        payload_json
    ) select extra.ruleset_version_id,
    extra.product_code,
    extra.item_name,
    extra.premium,
    extra.usage_limit,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json from (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'multirisk_basic',
                'Asuransi Multirisk',
                1,
                1,
                261,
                4,
                '{"premium":1,"usage_limit":1}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        product_code,
        item_name,
        premium,
        usage_limit,
        sort_order,
        card_qty,
        payload_json
    ) on conflict (ruleset_version_id, product_code) do update set item_name = excluded.item_name,
    premium = excluded.premium,
    usage_limit = excluded.usage_limit,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into ruleset_life_risks (
        ruleset_version_id,
        ruleset_game_asset_id,
        risk_code,
        item_name,
        effect_type,
        direction,
        amount,
        sort_order,
        card_qty,
        payload_json
    ) select extra.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.risk_code,
    extra.item_name,
    extra.effect_type,
    extra.direction,
    extra.amount,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json from (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_tunjangan',
                'Bonus Tunjangan',
                'COIN_EFFECT',
                'IN',
                4,
                271,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_biaya_perbaikan',
                'Biaya Perbaikan',
                'COIN_EFFECT',
                'OUT',
                4,
                272,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_kompetisi',
                'Bonus Kompetisi',
                'COIN_EFFECT',
                'IN',
                5,
                273,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":5}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_tetangga',
                'Bonus Tetangga',
                'COIN_EFFECT',
                'IN',
                3,
                274,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bayar_obat',
                'Bayar Obat',
                'COIN_EFFECT',
                'OUT',
                3,
                275,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_proyek',
                'Bonus Proyek',
                'COIN_EFFECT',
                'IN',
                6,
                276,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":6}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_sekolah_adik',
                'Biaya Sekolah Adik',
                'COIN_EFFECT',
                'OUT',
                4,
                277,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_motor_bocor',
                'Motor Bocor',
                'COIN_EFFECT',
                'OUT',
                3,
                278,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_uang_saku_hilang',
                'Uang Saku Hilang',
                'COIN_EFFECT',
                'OUT',
                2,
                279,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":2}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_tagihan_medis',
                'Tagihan Medis',
                'COIN_EFFECT',
                'OUT',
                3,
                280,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_biaya_perjalanan',
                'Biaya Perjalanan',
                'COIN_EFFECT',
                'OUT',
                4,
                281,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_peralatan_rusak',
                'Peralatan Rusak',
                'COIN_EFFECT',
                'OUT',
                4,
                282,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_kebutuhan_keluarga',
                'Kebutuhan Keluarga',
                'COIN_EFFECT',
                'OUT',
                4,
                283,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_servis_sepeda',
                'Servis Sepeda',
                'COIN_EFFECT',
                'OUT',
                3,
                284,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_biaya_listrik',
                'Biaya Listrik',
                'COIN_EFFECT',
                'OUT',
                4,
                285,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_biaya_kesehatan',
                'Biaya Kesehatan',
                'COIN_EFFECT',
                'OUT',
                4,
                286,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_uang_kas',
                'Uang Kas Berkurang',
                'COIN_EFFECT',
                'OUT',
                3,
                287,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_biaya_transport',
                'Biaya Transport',
                'COIN_EFFECT',
                'OUT',
                4,
                288,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_tagihan_air',
                'Tagihan Air',
                'COIN_EFFECT',
                'OUT',
                3,
                289,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_perbaikan_atap',
                'Perbaikan Atap',
                'COIN_EFFECT',
                'OUT',
                4,
                290,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_hospital',
                'Biaya Rumah Sakit',
                'COIN_EFFECT',
                'OUT',
                6,
                291,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_tabungan',
                'Bonus Tabungan',
                'COIN_EFFECT',
                'IN',
                4,
                292,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_tagihan_internet',
                'Tagihan Internet',
                'COIN_EFFECT',
                'OUT',
                3,
                293,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bonus_usaha',
                'Bonus Usaha',
                'COIN_EFFECT',
                'IN',
                5,
                294,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":5}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        risk_code,
        item_name,
        effect_type,
        direction,
        amount,
        sort_order,
        card_qty,
        payload_json
    )
    join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id and rga.asset_type = 'RISK'
    and rga.asset_code = extra.risk_code on conflict (ruleset_version_id, risk_code) do update set item_name = excluded.item_name,
    effect_type = excluded.effect_type,
    direction = excluded.direction,
    amount = excluded.amount,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

commit;