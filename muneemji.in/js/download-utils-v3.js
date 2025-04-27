const userDetails = {};
const phoneCheck = /[5-9]{1}[0-9]{9}/;

function generateUniqueId() {
    if (!localStorage.getItem('uid')) {
        const uniqueId = new Date().valueOf() + Math.random().toString().substring(2, 8);
        localStorage.setItem('uid', uniqueId);
    }
}
window.addEventListener("DOMContentLoaded", () => {
    generateUniqueId();
    const phone_number = document.getElementById('phone_number_v3');
    phone_number.addEventListener('keyup', () => {
        const getPhoneNumber = document.getElementById('phone_number_v3').value;
        if (getPhoneNumber && (getPhoneNumber.length == 10) && phoneCheck.test(getPhoneNumber)) {
            userDetails.phone_number = getPhoneNumber;
            userDetails.pageName = window.location.href;
            userDetails.userDeviceDetails = navigator.userAgent;
            document.getElementById('error_message_v3').style.display = "none";
        } else {
            document.getElementById('error_message_v3').style.display = "block";
        }
    });
});
$('#homepage-download-cta').on('click', function() {
    $('.modal').modal('hide');
    $('#collect_user_details_v3').removeClass('hide');
    $('#collect_user_details_v3').modal('show');
});

function getDetails() {
    $('.modal').modal('hide');
    $('#collect_user_details_v3').modal('show');
}

function storeUserData(userDetails) {
    fetch('/api/ns/collect-number/', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userDetails)
    }).then(res => res.json()).catch(err => console.log(err))
}

function startDownload(source) {
    const downloadDetails = {
        body: {},
        queryObj: {},
    };
    downloadDetails.body.hostname = window.location.hostname;
    downloadDetails.body.pageName = window.location.pathname;
    downloadDetails.body.userDeviceDetails = navigator.userAgent;
    downloadDetails.body.uniqueId = localStorage.getItem('uid');
    downloadDetails.body.phoneNumber = userDetails.phone_number ? userDetails.phone_number : null;
    if(userDetails.phone_number) {
        sendUserInfoToDataLayer(userDetails);
    }
    const query = new URL(window.location.href).search.substring(1);
    const params = query.split('&');
    if (params.length >= 1) {
        for (const item of params) {
            const data = item.split('=');
            downloadDetails.queryObj[`${data[0]}`] = decodeURIComponent(data[1]);
            if (data[0] === 'referrer_code') {
                downloadDetails.referrer_code = decodeURIComponent(data[1]);
            }
        }
    }
    downloadDetails.body.cta = source ? source : 'button-download';
    downloadDetails.urlSource = document.referrer;
    flag = false;
    $('#collect_user_details_v3').modal('hide');
    dwnldDesktopApplicationNew(downloadDetails);
}
document.addEventListener('keydown', function(evt) {
    const element = $('#collect_user_details_v3');
    if (element.is(':visible')) {
        evt = evt || window.event;
        if (evt.keyCode == 27) {
            evt.preventDefault();
            storeUserData(userDetails)
            // if(isChrome()) {
            //     showDownloadPrompt();
            //     setTimeout(hideDownloadPrompt, 10000);
            // }
        }
    }
});

$('#demo-button, #download-close').on('click', function() {
    if (userDetails.phone_number && (userDetails.phone_number.length == 10) && phoneCheck.test(userDetails.phone_number)) {
        storeUserData(userDetails)
    }
    startDownload();
    // if(isChrome()) {
    //     showDownloadPrompt();
    //     setTimeout(hideDownloadPrompt, 10000);
    // }
    $('#collect_user_details_v3').modal('hide');
})

function createQueryString(data) {
    return Object.keys(data).map(key => {
        let val = data[key];
        if (val !== null && typeof val === 'object') val = createQueryString(val);
        return `${key}=${encodeURIComponent(`${val}`.replace(/\s/g,'_'))}`
    }).join('&')
}

function dwnldDesktopApplicationNew(searchParams) {
    var link = document.createElement('a');
    const queryString = createQueryString(searchParams);
    link.href = '/desktop/download-v3?' + queryString;
    link.dispatchEvent(new MouseEvent('click'));
    $('.close.desk-dwn').click();
}

function sendUserInfoToDataLayer(userDetails) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'downloadPhoneNumber',
        phoneNumber: `+91${userDetails.phone_number}`,
        pageName: userDetails.pageName,
    });
}
// function showDownloadPrompt() {
//     document.getElementById('fade-wrapper').style.display = 'block';
// }
// function hideDownloadPrompt() {
//     document.getElementById('fade-wrapper').style.display = 'none';
// }

// function isChrome() {
//     const userAgent = navigator.userAgent.toLowerCase();
//     return /chrome/.test(userAgent) && !/edge|edg|opera|opr|brave/.test(userAgent);
// }
