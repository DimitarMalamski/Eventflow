document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", function (e) {
        if (e.target.classList.contains("calendar-nav")) {
            e.preventDefault();

            const month = e.target.getAttribute("data-month");
            const year = e.target.getAttribute("data-year");

            fetch(`/Calendar/LoadCalendarPartial?month=${month}&year=${year}`)
                .then(res => res.text())
                .then(html => {
                    const outer = document.getElementById("calendarOuterWrapper");
                    if (outer) {
                        outer.innerHTML = html;

                        console.log("🧠 Calendar updated via AJAX"); // ✅ Add this!

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