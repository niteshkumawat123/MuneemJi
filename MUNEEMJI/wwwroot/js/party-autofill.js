/**
 * Party Auto-Fill Script
 * When a party is selected from #partyDropdown, fetches party details
 * and auto-fills Billing Address, Shipping Address, Phone, and State of Supply fields.
 */
(function () {
    // Determine application base path from the script's own src attribute
    var scriptEl = document.querySelector('script[src*="party-autofill"]');
    var basePath = '';
    if (scriptEl) {
        var src = scriptEl.getAttribute('src');
        var idx = src.indexOf('/js/party-autofill');
        if (idx > 0) basePath = src.substring(0, idx);
    }

    document.addEventListener('DOMContentLoaded', function () {
        var dropdown = document.getElementById('partyDropdown');
        if (!dropdown) return;

        dropdown.addEventListener('change', function () {
            var partyId = this.value;
            if (!partyId) return;

            fetch(basePath + '/Party/GetPartyDetailsById?id=' + partyId, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Server returned ' + response.status);
                    }
                    return response.json();
                })
                .then(function (data) {
                    if (!data.success) return;

                    // Billing Address (textarea with word-wrap)
                    var billingAddr = document.querySelector('[name="Bill.BillingAddress"]');
                    if (billingAddr) billingAddr.value = data.billingAddress || '';

                    // Shipping Address (textarea with word-wrap)
                    var shippingAddr = document.querySelector('[name="Bill.ShippingAddress"]');
                    if (shippingAddr) shippingAddr.value = data.shippingAddress || '';

                    // Phone Number
                    var phone = document.querySelector('[name="Bill.PhoneNo"]');
                    if (phone) phone.value = data.phoneNumber || '';

                    // State of Supply (select dropdown with text values like "08-Rajasthan")
                    var stateSelect = document.querySelector('[name="Bill.StateOfSupply"]');
                    if (stateSelect && data.stateOfSupply) {
                        for (var i = 0; i < stateSelect.options.length; i++) {
                            if (stateSelect.options[i].value === data.stateOfSupply) {
                                stateSelect.selectedIndex = i;
                                break;
                            }
                        }
                    }

                    // Billing Name — always set to party name from database
                    var billingName = document.querySelector('[name="Bill.BillingName"]');
                    if (billingName) {
                        billingName.value = data.partyName || '';
                    }
                })
                .catch(function (err) {
                    console.error('Party auto-fill error:', err);
                });
        });
    });
})();
