function bindLikeStarButtons() {
    document.querySelectorAll(".like-star-btn").forEach(star => {
        if (!star.classList.contains("bound")) {
            star.classList.add("bound")

            star.addEventListener("click", async function () {
                const reminderId = this.dataset.id;
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

                this.disabled = true;

                try {
                    const res = await fetch("/Reminder/ToggleLike", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded",
                        },
                        body: `id=${reminderId}&__RequestVerificationToken=${encodeURIComponent(token)}`
                    });

                    const result = await res.json();
                    if (result.success) {
                        this.classList.toggle("liked", result.liked);
                    } else {
                        alert("Error toggling like.")
                    }
                } catch (error) {
                    console.error("Toggle failed", error);
                    alert("Something went wrong!");
                } finally {
                    this.disabled = false;
                }
            });
        }
    });
}

// DOM load
document.addEventListener("DOMContentLoaded", bindLikeStarButtons);

// Partail loads
document.addEventListener("reminders:updated", bindLikeStarButtons);