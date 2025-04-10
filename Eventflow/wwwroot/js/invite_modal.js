function openInviteModal() {
    const eventId = window.currentEventId;

    if (!eventId) {
        alert("Event ID not available. Open an event first.");
        return;
    }

    const input = document.getElementById("invite-event-id");

    if (input) {
        input.value = eventId;
    }

    const inviteModal = new bootstrap.Modal(document.getElementById("inviteModal"));
    inviteModal.show();
}

document.addEventListener("DOMContentLoaded", function () {
    const inviteForm = document.getElementById("inviteForm");

    if (inviteForm) {
        inviteForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            const username = document.getElementById("invite-username").value;
            const eventId = document.getElementById("invite-event-id").value;

            try {
                const response = await fetch("/Invite/Send", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({ eventId, username })
                });

                const result = await response.json();

                if (result.success) {
                    alert("✅ Invite sent!");
                    bootstrap.Modal.getInstance(document.getElementById("inviteModal")).hide();
                } else {
                    alert(result.message || "Something went wrong.");
                }
            } catch (err) {
                console.error(err);
                alert("Failed to send invite.");
            }
        });
    }
});