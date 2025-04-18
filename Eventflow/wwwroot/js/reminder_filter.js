document.addEventListener("DOMContentLoaded", () => {
    const stateDropdown = document.querySelector("select[name='state']");
    const container = document.getElementById("reminderListContainer");

    if (!stateDropdown || !container) return;

    const loadReminders = async (url) => {
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
            const res = await fetch(url);
            if (!res.ok) throw new Error("Failed to load reminders.");

            const html = await res.text();
            container.innerHTML = html;

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

    stateDropdown.addEventListener("change", () => {
        const state = stateDropdown.value;
        loadReminders(`/Reminder/GetRemindersPartial?state=${state}`);
    });

    document.addEventListener("click", (e) => {
        if (e.target.matches(".reminder-page-btn")) {
            e.preventDefault();
            const page = e.target.dataset.page;
            const state = stateDropdown.value || "unread";
            loadReminders(`/Reminder/GetRemindersPartial?state=${state}&page=${page}`);
        }
    });
});