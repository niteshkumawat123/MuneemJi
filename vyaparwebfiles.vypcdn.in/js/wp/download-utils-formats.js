$('#cancel-if').off().on('click', () => {
    $('#invoice-format-download-modal').modal('hide');
})
$('#submit-formats').off().on('click', () => {
    const userDetailsFormats = {};
    userDetailsFormats.phone_number = $.trim($('#phone-number-if').val());
    userDetailsFormats.pageName = window.location.href;
    userDetailsFormats.userDeviceDetails = navigator.userAgent;
    userDetailsFormats.businessType = $.trim($('#business-type').val());
    userDetailsFormats.city = $.trim($('#city').val());
    const validUserInput = validateUserInput(userDetailsFormats);
    if (validUserInput) {
        storeUserData(userDetailsFormats);
        startDownload();
        $('#invoice-format-download-modal').modal('hide');
    }
});

function validateUserInput(userInput) {
    const {
        phone_number,
        businessType,
        city
    } = userInput;
    let validationFlag = true;
    const phoneRegex = new RegExp('^[5-9]\\d{9}$');
    $('.invalid-feedback').hide();
    if (!(phone_number.match(phoneRegex))) {
        $('#invalid-feedback-phone-number-if').show();
        validationFlag = false
    }
    if (!businessType) {
        $('#invalid-feedback-business-type').show();
        validationFlag = false;
    }
    if (!city || city.length > 20) {
        $('#invalid-feedback-city').show();
        validationFlag = false;
    }
    return validationFlag;
}