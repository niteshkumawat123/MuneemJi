// ??????????????????????????????????????????????????
// ??????????????????????????????????????????????????????????????
// SIDEBAR.JS — Global Search Popup (Ctrl+F) + Collapsible Sidebar
// Complete, production-ready — MuneemJi
// ??????????????????????????????????????????????????????????????

(function () {
    'use strict';

    // ??????????????????????????????????????????????
    // ALL PAGES — Every navigable page in the app
    // URLs match existing sidebar nav-link hrefs exactly
    // ??????????????????????????????????????????????
    var ALL_PAGES = [
        // Home
        { name: "Home",                        url: "/Web/Home/index",                  icon: "bi-house-door" },

        // Parties
        { name: "All Parties",                 url: "/Web/party/index",                 icon: "bi-people" },
        { name: "Add Party",                   url: "/Web/party/add",                   icon: "bi-person-plus" },

        // Items
        { name: "All Items",                   url: "/Web/items/Products",              icon: "bi-box-seam" },
        { name: "Add Item",                    url: "/Web/Items/create",                icon: "bi-plus-circle" },
        { name: "Godown Management",           url: "/Web/godown/index",                icon: "bi-building" },

        // Sale
        { name: "New Invoice",                 url: "/Web/Sales/Create",                icon: "bi-file-earmark-plus" },
        { name: "Sales Invoices",              url: "/Web/Sales/index",                 icon: "bi-receipt" },
        { name: "Estimate/Quotation",          url: "/Web/Estimate_Quotations/index",   icon: "bi-file-earmark-text" },
        { name: "Payment In",                  url: "/Web/PaymentIn/index",             icon: "bi-cash-stack" },
        { name: "Sale Order",                  url: "/Web/SalesOrder/index",            icon: "bi-cart-check" },
        { name: "Delivery Challan",            url: "/Web/DeliveryChallan/index",       icon: "bi-truck" },
        { name: "Sale Return/Credit Note",     url: "/Web/CreditNote/index",            icon: "bi-arrow-return-left" },
        { name: "Other Income",                url: "/Web/OtherIncome/index",           icon: "bi-currency-dollar" },

        // Purchase & Expense
        { name: "New Purchase",                url: "/Web/PurchaseBill/Create",         icon: "bi-bag-plus" },
        { name: "Purchase Bill",               url: "/Web/PurchaseBill/index",          icon: "bi-cart3" },
        { name: "Payment Out",                 url: "/Web/PaymentOut/index",            icon: "bi-cash" },
        { name: "Expense",                     url: "/Web/Expense/Index",               icon: "bi-wallet2" },
        { name: "Purchase Order",              url: "/Web/PurchaseOrder/index",         icon: "bi-clipboard-check" },
        { name: "Purchase Return/Dr. Note",    url: "/Web/DebitNote/index",             icon: "bi-arrow-return-right" },

        // Grow Your Business
        { name: "WhatsApp Marketing",          url: "/Web/WhatsAppMarketing/index",     icon: "bi-whatsapp" },

        // Cash & Bank
        { name: "Bank Accounts",               url: "/Web/bank/index",                  icon: "bi-bank" },
        { name: "Cash In Hand",                url: "/Web/bank/CashinHand",             icon: "bi-cash-coin" },
        { name: "Loan Account",                url: "/Web/Loan/Index",                  icon: "bi-credit-card" },

        // Reports
        { name: "Reports",                     url: "/Web/Report/Index",                icon: "bi-bar-chart" },
        { name: "Sale Report",                 url: "/Web/Report/Sale_Report",          icon: "bi-graph-up" },
        { name: "Purchase Report",             url: "/Web/Report/Purchase_Report",      icon: "bi-graph-down" },
        { name: "All Transactions",            url: "/Web/Report/AllTransactions_Report", icon: "bi-list-check" },
        { name: "Day Book",                    url: "/Web/Report/Daybook_report",       icon: "bi-journal-text" },
        { name: "All Party Report",            url: "/Web/Report/All_Party_Report",     icon: "bi-people-fill" },
        { name: "Bill Wise Profit",            url: "/Web/Report/Bill_Wise_Profit",     icon: "bi-cash-stack" },
        { name: "Cash Flow Report",            url: "/Web/Report/CashFlow_report",      icon: "bi-water" },
        { name: "Stock Detail",                url: "/Web/Report/StockDetail",          icon: "bi-box" },
        { name: "Low Stock Summary",           url: "/Web/Report/LowStockSummary",     icon: "bi-exclamation-triangle" },
        { name: "Item Detail Report",          url: "/Web/Report/ItemDetail",           icon: "bi-info-circle" },
        { name: "Expense Report",              url: "/Web/Report/ExpenseReport",        icon: "bi-wallet" },
        { name: "Expense Category Report",     url: "/Web/Report/ExpenseCategoryReport", icon: "bi-tags" },
        { name: "Expense Item Report",         url: "/Web/Report/ExpenseItemReport",    icon: "bi-receipt-cutoff" },
        { name: "Other Income Report",         url: "/Web/Report/OtherIncome_Report",   icon: "bi-currency-exchange" },
        { name: "Sale/Purchase by Party",      url: "/Web/Report/Sale_Purchase_by_Party", icon: "bi-person-lines-fill" },
        { name: "Party Report by Item",        url: "/Web/Report/Party_Report_by_Item", icon: "bi-person-badge" },
        { name: "Sale/Purchase Order Report",  url: "/Web/Report/Sale_Purchase_Order_report", icon: "bi-clipboard2-data" },
        { name: "Sale Aging Report",           url: "/Web/Report/Sale_Aging_Report",    icon: "bi-clock-history" },
        { name: "TDS Receivable",              url: "/Web/Report/TDSReceivable",        icon: "bi-file-earmark-ruled" },
        { name: "TDS Payable",                 url: "/Web/Report/TDSPayable",           icon: "bi-file-earmark-minus" },
        { name: "TCS Receivable",              url: "/Web/Report/TCSReceivable",        icon: "bi-file-earmark-check" },
        { name: "Form 27EQ",                   url: "/Web/Report/Form27EQ",             icon: "bi-file-earmark-spreadsheet" },

        // Enquiry & Support
        { name: "Enquiry Management",          url: "/Web/Enquiry/Index",               icon: "bi-envelope-paper" },
        { name: "Support Enquiries",           url: "/Web/Support/Index",               icon: "bi-headset" },

        // Sync & Share
        { name: "Sync & Share",                url: "/Web/User/Index",                  icon: "bi-cloud-arrow-up" },

        // Settings
        { name: "Settings",                    url: "/Web/Settings/General/Index",       icon: "bi-gear" },

        // Plans & Pricing
        { name: "Plans & Pricing",             url: "/Web/PlansAndPricing/index",       icon: "bi-trophy" },

        // Business Profile
        { name: "Business Profile",            url: "/Web/BusinessProfile/Edit",        icon: "bi-building" },

        // All Transactions (Home)
        { name: "All Transactions (Home)",     url: "/Web/Home/AllTransection",         icon: "bi-arrow-left-right" }
    ];

    // ??????????????????????????????????????????????
    // CONSTANTS
    // ??????????????????????????????????????????????
    var RECENT_KEY = 'muneemji_recent_pages';
    var SIDEBAR_KEY = 'muneemji_sidebar_collapsed';
    var MAX_RECENT = 5;
    var SIDEBAR_EXPANDED_WIDTH = '234px';
    var SIDEBAR_COLLAPSED_WIDTH = '65px';
    var TRANSITION_DURATION = 300; // ms

    // ??????????????????????????????????????????????
    // RECENT PAGES — localStorage helpers
    // ??????????????????????????????????????????????
    function loadRecentPages() {
        try {
            var data = JSON.parse(localStorage.getItem(RECENT_KEY));
            return Array.isArray(data) ? data : [];
        } catch (e) {
            return [];
        }
    }

    function saveRecentPages(pages) {
        try {
            localStorage.setItem(RECENT_KEY, JSON.stringify(pages));
        } catch (e) { /* quota exceeded — silently ignore */ }
    }

    function updateRecentPages(pageName, url) {
        if (!pageName || !url) return;
        var recent = loadRecentPages();
        // Remove duplicate
        recent = recent.filter(function (r) { return r.url !== url; });
        // Add to front
        recent.unshift({ name: pageName, url: url });
        // Trim to max
        if (recent.length > MAX_RECENT) {
            recent = recent.slice(0, MAX_RECENT);
        }
        saveRecentPages(recent);
    }

    function trackCurrentPage() {
        var path = window.location.pathname;
        var pathLower = path.toLowerCase();
        // Try exact match first, then case-insensitive
        var match = null;
        for (var i = 0; i < ALL_PAGES.length; i++) {
            if (ALL_PAGES[i].url === path || ALL_PAGES[i].url.toLowerCase() === pathLower) {
                match = ALL_PAGES[i];
                break;
            }
        }
        if (match) {
            updateRecentPages(match.name, match.url);
        }
    }

    // ??????????????????????????????????????????????
    // NAVIGATION
    // ??????????????????????????????????????????????
    function navigateToPage(url, pageName) {
        updateRecentPages(pageName, url);
        closeSearchModal();
        window.location.href = url;
    }
    // Expose globally so onclick handlers in rendered HTML can call it
    window.navigateToPage = navigateToPage;

    // ??????????????????????????????????????????????
    // SEARCH MODAL
    // ??????????????????????????????????????????????
    var searchHighlightIndex = -1;

    function openSearchModal() {
        var modal = document.getElementById('searchModal');
        if (!modal) return;
        modal.style.display = 'flex';
        // Force reflow before adding class for CSS transition
        void modal.offsetWidth;
        modal.classList.add('show');
        var input = document.getElementById('searchModalInput');
        if (input) {
            input.value = '';
            input.focus();
        }
        searchHighlightIndex = -1;
        renderSearchResults('');
        document.body.style.overflow = 'hidden';
    }
    window.openSearchModal = openSearchModal;

    function closeSearchModal() {
        var modal = document.getElementById('searchModal');
        if (!modal) return;
        modal.classList.remove('show');
        setTimeout(function () {
            modal.style.display = 'none';
        }, 200);
        document.body.style.overflow = '';
    }
    window.closeSearchModal = closeSearchModal;

    function filterPages(searchText) {
        if (!searchText) return ALL_PAGES.slice();
        searchText = searchText.toLowerCase();
        return ALL_PAGES.filter(function (p) {
            return p.name.toLowerCase().indexOf(searchText) !== -1;
        });
    }

    function escapeHtml(str) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(str));
        return div.innerHTML;
    }

    function renderSearchResults(searchText) {
        var container = document.getElementById('searchResultsList');
        if (!container) return;

        searchText = (searchText || '').trim();
        var searchLower = searchText.toLowerCase();
        var html = '';

        if (!searchLower) {
            // Show Recent Pages + All Suggested Pages
            var recent = loadRecentPages();
            if (recent.length > 0) {
                html += '<div class="search-section-title">Recent Pages</div>';
                for (var r = 0; r < recent.length; r++) {
                    html += buildResultItem(recent[r].name, recent[r].url, '', true);
                }
            }
            html += '<div class="search-section-title">Suggested Pages</div>';
            for (var s = 0; s < ALL_PAGES.length; s++) {
                html += buildResultItem(ALL_PAGES[s].name, ALL_PAGES[s].url, ALL_PAGES[s].icon, false);
            }
        } else {
            var filtered = filterPages(searchLower);
            if (filtered.length === 0) {
                html = '<div class="search-no-results">No pages found for your search</div>';
            } else {
                for (var f = 0; f < filtered.length; f++) {
                    html += buildResultItem(filtered[f].name, filtered[f].url, filtered[f].icon, false);
                }
            }
        }

        container.innerHTML = html;
        searchHighlightIndex = -1;
    }

    function buildResultItem(name, url, icon, showArrow) {
        var safeName = escapeHtml(name);
        var safeUrl = escapeHtml(url);
        // Use data attributes; click handler reads them
        var item = '<div class="search-result-item" data-url="' + safeUrl + '" data-name="' + safeName + '">';
        if (icon) {
            item += '<i class="bi ' + icon + ' me-2"></i>';
        }
        item += '<span>' + safeName + '</span>';
        if (showArrow) {
            item += '<span class="search-result-arrow">?</span>';
        }
        item += '</div>';
        return item;
    }

    function moveSearchHighlight(direction) {
        var items = document.querySelectorAll('#searchResultsList .search-result-item');
        if (items.length === 0) return;

        // Remove old highlight
        for (var i = 0; i < items.length; i++) {
            items[i].classList.remove('highlighted');
        }

        searchHighlightIndex += direction;
        if (searchHighlightIndex < 0) searchHighlightIndex = items.length - 1;
        if (searchHighlightIndex >= items.length) searchHighlightIndex = 0;

        items[searchHighlightIndex].classList.add('highlighted');
        items[searchHighlightIndex].scrollIntoView({ block: 'nearest' });
    }

    function selectHighlighted() {
        var items = document.querySelectorAll('#searchResultsList .search-result-item');
        if (searchHighlightIndex >= 0 && searchHighlightIndex < items.length) {
            var el = items[searchHighlightIndex];
            navigateToPage(el.getAttribute('data-url'), el.getAttribute('data-name'));
        }
    }

    // ??????????????????????????????????????????????
    // SIDEBAR COLLAPSE / EXPAND
    // ??????????????????????????????????????????????
    // We store original data-bs-toggle values so we can restore them
    var _collapseLinksOriginal = [];

    function initSidebar() {
        var sidebar = document.getElementById('sidebar');
        if (!sidebar) return;

        // Remember which links had data-bs-toggle="collapse" originally
        var collapseLinks = sidebar.querySelectorAll('.nav-link[data-bs-toggle="collapse"]');
        for (var i = 0; i < collapseLinks.length; i++) {
            _collapseLinksOriginal.push({
                el: collapseLinks[i],
                toggle: collapseLinks[i].getAttribute('data-bs-toggle'),
                href: collapseLinks[i].getAttribute('href')
            });
        }

        // Apply saved state (no animation on page load)
        var collapsed = localStorage.getItem(SIDEBAR_KEY) === 'true';
        applySidebarState(collapsed, false);

        // Track the page the user just navigated to
        trackCurrentPage();

        // Highlight active nav item
        highlightNavItem();
    }

    function toggleSidebar() {
        var sidebar = document.getElementById('sidebar');
        if (!sidebar) return;
        var isCollapsed = sidebar.classList.contains('sidebar-collapsed');
        applySidebarState(!isCollapsed, true);
    }
    window.toggleSidebar = toggleSidebar;

    function applySidebarState(collapsed, animate) {
        var sidebar = document.getElementById('sidebar');
        var mainContent = document.querySelector('.main-content');
        var topHeader = document.querySelector('.top-header');
        var toggleBtn = document.getElementById('sidebarCollapseBtn');

        if (!sidebar) return;

        // Add transition classes if animating
        if (animate) {
            sidebar.classList.add('sidebar-transition');
            if (mainContent) mainContent.classList.add('content-area-transition');
            if (topHeader) topHeader.classList.add('content-area-transition');
        }

        if (collapsed) {
            // ?? COLLAPSE ??
            sidebar.classList.add('sidebar-collapsed');
            sidebar.classList.remove('sidebar-expanded');
            if (mainContent) mainContent.style.marginLeft = SIDEBAR_COLLAPSED_WIDTH;
            if (topHeader) topHeader.style.left = SIDEBAR_COLLAPSED_WIDTH;
            if (toggleBtn) toggleBtn.innerHTML = '<i class="bi bi-chevron-right"></i>';

            // Close all open sub-menus
            var openMenus = sidebar.querySelectorAll('.collapse.show');
            for (var i = 0; i < openMenus.length; i++) {
                var bsCollapse = bootstrap.Collapse.getInstance(openMenus[i]);
                if (bsCollapse) {
                    bsCollapse.hide();
                } else {
                    openMenus[i].classList.remove('show');
                }
            }

            // Disable collapse toggle on parent links so clicking icons does nothing
            for (var c = 0; c < _collapseLinksOriginal.length; c++) {
                _collapseLinksOriginal[c].el.removeAttribute('data-bs-toggle');
            }

            // Enable Bootstrap tooltips
            enableTooltips(sidebar);

        } else {
            // ?? EXPAND ??
            sidebar.classList.remove('sidebar-collapsed');
            sidebar.classList.add('sidebar-expanded');
            if (mainContent) mainContent.style.marginLeft = SIDEBAR_EXPANDED_WIDTH;
            if (topHeader) topHeader.style.left = SIDEBAR_EXPANDED_WIDTH;
            if (toggleBtn) toggleBtn.innerHTML = '<i class="bi bi-chevron-left"></i>';

            // Restore collapse toggle on parent links
            for (var r = 0; r < _collapseLinksOriginal.length; r++) {
                _collapseLinksOriginal[r].el.setAttribute('data-bs-toggle', _collapseLinksOriginal[r].toggle);
            }

            // Disable tooltips
            disableTooltips(sidebar);
        }

        // Persist state
        try {
            localStorage.setItem(SIDEBAR_KEY, collapsed ? 'true' : 'false');
        } catch (e) { /* ignore */ }

        // Remove transition classes after animation completes
        if (animate) {
            setTimeout(function () {
                sidebar.classList.remove('sidebar-transition');
                if (mainContent) mainContent.classList.remove('content-area-transition');
                if (topHeader) topHeader.classList.remove('content-area-transition');
            }, TRANSITION_DURATION + 50);
        }
    }

    // ??????????????????????????????????????????????
    // TOOLTIPS (only in collapsed state)
    // Uses data-bs-title attribute already on nav-links
    // We do NOT touch data-bs-toggle on collapse links;
    // instead we create tooltips manually via JS API.
    // ??????????????????????????????????????????????
    var _activeTooltips = [];

    function enableTooltips(sidebar) {
        disableTooltips(sidebar); // clean up first
        var links = sidebar.querySelectorAll('.nav-link[data-bs-title], .nav-link-left[data-bs-title]');
        for (var i = 0; i < links.length; i++) {
            var tt = new bootstrap.Tooltip(links[i], {
                title: links[i].getAttribute('data-bs-title'),
                placement: 'right',
                trigger: 'hover',
                container: 'body'
            });
            _activeTooltips.push({ el: links[i], instance: tt });
        }
    }

    function disableTooltips(sidebar) {
        for (var i = 0; i < _activeTooltips.length; i++) {
            try { _activeTooltips[i].instance.dispose(); } catch (e) { /* ignore */ }
        }
        _activeTooltips = [];
    }

    // ??????????????????????????????????????????????
    // HIGHLIGHT ACTIVE NAV ITEM based on current URL
    // ??????????????????????????????????????????????
    function highlightNavItem() {
        var path = window.location.pathname.toLowerCase();
        var sidebar = document.getElementById('sidebar');
        if (!sidebar) return;

        // Remove any JS-added active from sub-links
        var subLinks = sidebar.querySelectorAll('.btn-toggle-nav .nav-link');
        for (var i = 0; i < subLinks.length; i++) {
            var href = subLinks[i].getAttribute('href');
            if (href && href.toLowerCase() === path) {
                subLinks[i].classList.add('active');
            }
        }
    }

    // ??????????????????????????????????????????????
    // EVENT LISTENERS — wired up on DOMContentLoaded
    // ??????????????????????????????????????????????
    document.addEventListener('DOMContentLoaded', function () {
        // 1. Initialize sidebar state
        initSidebar();

        // 2. Search modal input events
        var searchInput = document.getElementById('searchModalInput');
        if (searchInput) {
            searchInput.addEventListener('input', function () {
                renderSearchResults(this.value);
            });
            searchInput.addEventListener('keydown', function (e) {
                switch (e.key) {
                    case 'ArrowDown':
                        e.preventDefault();
                        moveSearchHighlight(1);
                        break;
                    case 'ArrowUp':
                        e.preventDefault();
                        moveSearchHighlight(-1);
                        break;
                    case 'Enter':
                        e.preventDefault();
                        selectHighlighted();
                        break;
                    case 'Escape':
                        e.preventDefault();
                        closeSearchModal();
                        break;
                }
            });
        }

        // 3. Close search modal when clicking backdrop
        var searchModal = document.getElementById('searchModal');
        if (searchModal) {
            searchModal.addEventListener('click', function (e) {
                if (e.target === searchModal) {
                    closeSearchModal();
                }
            });
            // Delegate click on result items
            searchModal.addEventListener('click', function (e) {
                var item = e.target.closest('.search-result-item');
                if (item) {
                    var url = item.getAttribute('data-url');
                    var name = item.getAttribute('data-name');
                    if (url && name) {
                        navigateToPage(url, name);
                    }
                }
            });
        }

        // 4. Global keyboard shortcuts
        document.addEventListener('keydown', function (e) {
            // Ctrl+F or Cmd+F ? open search
            if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
                e.preventDefault();
                openSearchModal();
                return;
            }
            // Escape ? close search if open
            if (e.key === 'Escape') {
                var modal = document.getElementById('searchModal');
                if (modal && modal.classList.contains('show')) {
                    e.preventDefault();
                    closeSearchModal();
                }
            }
        });

        // 5. Prevent sub-menu expand when sidebar is collapsed
        //    We intercept clicks on the parent nav-links
        var sidebar = document.getElementById('sidebar');
        if (sidebar) {
            sidebar.addEventListener('click', function (e) {
                if (!sidebar.classList.contains('sidebar-collapsed')) return;
                // Check if click is on a parent nav-link (one that originally had collapse toggle)
                var link = e.target.closest('.nav-link');
                if (!link) return;
                for (var i = 0; i < _collapseLinksOriginal.length; i++) {
                    if (_collapseLinksOriginal[i].el === link) {
                        e.preventDefault();
                        e.stopPropagation();
                        return;
                    }
                }
            });
        }
    });

})();
