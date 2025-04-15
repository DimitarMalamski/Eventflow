(() => {
    const state = localStorage.getItem("sidebarState");
    const html = document.documentElement;

    html.classList.remove("js-sidebar-loading");

    if (state === "collapsed") {
        html.classList.add("sidebar-collapsed-init");
    } else {
        html.classList.add("sidebar-expanded-init");
    }
})();

document.addEventListener("DOMContentLoaded", () => {
    const sidebarWrapper = document.getElementById("sidebarWrapper");
    const toggleBtn = document.getElementById("sidebarToggle");
    const toggleIcon = document.getElementById("toggleIcon");

    const isCollapsed = sidebarWrapper.classList.contains("sidebar-collapsed");

    toggleIcon.classList.toggle("bi-chevron-left", !isCollapsed);
    toggleIcon.classList.toggle("bi-chevron-right", isCollapsed);

    toggleBtn.addEventListener("click", () => {
        const isNowCollapsed = sidebarWrapper.classList.contains("sidebar-collapsed");

        sidebarWrapper.classList.toggle("sidebar-collapsed");
        sidebarWrapper.classList.toggle("sidebar-expanded");

        toggleIcon.classList.toggle("bi-chevron-left", isNowCollapsed);
        toggleIcon.classList.toggle("bi-chevron-right", !isNowCollapsed);

        const newState = isNowCollapsed ? "expanded" : "collapsed";

        document.cookie = `sidebarState=${newState}; path=/; max-age=31536000; SameSite=Lax`;});
});
