import { invitePeople } from './invite_modal.js';
function openEventModal(event) {
    window.currentEventId = event.id;

    document.getElementById("modal-event-title").innerText = event.title;
    document.getElementById("modal-event-description").innerText = event.description;
    document.getElementById("modal-event-date").innerText = event.date;
    document.getElementById("modal-event-category").innerText = event.category;
    document.getElementById("modal-event-creator").textContent = event.creatorUsername;

    const participantsList = document.getElementById("modal-event-participants");
    participantsList.innerHTML = "";

    event.participants.forEach(name => {
        const li = document.createElement("li");
        li.textContent = name;
        participantsList.appendChild(li);
    })

    const creatorSpan = document.getElementById("modal-event-creator");
    if (event.isCreator === true) {
        creatorSpan.textContent = "Yours"
    } else {
        creatorSpan.textContent = event.creatorUsername;
    }

    const leaveEventInput = document.getElementById("leave-event-id");
    const leaveBtn = document.getElementById("leave-button");

    if (event.isInvited && !event.isCreator) {
        leaveEventInput.value = event.id;
        leaveBtn.style.display = "inline-block";
    }
    else {
        leaveBtn.style.display = "none";
    }

    const editBtn = document.getElementById("edit-button");
    const inviteBtn = document.getElementById("invite-button");
    const reminderBtn = document.getElementById("reminder-button");

    if (event.isInvited) {
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
function bindModalActionButtons() {
    document.getElementById("edit-button")?.addEventListener("click", editEvent);
    document.getElementById("reminder-button")?.addEventListener("click", setReminder);
    document.getElementById("invite-button")?.addEventListener("click", invitePeople);

    document.getElementById("leave-button")?.addEventListener("click", () => {
        const form = document.getElementById("leave-event-form");
        const submitBtn = document.getElementById("leave-form-submit");

        if (form && submitBtn) {
            submitBtn.click();
        }
        else {
            console.error("Leave event form or submit button not found.")
        }
    })
}

function bindEventClickHandlers() {
    document.querySelectorAll(".calendar-event").forEach(btn => {
        btn.addEventListener("click", function () {
            console.log("📅 Event clicked:", this.dataset.id)

            const type = this.dataset.type;

            if (type === "national") {
                document.getElementById("modal-event-title").innerText = this.dataset.title;
                document.getElementById("modal-event-description").innerText = this.dataset.description;
                document.getElementById("modal-event-date").innerText = this.dataset.date;
                document.getElementById("modal-event-category").innerText = "National Holiday";
                document.getElementById("modal-event-creator").innerText = this.dataset.country;

                document.getElementById("edit-button").style.display = "none";
                document.getElementById("invite-button").style.display = "none";
                document.getElementById("reminder-button").style.display = "none";
                document.getElementById("leave-button").style.display = "none";

                new bootstrap.Modal(document.getElementById('eventModal')).show();
                return;
            }

            const event = {
                id: this.dataset.id,
                title: this.dataset.title,
                description: this.dataset.description,
                date: this.dataset.date,
                category: this.dataset.category,
                isInvited: this.dataset.isInvited === "true",
                creatorUsername: this.dataset.creator,
                isCreator: this.dataset.isCreator === "true",
                participants: this.dataset.participants ? this.dataset.participants.split(",") : []
            };
            openEventModal(event);
        });
    });
}
export function initEventModalUI() {
    bindEventClickHandlers();
    bindModalActionButtons();
}