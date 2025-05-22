document.addEventListener("DOMContentLoaded", function () {
   const filterForm = document.getElementById("eventFilterForm");
   const tableContainer = document.getElementById("event-table-container");

   if (!filterForm || !tableContainer) {
      return;
   }

   const triggerFilter = async () => {
      const formData = new FormData(filterForm);
      const search = formData.get("SearchTerm") || "";
      const categoryId = formData.get("CategoryId") || "";
      const ownerUsername = formData.get("OwnerUsername") || "";
      const date = formData.get("Date") || "";

      try {
         const res = await fetch(`/Admin/GetFilteredEventsPartial?search=${encodeURIComponent(search)}&categoryId=${encodeURIComponent(categoryId)}&ownerUsername=${encodeURIComponent(ownerUsername)}&date=${encodeURIComponent(date)}`);

         if (!res.ok) {
            throw new Error("Failed to fetch filtered events.");
         }
         const html = await res.text();
         tableContainer.innerHTML = html;
      }
      catch (error) {
         console.error("Error loading filtered events:", error);
      }
   };

   const searchInput = document.getElementById("search");

   if (searchInput) {
      searchInput.addEventListener("input", () => {
         triggerFilter();
      });
   }

   const dropdowns = ["category", "date"];
   dropdowns.forEach(id => {
      const el = document.getElementById(id);

      if (el) {
         el.addEventListener("change", function () {
            triggerFilter();
         });
      }
   });

   const ownerInput = document.getElementById("owner");
   if (ownerInput) {
      ownerInput.addEventListener("input", () => {
         triggerFilter();
      });
   }

   const resetBtn = document.getElementById("resetFiltersBtn");

   if (resetBtn) {
      resetBtn.addEventListener("click", () => {
         filterForm.reset();

         const category = document.getElementById("category");

         if (category) {
            category.value = "";
         }

         triggerFilter();
      });
   }

   const handleFormSubmit = (e) => {
      e.preventDefault();
      triggerFilter();
   };

   filterForm.addEventListener("submit", handleFormSubmit);
});