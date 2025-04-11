function openEventModal(event) {
    window.currentEventId = event.id;

    document.getElementById("modal-event-title").innerText = event.title;
    document.getElementById("modal-event-description").innerText = event.description;
    document.getElementById("modal-event-date").innerText = event.date;
    document.getElementById("modal-event-category").innerText = event.category;

    const isInvited = event.isInvited === true || event.isInvited === "true";

    const editBtn = document.getElementById("edit-button");
    const inviteBtn = document.getElementById("invite-button");
    const reminderBtn = document.getElementById("reminder-button");

    if (isInvited) {
        editBtn.style.display = "none";
        inviteBtn.style.display = "none";
        reminderBtn.style.display = "inline-block";
    } else {
        editBtn.style.display = "inline-block";
        inviteBtn.style.display = "inline-block";
        reminderBtn.style.display = "inline-block";
    }

    const modal = new bootstrap.Modal(document.getElementById('eventModal'));
    modal.show();
}
function editEvent() {
    const eventId = window.currentEventId;

    if (eventId) {
        window.location.href = `/Event/Edit/${eventId}`
    }
    else {
        alert("Event ID is missing.");
    }
    
}

function setReminder() {
    openSetReminderModal();
}

function invitePeople() {
    openInviteModal();
}

