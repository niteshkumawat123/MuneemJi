let totalRatingCount = 0,
    ratingSum = 0;
document.addEventListener("DOMContentLoaded", () => {
    let showValue = document.querySelector('#rating-value');
    fetch('/api/ns/rating/get-rating', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            page_url: window.location.href
        })
    }).then(res => res.json()).then((res) => {
        let schema = [];
        totalRatingCount = res.data.ratingData.totalCount;
        ratingSum = res.data.ratingData.ratingSum;

        let ratingAvg = 0;
        if (totalRatingCount === 0) {
            showValue.innerHTML = `Average rating 0 / 5. Vote count: 0`;
        } else {
            ratingAvg = ratingSum / totalRatingCount;
            showValue.innerHTML =
                `Average rating ${ratingAvg.toFixed(2)} / 5. Vote count: ${totalRatingCount}`;
        }
        schema = [
            {
                "@context": "http://schema.org",
                "@type": "SoftwareApplication",
                "name": "Vyapar App",
                "url": "https://vyaparapp.in",
                "applicationCategory": "Billing Software",
                "downloadUrl": "https://desktop.vypcdn.in/VyaparApp.exe",
                "operatingSystem": ["Windows", "Android", "iOS"],
                "offers": {
                    "@type": "Offer",
                    "price": "1.00",
                    "priceCurrency": "INR"
                },
                "screenshot": "https://vyaparwebsiteimages.vypcdn.in/marketing-images/images/home-page-revamp/vyaapr_hero.webp",
                "aggregateRating": { "@type": "AggregateRating", "bestRating": 5, "ratingCount": totalRatingCount, "ratingValue": ratingAvg.toFixed(2) },
            }
        ];
        let script = document.createElement('script');
        script.type = "application/ld+json";
        script.text = JSON.stringify(schema);
        document.querySelector('head').appendChild(script);
        for (let i = 0.5; i <= ratingAvg; i += 0.5) {
            document.getElementsByClassName(`star${i}`)[0].style.color = "#f5a623";
        }
    }).catch(err => console.log(err));
    let star = document.querySelectorAll('input');
    let ratingGreat = document.querySelector('#rating-great');
    const ratingDetails = {
        page_url: window.location.href,
        os_details: navigator.userAgent,
    }
    for (let i = 0; i < star.length; i++) {
        star[i].addEventListener('click', function () {
            i = this.value;
            ratingDetails.rating = Number(i);
            if (ratingDetails.rating) {
                let avgRating = (ratingSum + parseFloat(ratingDetails.rating)) / (totalRatingCount + 1);
                showValue.innerHTML =
                    `Average rating ${avgRating.toFixed(2)} / 5. Vote count: ${totalRatingCount + 1}`;
                for (let i = 0.5; i <= 5; i = i + 0.5) {
                    document.getElementsByClassName(`star${i}`)[0].style.color = "#ddd";
                }
                for (let i = 0.5; i <= ratingDetails.rating; i = i + 0.5) {
                    document.getElementsByClassName(`star${i}`)[0].style.color = "#f5a623";
                }
                fetch('/api/ns/rating', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(ratingDetails)
                }).then(res => res.json()).then(data => {
                    document.getElementsByClassName('brfore-rating-msg')[0].style.display =
                        "none";
                }).catch(err => {
                    console.log('error', err);
                    document.getElementsByClassName('brfore-rating-msg')[0].style.display =
                        "none";
                })
                ratingGreat.innerHTML = "Thank you for rating this post ";
            }
        });
    }
});