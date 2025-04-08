function openEventModal(event) {
    document.getElementById("modal-event-title").innerText = event.title;
    document.getElementById("modal-event-description").innerText = event.description;
    document.getElementById("modal-event-date").innerText = event.date;
    document.getElementById("modal-event-category").innerText = event.category;

    const modal = new bootstrap.Modal(document.getElementById('eventModal'));
    modal.show();
}
function editEvent() {
    // TODO
}

function setReminder() {
    // TODO
}

function invitePeople() {
    // TODO
}

