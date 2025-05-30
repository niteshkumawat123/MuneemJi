const scrollPhoneNumber = document.getElementById("scroll-phone"),
    submitButton = document.getElementById("scroll-submit"),
    closeButton = document.getElementById("close-scroll");
let scrollPopupShown = !1;
closeButton.addEventListener("click", function() {
    $("#scroll-popup").modal("hide"), scrollPopupShown = !0
}), scrollPhoneNumber.addEventListener("focus", function() {
    scrollPhoneNumber.value || (scrollPhoneNumber.placeholder = "")
}), scrollPhoneNumber.addEventListener("blur", function() {
    scrollPhoneNumber.value.length < 1 && (scrollPhoneNumber.placeholder = "Enter your phone number")
}), scrollPhoneNumber.addEventListener("keyup", function() {
    let e = scrollPhoneNumber.value;
    e.length > 10 && (e = e.substr(0, 10), scrollPhoneNumber.value = e)
}), submitButton.addEventListener("click", function() {
    let e = scrollPhoneNumber.value,
        l = {};
    if (e && 10 === e.length && phoneCheck.test(e)) l.phone_number = e, l.pageName = "scroll popup", l.userDeviceDetails = navigator.userAgent, document.getElementById("scroll_error_message").style.display = "none";
    else {
        document.getElementById("scroll_error_message").style.display = "block";
        return
    }
    $.ajax({
        type: "POST",
        url: "/api/ns/collect-number/",
        data: l,
        success: function(e) {
            $(".scroll-popup-title, .scroll-input-section").hide(), $(".scroll-input-success, .scroll-popup-success").show(), $("#scroll_modal_content").css("height", "210px"), $("#scroll_modal_content").css("background-image", "none")
        },
        error: function(e) {
            alert("Some error occured. Please try again in some time.")
        }
    })
}), document.addEventListener("scroll", function() {
    let e = 100 * $(window).scrollTop() / ($(document).height() - $(window).height());
    (e > 70 || $(document).height() - $(window).scrollTop() < 900) && !scrollPopupShown && window.innerWidth < 600 && ($("#scroll-popup").modal("show"), scrollPopupShown = !0)
});