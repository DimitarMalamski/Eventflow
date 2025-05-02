document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", function (e) {
        if (e.target.classList.contains("calendar-nav")) {
            e.preventDefault();

            const month = e.target.getAttribute("data-month");
            const year = e.target.getAttribute("data-year");

            const countrySelect = document.getElementById("countrySelect");
            const countryId = countrySelect?.value;

            const view = document.getElementById("calendarView")?.value;

            let url = "";

            if (view === "national") {
                if (!countryId || countryId === "0") {
                    url = `/Calendar/LoadEmptyCalendarPartial?month=${month}&year=${year}`;
                } else {
                    url = `/Calendar/LoadCalendarByCountryPartial?countryId=${countryId}&month=${month}&year=${year}`;
                }
            } else if (view === "personal") {
                url = `/Event/LoadPersonalCalendarPartial?month=${month}&year=${year}`;
            }

            if (!url) {
                console.warn("Calendar navigation blocked: view or params missing.");
                return;
            }

            console.log("Navigating to calendar URL:", url);

            fetch(url)
                .then(res => res.text())
                .then(html => {
                    const outer = document.getElementById("calendarOuterWrapper");
                    if (outer) {
                        outer.innerHTML = html;

                        document.dispatchEvent(new Event("calendar:updated"));

                        if (typeof initCalendarUI === "function") {
                            initCalendarUI();
                        }
                    } else {
                        console.error("calendarOuterWrapper not found in DOM.");
                    }
                })
                .catch(err => console.error("Calendar AJAX load failed:", err));
        }
    });

    if (typeof initCalendarUI === "function") {
        initCalendarUI();
    }
});