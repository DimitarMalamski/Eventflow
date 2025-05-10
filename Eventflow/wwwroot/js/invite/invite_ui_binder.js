import { initInviteFilter } from "./invite_filter.js";

export function initInvitesUI() {
   initInviteFilter();
}

document.addEventListener("DOMContentLoaded", initInvitesUI);