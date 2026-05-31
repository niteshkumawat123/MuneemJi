$('#mobile-download-link, .download-for-mobile, #mobile-download-strip').on('click', function(e) {
    if (isIos()) {
        e.preventDefault();
        window.location.href = 'https://apps.apple.com/in/app/vyapar-gst-invoicing-software/id6478382307';
    }
})

function isIos() {
    return !window.MSStream && /iPad|iPhone|iPod/.test(navigator.userAgent);
}