export function openInviteModal() {
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

export function initInviteModal() {
    const inviteForm = document.getElementById("inviteForm");

    if (inviteForm && !inviteForm.classList.contains("bound")) {
        inviteForm.classList.add("bound");

        inviteForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            const username = document.getElementById("invite-username").value;
            const eventId = document.getElementById("invite-event-id").value;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            try {
                const response = await fetch("/Invite/Send", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify({ eventId: parseInt(eventId), username })
                });

                const result = await response.json();

                if (result.success) {
                    bootstrap.Modal.getInstance(document.getElementById("inviteModal")).hide();

                    Swal.fire({
                        icon: 'success',
                        title: 'Invite Sent!',
                        text: `Your invitation has been sent successfully.`,
                        timer: 2000,
                        showConfirmButton: false
                    });

                    inviteForm.reset();

                    if (typeof checkNotificationDots === "function") {
                        checkNotificationDots();
                    }
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Invite Failed',
                        text: result.message || 'Something went wrong.'
                    });
                }
            } catch (err) {
                Swal.fire({
                    icon: 'error',
                    title: 'Invite Failed',
                    text: 'Failed to send invite. Please try again.'
                });
            }
        });
    }
};

export function invitePeople() {
    openInviteModal();
}

window.openInviteModal = openInviteModal;