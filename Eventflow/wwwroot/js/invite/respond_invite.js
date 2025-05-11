export function initInviteResponse() {
   const bind = () => {
       document.querySelectorAll(".invite-response-btn").forEach(button => {
           if (!button.classList.contains("bound")) {
               button.classList.add("bound");

               button.addEventListener("click", async () => {
                   const inviteId = button.dataset.id;
                   const statusId = button.dataset.status;
                   const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

                   const card = document.getElementById(`reminder-${inviteId}`);
                   const wrapper = document.getElementById(`reminder-wrapper-${inviteId}`);

                   if (!token || !card || !wrapper) {
                       console.warn("Missing token or card elements");
                       return;
                   }

                   try {
                       const res = await fetch("/Invite/Respond", {
                           method: "POST",
                           headers: {
                               "Content-Type": "application/x-www-form-urlencoded"
                           },
                           body: `inviteId=${inviteId}&statusId=${statusId}&__RequestVerificationToken=${encodeURIComponent(token)}`
                       });

                        if (!res.ok) {
                           alert("Something went wrong.");
                           return;
                        }

                       card.classList.add("reminder-slide-out");
                       card.addEventListener("animationend", () => {
                           wrapper.style.height = wrapper.scrollHeight + "px";
                           void wrapper.offsetHeight;
                           wrapper.classList.add("collapsing");
                           wrapper.addEventListener("transitionend", () => {
                               wrapper.remove();
                           }, { once: true });
                       }, { once: true });

                   } catch (err) {
                       console.error("Error handling invite:", err);
                       alert(err.message || "Something went wrong.");
                   }
               });
           }
       });
   };

   bind();
   document.addEventListener("invites:updated", bind);
}
