(() => {
    const wrapper = document.getElementById("sidebarWrapper");
    const toggleIcon = document.getElementById("toggleIcon");

    if (!wrapper || !toggleIcon) return;

    const match = document.cookie.match(/sidebarState=(collapsed|expanded)/);
    const state = "expanded";

    wrapper.classList.add(`sidebar-${state}`);
    toggleIcon.classList.toggle("bi-chevron-left", state === "expanded");
    toggleIcon.classList.toggle("bi-chevron-right", state === "collapsed");
})();

document.addEventListener("DOMContentLoaded", () => {
    const wrapper = document.getElementById("sidebarWrapper");
    const toggleBtn = document.getElementById("sidebarToggle");
    const toggleIcon = document.getElementById("toggleIcon");

    if (!wrapper || !toggleBtn || !toggleIcon) return;

    const getCookie = name => {
        const match = document.cookie.match(new RegExp(`(^| )${name}=([^;]+)`));
        return match ? match[2] : null;
    };

    const setCookie = (name, value, days = 365) => {
        const d = new Date();
        d.setTime(d.getTime() + days * 86400000);
        document.cookie = `${name}=${value}; expires=${d.toUTCString()}; path=/; SameSite=Lax`;
    };

    toggleBtn.addEventListener("click", () => {
        const isCollapsed = wrapper.classList.contains("sidebar-collapsed");

        wrapper.classList.toggle("sidebar-collapsed", !isCollapsed);
        wrapper.classList.toggle("sidebar-expanded", isCollapsed);

        toggleIcon.classList.toggle("bi-chevron-left", isCollapsed);
        toggleIcon.classList.toggle("bi-chevron-right", !isCollapsed);

        setCookie("sidebarState", isCollapsed ? "collapsed" : "expanded");
    });
});
