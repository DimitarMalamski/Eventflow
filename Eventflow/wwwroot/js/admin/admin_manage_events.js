document.addEventListener("DOMContentLoaded", () => {
   const viewButtons = document.querySelectorAll(".btn-view-event");

   viewButtons.forEach(button => {
      button.addEventListener("click", () => {
         const row = button.closest("tr");

         if (!row) {
            return;
         }

         const title = row.dataset.eventTitle;
         const description = row.dataset.eventDescription;
         const date = row.dataset.eventDate;
         const owner = row.dataset.eventOwner;
         const category = row.dataset.eventCategory;
         const participantsRaw = row.dataset.eventParticipants;

         document.getElementById("modalEventTitle").textContent = title || "Unknown";
         document.getElementById("modalEventDescription").textContent = description || "No Description";
         document.getElementById("modalEventDate").textContent = date || "Unknown";
         document.getElementById("modalEventOwner").textContent = owner || "Unknown";
         document.getElementById("modalEventCategory").textContent = category || "None";

         const list = document.getElementById("modalEventParticipants");
         list.innerHTML = "";

         if (!participantsRaw) {
            list.innerHTML = `<li class="list-group-item text-muted">No participants</li>`;
            return;
         }

         let participants;

         try {
            participants = JSON.parse(participantsRaw);
         }
         catch (error) {
            list.innerHTML = `<li class="list-group-item text-danger">Error parsing participants</li>`;
            return;
         }

         if (participants.length === 0) {
            list.innerHTML = `<li class="list-group-item text-muted">No participants</li>`;
            return;
         }

         console.log("Parsed participants:", participants);
         
         participants.forEach(p => {
            const badgeClass = getStatusBadge(p.status);

            const li = document.createElement("li");
            li.className = "list-group-item d-flex justify-content-between align-items-center";

            li.innerHTML = `
               <div>
                  <strong>${p.username}</strong>
                  <small class="text-muted">(${p.email})</small>
               </div>
               <span class="badge ${badgeClass}">${p.status}</span>
            `;

            list.appendChild(li);
         });
      });
   });

   function getStatusBadge(status) {
      if (!status || typeof status !== "string") {
         return "bg-secondary";
      }

      switch (status.toLowerCase()) {
         case "accepted":
            return "bg-success";
         case "declined":
            return "bg-danger";
         case "pending":
            return "bg-warning text-dark";
         default:
            return "bg-secondary";
      }
   }
});