document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", async (e) => {
        if (e.target.classList.contains("mark-as-read-btn")) {
            const button = e.target;
            const reminderId = button.getAttribute("data-id");

            const card = document.getElementById(`reminder-${reminderId}`);
            const wrapper = document.getElementById(`reminder-wrapper-${reminderId}`);
            const token = document.querySelector("input[name='__RequestVerificationToken']")?.value;

            if (!token || !card || !wrapper) return;

            button.disabled = true;

            try {
                const res = await fetch("/Reminder/MarkAsRead", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        "RequestVerificationToken": token
                    },
                    body: `id=${reminderId}`
                });

                if (!res.ok) {
                    throw new Error("Failed to mark as read.");
                }

                card.classList.add("reminder-slide-out");

                card.addEventListener("animationend", () => {
                    const wrapperHeight = wrapper.scrollHeight + "px";
                    wrapper.style.height = wrapperHeight;

                    void wrapper.offsetHeight;

                    wrapper.classList.add("collapsing");

                    wrapper.addEventListener("transitionend", () => {
                        wrapper.remove();
                    }, { once: true });
                });

            } catch (error) {
                console.error("Error marking reminder as read:", error);
                alert("Something went wrong.");
            }
        }
    });
});
