function openEventModal(event) {
    document.getElementById("modal-event-title").innerText = event.title;
    document.getElementById("modal-event-description").innerText = event.description;
    document.getElementById("modal-event-date").innerText = event.date;
    document.getElementById("modal-event-category").innerText = event.category;

    const modal = new bootstrap.Modal(document.getElementById('eventModal'));
    modal.show();
}
function editEvent() {
    console.log("Edit button clicked");
    // TODO: Show edit form, redirect to edit page, etc.
}

function setReminder() {
    console.log("Set reminder clicked");
    // TODO: Open reminder popup or scheduler
}

function invitePeople() {
    console.log("Invite people clicked");
    // TODO: Open invite form/modal
}

