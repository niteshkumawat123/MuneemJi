let $ipInfoManager = (function() {
    function getExpirationForIpInfoData() {
        let currentDate = new Date();
        let next7thDayDate = new Date(currentDate.getTime() + (7 * 24 * 60 * 60 * 1000));
        return next7thDayDate.getTime();
    }

    function getCountryInfo() {
        let ipInfo = localStorage.getItem('ipInfo');
        let currentDate = new Date();
        ipInfo = ipInfo ? JSON.parse(ipInfo) : null;
        if (ipInfo && ipInfo.expirationTime && ipInfo.expirationTime > currentDate.getTime()) {
            return ipInfo;
        }
        return null;
    }

    function getIpInfo(callback) {
        let countryInfo = getCountryInfo();
        if (countryInfo) {
            callback(countryInfo);

        } else {
            $.getJSON('https://ipinfo.io/json', function(data) {
                data.expirationTime = getExpirationForIpInfoData();
                localStorage.setItem('ipInfo', JSON.stringify(data));
                callback(data);
            });
        }
    }

    function getOnlyCountryInfo(callback) {
        let countryInfo = getCountryInfo();
        if (countryInfo) {
            callback(countryInfo.country);
        } else {
            $.getJSON('https://ipinfo.io/json', function(data) {
                data.expirationTime = getExpirationForIpInfoData();
                localStorage.setItem('ipInfo', JSON.stringify(data));
                callback(data.country);
            });
        }
    }
    return {
        getIpInfo,
        getOnlyCountryInfo
    };
})();