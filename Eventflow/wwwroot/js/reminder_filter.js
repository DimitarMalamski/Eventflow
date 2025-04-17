document.addEventListener("DOMContentLoaded", () => {
    const stateDropdown = document.querySelector("select[name='state']");
    const container = document.getElementById("reminderListContainer");

    if (!stateDropdown || !container) {
        return;
    }

    stateDropdown.addEventListener("change", async () => {
        const selectedState = stateDropdown.value;

        container.classList.remove("fade-in");
        container.classList.add("fade-out");

        container.addEventListener("transitionend", async function handler() {
            container.removeEventListener("transitionend", handler);

            try {
                const res = await fetch(`/Reminder/GetRemindersPartial?state=${selectedState}`);

                if (!res.ok) {
                    throw new Error("Failed to load reminders.");
                }

                const html = await res.text();
                container.innerHTML = html;

                container.classList.remove("fade-out");
                container.classList.add("fade-in");

                setTimeout(() => {
                    container.classList.remove("fade-in");
                }, 300);

            } catch (error) {
                console.log("Error loading reminders:", error);
                alert("Could not load reminders.");
                container.classList.remove("fade-out");
            }
        }, { once: true });
    });
});