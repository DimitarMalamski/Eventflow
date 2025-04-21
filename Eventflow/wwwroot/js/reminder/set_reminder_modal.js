import { checkNotificationDots } from "../messages/check_notification_dots.js";
export function openSetReminderModal() {
    console.log("🔔 openSetReminderModal called. Current Event ID:", window.currentEventId);


    const eventId = window.currentEventId;

    if (!eventId) {
        alert("Event ID not available. Open an event first.");
        return;
    }

    const input = document.getElementById("reminder-event-id");

    if (input) {
        input.value = eventId;
        console.log("✅ Reminder input value set to:", input.value); 
    }

    const reminderForm = document.getElementById("setReminderForm");
    if (reminderForm) {
        const clone = reminderForm.cloneNode(true);
        reminderForm.replaceWith(clone);

        clone.addEventListener("submit", handleReminderFormSubmit);
    }

    const reminderModal = new bootstrap.Modal(document.getElementById("setReminderModal"));
    reminderModal.show();
}

async function handleReminderFormSubmit(e) {
    e.preventDefault();

    const formData = new FormData(e.target);

    const reminder = {
        title: formData.get("Title"),
        description: formData.get("Description"),
        reminderDate: new Date(formData.get("ReminderDate")).toISOString(),
        personalEventId: parseInt(formData.get("PersonalEventId"))
    };

    try {
        const response = await fetch("/Reminder/Create", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']").value
            },
            body: JSON.stringify(reminder)
        });

        const result = await response.json();

        console.log("Reminder save result:", result);

        if (result.success) {
            bootstrap.Modal.getInstance(document.getElementById("setReminderModal")).hide();

            Swal.fire({
                icon: 'success',
                title: 'Reminder Set!',
                text: result.message,
                timer: 2000,
                showConfirmButton: false
            });

            reminderForm.reset();

            checkNotificationDots();
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Reminder Failed',
                text: result.message || 'Something went wrong.'
            });
        }
    } catch (err) {
        console.error("Reminder save failed:", err);
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Failed to set reminder. Please try again.'
        });
    }
}

const reminderForm = document.getElementById("setReminderForm");

document.addEventListener("DOMContentLoaded", function () {
    if (reminderForm) {
        reminderForm.addEventListener("submit", handleReminderFormSubmit);
    }
});
