document.addEventListener('scroll', function() {
    const scrollPercentage = 100 * $(window).scrollTop() / ($(document).height() - $(window).height());
    if (((scrollPercentage > 3 || $(document).height() - $(window).scrollTop() < 900)) && window.innerWidth < 450) {
        $('#mobile-strip').fadeIn(100);
    } else {
        $('#mobile-strip').fadeOut(100);
    }
})