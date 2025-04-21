export function initReminderFilter() {
    const getFilterParams = () => {
        const state = document.querySelector("select[name='state']")?.value || "unread";
        const search = document.getElementById("searchInput")?.value?.trim() || "";
        const sortBy = document.getElementById("sortDropdown")?.value || "";
        return { state, search, sortBy };
    };

    const loadReminders = async (page = 1, state = "unread", search = "", sortBy = "") => {
        const container = document.getElementById("reminderListContainer");
        if (!container) return;

        container.classList.remove("fade-in");
        container.classList.add("fade-out");

        await new Promise((resolve) => {
            const handler = () => {
                container.removeEventListener("transitionend", handler);
                resolve();
            };
            container.addEventListener("transitionend", handler, { once: true });
        });

        try {
            const query = new URLSearchParams({ state, page, search, sortBy });
            const res = await fetch(`/Reminder/GetRemindersPartial?${query}`);
            if (!res.ok) throw new Error("Failed to load reminders.");

            const html = await res.text();
            container.innerHTML = html;

            setTimeout(() => {
                document.querySelector(".empty-state")?.classList.add("fade-in");
            }, 100);

            document.dispatchEvent(new Event("reminders:updated"));

            void container.offsetWidth;
            container.classList.remove("fade-out");
            container.classList.add("fade-in");

            setTimeout(() => container.classList.remove("fade-in"), 300);
        } catch (error) {
            console.error("Error loading reminders:", error);
            alert("Could not load reminders.");
            container.classList.remove("fade-out");
        }
    };

    const bindFilters = () => {
        const stateDropdown = document.querySelector("select[name='state']");
        const applyBtn = document.getElementById("applyFiltersBtn");

        if (!stateDropdown || !applyBtn) return;

        applyBtn.addEventListener("click", () => {
            const { state, search, sortBy } = getFilterParams();
            loadReminders(1, state, search, sortBy);
        });

        stateDropdown.addEventListener("change", () => {
            const { state, search, sortBy } = getFilterParams();
            loadReminders(1, state, search, sortBy);
        });

        document.addEventListener("click", (e) => {
            if (e.target.matches(".reminder-page-btn")) {
                e.preventDefault();
                const page = e.target.dataset.page;
                const state = e.target.dataset.state || "unread";
                const search = e.target.dataset.search || "";
                const sortBy = e.target.dataset.sort || "";

                if (page) loadReminders(parseInt(page), state, search, sortBy);
            }
        });
    };

    bindFilters();
}
