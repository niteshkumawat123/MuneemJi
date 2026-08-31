// dropdown-loader.js
// Automatically populates select elements with data-dropdown-source attribute
// Usage: <select data-dropdown-source="Godowns" data-default-text="All Godown">
//          <option selected>All Godown</option>
//        </select>
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var selects = document.querySelectorAll('select[data-dropdown-source]');
        selects.forEach(function (sel) {
            var source = sel.getAttribute('data-dropdown-source');
            var url = '/Web/DropdownAjax/' + source;
            fetch(url)
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    while (sel.options.length > 1) {
                        sel.remove(1);
                    }
                    if (data && data.length > 0) {
                        data.forEach(function (item) {
                            var opt = document.createElement('option');
                            opt.value = item.id;
                            opt.textContent = item.name;
                            sel.appendChild(opt);
                        });
                    }
                })
                .catch(function (err) {
                    console.warn('[dropdown-loader] Failed to load ' + source, err);
                });
        });
    });
})();
