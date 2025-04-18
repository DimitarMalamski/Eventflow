document.addEventListener("DOMContentLoaded", () => {
    const token = document.querySelector("input[name='__RequestVerificationToken']")?.value;

    document.body.addEventListener("click", async (e) => {
        if (e.target.classList.contains("like-star-btn")) {
            const button = e.target;
            const reminderId = button.getAttribute("data-id");

            if (!token || !reminderId) return;

            try {
                const res = await fetch("/Reminder/ToggleLike", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        "RequestVerificationToken": token
                    },
                    body: `id=${reminderId}`
                });

                const result = await res.json();

                if (result.success) {
                    button.classList.toggle("liked", result.liked);
                } else {
                    alert("Failed to update like state.");
                }

            } catch (err) {
                console.error("Toggle like failed:", err);
                alert("Something went wrong.");
            }
        }
    });
});