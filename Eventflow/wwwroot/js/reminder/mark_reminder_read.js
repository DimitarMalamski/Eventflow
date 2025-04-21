export function initMarkAsRead() {
    const bind = () => {
        document.querySelectorAll(".mark-as-read-btn").forEach(button => {
            if (!button.classList.contains("bound")) {
                button.classList.add("bound");

                button.addEventListener("click", async () => {
                    const reminderId = button.dataset.id;
                    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

                    const card = document.getElementById(`reminder-${reminderId}`);
                    const wrapper = document.getElementById(`reminder-wrapper-${reminderId}`);

                    if (!token || !card || !wrapper) {
                        return;
                    }

                    button.disabled = true;

                    try {
                        const res = await fetch("/Reminder/MarkAsRead", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/x-www-form-urlencoded",
                            },
                            body: `id=${reminderId}&__RequestVerificationToken=${encodeURIComponent(token)}`
                        });

                        if (!res.ok) {
                            throw new Error("Failed to mark as read.");
                        }

                        card.classList.add("reminder-slide-out");

                        card.addEventListener("animationend", () => {

                            wrapper.style.height = wrapper.scrollHeight + "px";
                            void wrapper.offsetHeight;
                            wrapper.classList.add("collapsing");
                            wrapper.addEventListener("transitionend", () => {

                                wrapper.remove();
                            }, { once: true });
                        }, { once: true });

                    } catch (err) {
                        console.error("Error marking reminder as read:", err);
                        alert("Something went wrong.");
                    }
                });
            }
        });
    }

    bind();
    document.addEventListener("reminders:updated", bind);
}