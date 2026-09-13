// Fungsi file: Membuka satu kategori statistik sambil mempertahankan rincian sesi di bawahnya.
(() => {
    const playerInput = document.getElementById('statistics-player');
    if (playerInput) {
        const playerId = playerInput.form.elements.playerId;
        const options = Array.from(playerInput.list.options);
        const selected = options.find(option => option.dataset.playerId === playerId.value);
        if (selected) playerInput.value = selected.value;
        const resolvePlayer = () => {
            const value = playerInput.value.trim();
            const exact = options.find(option => option.value === value);
            const matches = options.filter(option => option.value.toLowerCase() === value.toLowerCase());
            // Never silently select the first person when a typed name is ambiguous.
            const match = exact || (matches.length === 1 ? matches[0] : null);
            playerId.value = match?.dataset.playerId || '';
            playerInput.setCustomValidity(match ? '' : playerInput.dataset.invalidPlayer);
        };
        playerInput.addEventListener('input', resolvePlayer);
        resolvePlayer();
    }
    const buttons = document.querySelectorAll('[data-statistics-panel]');
    const sections = document.querySelectorAll('[data-statistics-section]');
    buttons.forEach(button => button.addEventListener('click', () => {
        const selected = button.dataset.statisticsPanel;
        buttons.forEach(item => item.setAttribute('aria-expanded', String(item === button)));
        sections.forEach(section => { section.hidden = section.id !== selected; });
    }));
    document.querySelectorAll('[data-statistics-pagination]').forEach(pager => {
        const rows = Array.from(document.getElementById(pager.querySelector('[data-page-next]').getAttribute('aria-controls')).tBodies[0].rows);
        const back = pager.querySelector('[data-page-back]');
        const next = pager.querySelector('[data-page-next]');
        const status = pager.querySelector('[data-page-status-text]');
        const pageSize = 5;
        let page = 0;
        const render = () => {
            const start = page * pageSize;
            rows.forEach((row, index) => { row.hidden = index < start || index >= start + pageSize; });
            back.disabled = page === 0;
            next.disabled = start + pageSize >= rows.length;
            status.textContent = pager.dataset.pageStatus
                .replace('{start}', rows.length ? start + 1 : 0)
                .replace('{end}', Math.min(start + pageSize, rows.length))
                .replace('{total}', rows.length);
        };
        back.addEventListener('click', () => { if (page > 0) { page--; render(); } });
        next.addEventListener('click', () => { if ((page + 1) * pageSize < rows.length) { page++; render(); } });
        render();
    });
})();
