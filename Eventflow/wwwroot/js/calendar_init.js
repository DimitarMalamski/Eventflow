window.initCalendarUI = function () {
    document.querySelectorAll(".dropdown-arrow").forEach(arrow => {
        arrow.addEventListener("click", function () {
            const dropdownId = this.getAttribute("data-dropdown-id");
            toggleDropdown(dropdownId, this);
        });
    });

    document.addEventListener("click", function (e) {
        const isDropdownToggle = e.target.closest(".dropdown-arrow");
        const isDropdown = e.target.closest(".event-dropdown");

        if (!isDropdownToggle && !isDropdown) {
            document.querySelectorAll(".event-dropdown").forEach(d => d.classList.remove("show"));
            document.querySelectorAll(".dropdown-arrow").forEach(a => a.classList.remove("rotated"));
        }
    });
};