function initCalendarUI() {
    document.querySelectorAll(".dropdown-arrow").forEach(arrow => {
        arrow.addEventListener("click", function () {
            const dropdownId = this.getAttribute("data-dropdown-id");
            const targetDropdown = document.getElementById(dropdownId);

            const allDropdowns = document.querySelectorAll(".event-dropdown");
            const allArrows = document.querySelectorAll(".dropdown-arrow");

            const isVisible = targetDropdown.classList.contains("show");

            allDropdowns.forEach(d => d.classList.remove("show"));
            allArrows.forEach(a => a.classList.remove("rotated"));

            if (!isVisible) {
                targetDropdown.classList.add("show");
                this.classList.add("rotated");
            }
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
}
//function toggleDropdown(dropdownId, arrowElement) {
//    const targetDropdown = document.getElementById(dropdownId);

//    const allDropdowns = document.querySelectorAll(".event-dropdown");
//    const allArrows = document.querySelectorAll(".dropdown-arrow");

//    const isVisible = targetDropdown.classList.contains("show");

//    allDropdowns.forEach(d => d.classList.remove("show"));
//    allArrows.forEach(a => a.classList.remove("rotated"));

//    if (!isVisible) {
//        targetDropdown.classList.add("show");
//        if (arrowElement) {
//            arrowElement.classList.add("rotated");
//        }
//    }
//}

//document.addEventListener("click", function (e) {
//    const isDropdownToggle = e.target.closest(".dropdown-arrow");
//    const isDropdown = e.target.closest(".event-dropdown");

//    if (!isDropdownToggle && !isDropdown) {
//        document.querySelectorAll(".event-dropdown").forEach(d => d.classList.remove("show"));
//        document.querySelectorAll(".dropdown-arrow").forEach(a => a.classList.remove("rotated"));
//    }
//});
