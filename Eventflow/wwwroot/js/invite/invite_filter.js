export function initInviteFilter() {
   const getFilterParams = () => {
       const statusId = document.querySelector("select[name='statusId']")?.value || "1";
       const search = document.getElementById("inviteSearchInput")?.value?.trim() || "";
       const sortBy = document.getElementById("inviteSortDropdown")?.value || "";
       return { statusId, search, sortBy };
   };

   const loadInvites = async (page = 1, statusId = "1", search = "", sortBy = "") => {
       const container = document.getElementById("inviteListContainer");
       if (!container) return;

       container.classList.remove("fade-in");
       container.classList.add("fade-out");

       await new Promise((resolve) => {
           const handler = () => {
               container.removeEventListener("transitionend", handler);
               resolve();
           };
           container.addEventListener("transitionend", handler, { once: true });
       });

       try {
           const query = new URLSearchParams({ statusId, page, search, sortBy });
           const res = await fetch(`/Invite/GetInvitesPartial?${query}`);
           if (!res.ok) throw new Error("Failed to load invites.");

           const html = await res.text();
           container.innerHTML = html;

           document.dispatchEvent(new Event("invites:updated"));

           void container.offsetWidth;
           container.classList.remove("fade-out");
           container.classList.add("fade-in");

           setTimeout(() => container.classList.remove("fade-in"), 300);
       } catch (error) {
           console.error("Error loading invites:", error);
           alert("Could not load invites.");
           container.classList.remove("fade-out");
       }
   };

   const bindFilters = () => {
       const statusDropdown = document.querySelector("select[name='statusId']");
       const applyBtn = document.getElementById("applyInviteFiltersBtn");

       if (!statusDropdown || !applyBtn) return;

       applyBtn.addEventListener("click", () => {
           const { statusId, search, sortBy } = getFilterParams();
           loadInvites(1, statusId, search, sortBy);
       });

       statusDropdown.addEventListener("change", () => {
           const { statusId, search, sortBy } = getFilterParams();
           loadInvites(1, statusId, search, sortBy);
       });

       document.addEventListener("click", (e) => {
           if (e.target.matches(".reminder-page-btn")) {
               e.preventDefault();
               const page = e.target.dataset.page;
               const statusId = e.target.dataset.state || "1";
               const search = e.target.dataset.search || "";
               const sortBy = e.target.dataset.sort || "";

               if (page) loadInvites(parseInt(page), statusId, search, sortBy);
           }
       });
   };

   bindFilters();
}
