/**
 * report-table-enhance.js
 * Auto-enhances every table inside #contentArea with:
 *   1. Adds .rpt-table class (for shared ERP styling)
 *   2. Wraps in .rpt-table-wrap if not already wrapped
 *   3. Per-column text filter row
 *   4. Draggable column resize handles
 * Runs automatically after DOM is ready or after AJAX content injection.
 */
(function () {
    'use strict';

    function enhanceTable(table) {
        if (table.dataset.enhanced) return;
        table.dataset.enhanced = '1';

        // Add rpt-table class if missing
        if (!table.classList.contains('rpt-table')) {
            table.classList.add('rpt-table');
        }

        // Ensure thead exists; if table uses first row as header, wrap it
        var thead = table.querySelector('thead');
        if (!thead) {
            var firstRow = table.querySelector('tr');
            if (!firstRow) return;
            var hasTh = firstRow.querySelector('th');
            if (hasTh) {
                thead = document.createElement('thead');
                firstRow.parentNode.insertBefore(thead, firstRow);
                thead.appendChild(firstRow);
                // Wrap remaining rows in tbody if needed
                var tbody = table.querySelector('tbody');
                if (!tbody) {
                    tbody = document.createElement('tbody');
                    while (table.querySelector('tr:not(thead tr)')) {
                        var r = table.querySelector('tr:not(thead tr)');
                        if (!r || thead.contains(r)) break;
                        tbody.appendChild(r);
                    }
                    table.appendChild(tbody);
                }
            } else {
                return; // no header row at all
            }
        }

        var headerRow = thead.querySelector('tr');
        if (!headerRow) return;
        var ths = headerRow.querySelectorAll('th');
        if (ths.length === 0) return;

        // Wrap table in .rpt-table-wrap if not wrapped
        var parent = table.parentElement;
        if (parent && !parent.classList.contains('rpt-table-wrap')) {
            var wrap = document.createElement('div');
            wrap.className = 'rpt-table-wrap';
            parent.insertBefore(wrap, table);
            wrap.appendChild(table);
        }

        // --- 1. Per-column filter row ---
        var filterRow = document.createElement('tr');
        filterRow.className = 'col-filter-row';
        for (var i = 0; i < ths.length; i++) {
            var fth = document.createElement('th');
            var inp = document.createElement('input');
            inp.type = 'text';
            inp.placeholder = '\u2315';
            inp.dataset.colIdx = i;
            inp.addEventListener('input', makeColumnFilter(table));
            fth.appendChild(inp);
            filterRow.appendChild(fth);
        }
        thead.appendChild(filterRow);

        // --- 2. Column resize handles ---
        for (var j = 0; j < ths.length; j++) {
            var handle = document.createElement('div');
            handle.className = 'rpt-col-resize';
            ths[j].appendChild(handle);
            ths[j].style.position = 'relative';
            bindResize(handle, ths[j], table);
        }
    }

    function makeColumnFilter(table) {
        return function () {
            var filters = table.querySelectorAll('thead tr.col-filter-row input');
            var tbody = table.querySelector('tbody');
            if (!tbody) return;
            var rows = tbody.querySelectorAll('tr');
            rows.forEach(function (row) {
                if (row.classList.contains('no-data-row')) return;
                var cells = row.querySelectorAll('td');
                if (cells.length === 0) return;
                var visible = true;
                filters.forEach(function (f) {
                    var ci = parseInt(f.dataset.colIdx, 10);
                    var term = f.value.toLowerCase().trim();
                    if (!term) return;
                    var cell = cells[ci];
                    if (!cell) return;
                    if (cell.textContent.toLowerCase().indexOf(term) === -1) visible = false;
                });
                row.style.display = visible ? '' : 'none';
            });
        };
    }

    function bindResize(handle, th, table) {
        var startX, startW;
        handle.addEventListener('mousedown', function (e) {
            e.preventDefault();
            startX = e.pageX;
            startW = th.offsetWidth;
            handle.classList.add('active');
            table.style.tableLayout = 'fixed';
            if (!table.dataset.widthsLocked) {
                var allTh = table.querySelectorAll('thead tr:first-child th');
                allTh.forEach(function (t) { t.style.width = t.offsetWidth + 'px'; });
                table.dataset.widthsLocked = '1';
            }
            document.addEventListener('mousemove', onMove);
            document.addEventListener('mouseup', onUp);
        });
        function onMove(e) {
            var w = startW + (e.pageX - startX);
            if (w > 30) th.style.width = w + 'px';
        }
        function onUp() {
            handle.classList.remove('active');
            document.removeEventListener('mousemove', onMove);
            document.removeEventListener('mouseup', onUp);
        }
    }

    // Strip standalone page wrappers (inline <style>, etc.) injected via AJAX
    function stripStandaloneWrappers(container) {
        if (!container) return;
        // Remove all inline <style> blocks from standalone pages so shared CSS wins
        var styles = container.querySelectorAll('style');
        styles.forEach(function (s) { s.remove(); });
        // Remove any <meta> tags that leaked in
        var metas = container.querySelectorAll('meta');
        metas.forEach(function (m) { m.remove(); });
    }

    // Public API so AJAX loads can re-trigger
    window.enhanceReportTables = function () {
        var container = document.getElementById('contentArea');
        if (container) stripStandaloneWrappers(container);
        var tables = container ? container.querySelectorAll('table') : document.querySelectorAll('.rpt-table');
        tables.forEach(enhanceTable);
    };

    // Auto-run on DOMContentLoaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', window.enhanceReportTables);
    } else {
        window.enhanceReportTables();
    }
})();
