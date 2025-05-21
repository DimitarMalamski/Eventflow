document.addEventListener("DOMContentLoaded", () => {
   const editButtons = document.querySelectorAll(".btn-edit-event");
   const form = document.getElementById("editEventForm");
   const participantsContainer = document.getElementById("editParticipantsContainer");

   editButtons.forEach(button => {
      button.addEventListener("click", () => {
         const row = button.closest("tr");

         if (!row) {
             return;
         }

         const eventId = row.dataset.eventId;
         const title = row.dataset.eventTitle;
         const description = row.dataset.eventDescription;
         const date = row.dataset.eventDate;
         const categoryName = row.dataset.eventCategory;
         const participantsRaw = row.dataset.eventParticipants;

         document.getElementById("editEventId").value = eventId;
         document.getElementById("editTitle").value = title;
         document.getElementById("editDescription").value = description;
         document.getElementById("editDate").value = date;
         document.getElementById("editCategory").value = getCategoryIdByName(categoryName);

         renderEditModalParticipants(participantsRaw, eventId);

         const modal = new bootstrap.Modal(document.getElementById("editEventModal"));
         modal.show(); 
      });
   });

   form.addEventListener("submit", async (e) => {
      e.preventDefault();

      const formData = new FormData(form);
      const json = Object.fromEntries(formData.entries());

      try {
         const res = await fetch("/Admin/Edit", {
            method: "POST",
            headers: {
               "Content-Type": "application/json"
            },
            body: JSON.stringify(json)
         });

         if (!res.ok) {
            throw new Error("Failed to update event");
         }

         const updatedEvent = await res.json();
         const row = document.querySelector(`tr[data-event-id="${updatedEvent.eventId}"]`);

         if (row) {
            row.dataset.eventTitle = updatedEvent.title;
            row.dataset.eventDescription = updatedEvent.description;
            row.dataset.eventDate = updatedEvent.date;
            row.dataset.eventCategory = updatedEvent.categoryName;

            row.querySelector("td:nth-child(1)").textContent = updatedEvent.title;
            row.querySelector("td:nth-child(2)").textContent = updatedEvent.description || "No description";
            row.querySelector("td:nth-child(3)").textContent = updatedEvent.date;
            row.querySelector("td:nth-child(5)").textContent = updatedEvent.categoryName || "None";
         }

         const updatedParticipants = [];
         document.querySelectorAll("#editParticipantsContainer .participant-row").forEach(pRow => {
            updatedParticipants.push({
               userId: parseInt(pRow.dataset.userId),
               username: pRow.querySelector("strong").textContent,
               email: pRow.querySelector("small").textContent.replace(/[()]/g, ""),
               status: pRow.dataset.status
            });
         });

         row.dataset.eventParticipants = JSON.stringify(updatedParticipants);

         bootstrap.Modal.getInstance(document.getElementById("editEventModal")).hide();
      } catch (err) {
         alert(err.message);
         console.error("Edit event error:", err);
      }
   });

   function getCategoryIdByName(categoryName) {
      const options = document.querySelectorAll("#editCategory option");
      for (let opt of options) {
         if (opt.textContent.trim() === categoryName?.trim()) {
            return opt.value;
         }
      }

      return "";
   }

   function renderEditModalParticipants(participantsRaw, eventId) {
      participantsContainer.innerHTML = "";

      if (!participantsRaw) {
         participantsContainer.innerHTML = `<div class="text-muted">No participants</div>`;
         return;
      }

      let participants;
      try {
         participants = JSON.parse(participantsRaw);
      } catch (error) {
         participantsContainer.innerHTML = `<div class="text-danger">Error loading participants</div>`;
         return;
      }

      if (participants.length === 0) {
         participantsContainer.innerHTML = `<div class="text-muted">No participants</div>`;
         return;
      }

      participants.forEach(p => {
         const row = document.createElement("div");
         row.className = "participant-row d-flex justify-content-between align-items-center";
         row.dataset.userId = p.userId;
         row.dataset.status = p.status;

         row.innerHTML = `
            <div>
               <strong>${p.username}</strong>
               <small class="text-muted ms-2">(${p.email})</small>
               <span class="text-muted ms-2">[${p.status}]</span>
            </div>
            <button type="button" class="btn btn-sm btn-outline-danger remove-participant-btn">➖</button>
         `;

         row.querySelector(".remove-participant-btn").addEventListener("click", () => {
            removeParticipant(row, eventId);
         });

         participantsContainer.appendChild(row);
      });
   }

   async function removeParticipant(row, eventId) {
      const userId = row.dataset.userId;
      const status = row.dataset.status.toLowerCase();

      // If Declined, remove from UI only
      if (status === "declined") {
         row.remove();
         return;
      }

      try {
         const res = await fetch("/Admin/RemoveParticipant", {
            method: "POST",
            headers: {
               "Content-Type": "application/json"
            },
            body: JSON.stringify({
               eventId: parseInt(eventId),
               userId: parseInt(userId)
            })
         });

         if (res.ok) {
            row.remove();
         } else {
            alert("Failed to remove participant");
         }
      } catch (err) {
         console.error("Remove participant error:", err);
         alert("Error removing participant");
      }
   }
});