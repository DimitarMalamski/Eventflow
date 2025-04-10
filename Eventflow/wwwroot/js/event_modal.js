function openEventModal(event) {
    window.currentEventId = event.id;

    document.getElementById("modal-event-title").innerText = event.title;
    document.getElementById("modal-event-description").innerText = event.description;
    document.getElementById("modal-event-date").innerText = event.date;
    document.getElementById("modal-event-category").innerText = event.category;

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
    // TODO
}

function invitePeople() {
    openInviteModal()
}

