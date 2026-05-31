function generateUniqueId() {
    if (!localStorage.getItem('uid')) {
        const uniqueId = new Date().valueOf() + Math.random().toString().substring(2, 8);
        localStorage.setItem('uid', uniqueId);
    }
}
window.addEventListener("DOMContentLoaded", () => {
    generateUniqueId();
    const youtubeElement = document.getElementById('play-youtube-video');
    if (youtubeElement) {
        youtubeElement.addEventListener("click", function() {
            document.getElementById('play-youtube-video').style.display = 'none';
            document.getElementById('youtube-video-play').style.display = 'block';
        });
    }
    const phone_number = document.getElementById('phone_number');
    if (phone_number) {
        phone_number.addEventListener('keyup', () => {
            const getPhoneNumber = document.getElementById('phone_number').value;
            if (getPhoneNumber && (getPhoneNumber.length == 10) && phoneCheck.test(getPhoneNumber)) {
                userDetails.phone_number = getPhoneNumber;
                userDetails.pageName = window.location.href;
                userDetails.userDeviceDetails = navigator.userAgent;
                document.getElementById('error_message').style.display = "none";
            } else {
                document.getElementById('error_message').style.display = "block";
            }
        });
    }
});
$('.contact-popup, .roll-button').on('click', function() {
    const url = window.location.pathname;
    if (url.includes('format')) {
        startDownload('button-download');
    } else {
        $('#collect_user_details').modal('show');
    }
});
const phoneCheck = /[6-9]{1}[0-9]{9}/;
const userDetails = {};

function getDetails() {
    $('.modal').modal('hide');
  	const url = window.location.pathname;
    if (url.includes('format')) {
        startDownload('button-download');
    } else {
        $('#collect_user_details').modal('show');
    }
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
    downloadDetails.urlSource = document.referrer;
    downloadDetails.body.cta = source ? source : 'button-download';
    if (userDetails.phone_number) {
        storeUserData(userDetails);
    }
    dwnldDesktopApplicationNew(downloadDetails);
    $('#collect_user_details').modal('hide');
}
document.addEventListener('keydown', function(evt) {
    const element = $('#collect_user_details');
    if (element.is(':visible')) {
        evt = evt || window.event;
        if (evt.keyCode == 27) {
            evt.preventDefault();
            startDownload();
        }
    }
});

function createQueryString(data) {
    return Object.keys(data).map(key => {
        let val = data[key];
        if (val !== null && typeof val === 'object') val = createQueryString(val);
        return `${key}=${encodeURIComponent(`${val}`.replace(/\s/g,'_'))}`
    }).join('&')
}

function sendUserInfoToDataLayer(userDetails) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'downloadPhoneNumber',
        phoneNumber: `+91${userDetails.phone_number}`,
        pageName: userDetails.pageName,
    });
}

function dwnldDesktopApplicationNew(searchParams) {
    var link = document.createElement('a');
    const queryString = createQueryString(searchParams);
    link.href = '/desktop/download-v3?' + queryString;
    link.dispatchEvent(new MouseEvent('click'));
    $('.close.desk-dwn').click();
}