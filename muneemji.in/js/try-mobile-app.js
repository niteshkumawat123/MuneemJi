const phoneNomberRegex = /[5-9]{1}[0-9]{9}/;
const usersDetail = {}

const phoneNumberElement = document.getElementById('user-phone-number');

phoneNumberElement.addEventListener('keyup', () => {

    const getPhoneNumber = document.getElementById('user-phone-number').value;
    if (getPhoneNumber && phoneNomberRegex.test(getPhoneNumber)) {
        usersDetail.phone_number = getPhoneNumber;
        usersDetail.pageName = window.location.pathname;
        usersDetail.sourceName = 'Try Mobile App';
    }
});

function captureUserInput(requestedBtn) {

    if (usersDetail.phone_number && usersDetail.phone_number.length == 10) {
        fetch('/api/ns/collect-number/', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(usersDetail)
        })
            .then(res => res.json())
            .then((responseData) => {
                $('#smsVerification').modal('show');
                if (responseData.statusCode === 201) {
                    document.getElementById('smsVerificationRespMsg').innerHTML = "Success !!";
                    document.getElementById("smsVerificationRespMsg").style.color = "#28a745";
                    document.getElementById('smsVerificationPara').innerHTML = `We have sent an install link to your <span class="font-weight-bold">WhatsApp</span>.<br> Please check and install the app.`;
                } else {
                    document.getElementById('smsVerificationRespMsg').innerHTML = "Ooops...";
                    document.getElementById("smsVerificationRespMsg").style.color = "#ed1a3b";
                    document.getElementById('smsVerificationPara').innerHTML = "The request failed. Please check your mobile number and try again.";
                }
                // usersDetail.phone_number = '';
                // document.getElementById('user-phone-number').value = '';
            })
            .catch(err => {
                $('#smsVerification').modal('show');
                document.getElementById('smsVerificationRespMsg').innerHTML = "Ooops...";
                document.getElementById("smsVerificationRespMsg").style.color = "#ed1a3b";
                document.getElementById('smsVerificationPara').innerHTML = "The request failed. Please check your mobile number and try again.";
            })
    }
    else {
        $('#smsVerification').modal('show');
        document.getElementById('smsVerificationRespMsg').innerHTML = "Ooops...";
        document.getElementById("smsVerificationRespMsg").style.color = "#ed1a3b";
        document.getElementById('smsVerificationPara').innerHTML = "The request failed. Please check your mobile number and try again.";
    }
    usersDetail.phone_number = '';
    document.getElementById('user-phone-number').value = '';
    $('#tryMobileAppModal').modal('hide');
    if (requestedBtn) {
        $('#tryMobileAppModal').modal('hide');
    }
}
function closeSmsAlertModal() {
    $('#smsVerification').modal('hide');
}

function closeTryMobileAppModal(){
    $('#tryMobileAppModal').modal('hide');
}


document.onkeydown = function (evt) {
    evt = evt || window.event;
    const element = $('#tryMobileAppModal');
    if (element.is(':visible') && evt.keyCode == 27) {
        captureUserInput();
    }
};

document.onclick = function (evt) {
    evt = evt || window.event;
    const element = $('#smsVerification');
    if(element.is(':visible')) {
        closeSmsAlertModal();
    }
}