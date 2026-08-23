(function () {
    "use strict";
    const key = "pathandpaws_cookie_consent";

    function showBanner() {
        if (document.getElementById("cookie-consent-banner")) return;

        const banner = document.createElement("aside");
        banner.id = "cookie-consent-banner";
        banner.className = "cookie-consent";
        banner.setAttribute("role", "dialog");
        banner.setAttribute("aria-labelledby", "cookie-consent-title");

        banner.innerHTML = `
            <div class="cookie-consent__content">
                <h2 id="cookie-consent-title">Cookies on Path &amp; Paws</h2>
                <p>We use essential cookies to make this website work properly. We do not use advertising or tracking cookies.</p>
                <a href="/cookie-policy.html">Find out more</a>
            </div>
            <div class="cookie-consent__actions">
                <button type="button" data-choice="reject">Reject non-essential</button>
                <button type="button" data-choice="accept">Accept</button>
            </div>`;

        document.body.appendChild(banner);

        banner.querySelectorAll("[data-choice]").forEach(button => {
            button.addEventListener("click", () => {
                try { localStorage.setItem(key, button.dataset.choice); } catch {}
                banner.remove();
            });
        });
    }

    function init() {
        let choice = null;
        try { choice = localStorage.getItem(key); } catch {}
        if (!choice) showBanner();
    }

    if (document.readyState === "loading")
        document.addEventListener("DOMContentLoaded", init);
    else
        init();

    window.PathAndPawsCookieSettings = showBanner;
})();