function toggleDropdown(dropdownId, arrowElement) {
    const targetDropdown = document.getElementById(dropdownId);
    const allDropdowns = document.querySelectorAll(".event-dropdown");
    const allArrows = document.querySelectorAll(".dropdown-arrow");

    // Check if the target dropdown is already visible
    const isVisible = targetDropdown.classList.contains("show");

    // Close all dropdowns and reset the arrow rotation
    allDropdowns.forEach(d => d.classList.remove("show"));
    allArrows.forEach(a => a.classList.remove("rotated"));

    // If the target dropdown is not visible, show it and rotate the arrow
    if (!isVisible) {
        targetDropdown.classList.add("show");
        arrowElement.classList.add("rotated");
    }
}

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