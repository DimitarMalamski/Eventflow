document.addEventListener("DOMContentLoaded", () => {
    let selectedEventId = null;

    const deleteButtons = document.querySelectorAll(".btn-delete-event");
    const modal = new bootstrap.Modal(document.getElementById("deleteEventModal"));
    const confirmBtn = document.getElementById("confirmDeleteEventBtn");

    deleteButtons.forEach(button => {
        button.addEventListener("click", () => {
            selectedEventId = button.dataset.eventId;
            const title = button.dataset.eventTitle;

            document.getElementById("deleteEventTitle").textContent = title || "this event";
            document.getElementById("deleteEventId").value = selectedEventId;

            modal.show();
        });
    });

    confirmBtn.addEventListener("click", async () => {
        if (!selectedEventId) return;

        try {
            const res = await fetch("/Admin/DeleteEvent", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ id: parseInt(selectedEventId) })
            });

            const result = await res.json();

            if (result.success) {
                const row = document.querySelector(`tr[data-event-id="${selectedEventId}"]`);
                if (row) {
                    row.remove();
                }
                modal.hide();
            } else {
                alert(result.message || "Failed to delete event.");
            }
        } catch (err) {
            console.error("Error deleting event:", err);
            alert("An error occurred while deleting the event.");
        }
    });
});
