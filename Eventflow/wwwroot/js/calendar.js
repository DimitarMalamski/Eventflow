function toggleDropdown(id, arrowElement) {
    const target = document.getElementById(id);
    const allDropdowns = document.querySelectorAll(".event-dropdown");
    const allArrows = document.querySelectorAll(".dropdown-arrow");

    const isVisible = target.classList.contains("show");

    allDropdowns.forEach(d => d.classList.remove("show"));
    allArrows.forEach(a => a.classList.remove("rotated"));

    if (!isVisible) {
        target.classList.add("show");
        if (arrowElement) {
            arrowElement.classList.add("rotated");
        }
    }
}

document.addEventListener("click", function (e) {
    const isDropdownToggle = e.target.closest(".dropdown-arrow");
    const isDropdown = e.target.closest(".event-dropdown");

    if (!isDropdownToggle && !isDropdown) {
        document.querySelectorAll(".event-dropdown").forEach(d => d.classList.remove("show"));
    }
});
