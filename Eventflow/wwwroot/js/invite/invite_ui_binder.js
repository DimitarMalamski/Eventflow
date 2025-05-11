import { initInviteFilter } from "./invite_filter.js";
import { initInviteResponse  } from "./respond_invite.js";
import { showDailyEmptyMessage } from "./empty_invite_message.js"

export function initInvitesUI() {
   initInviteFilter();
   initInviteResponse();
   showDailyEmptyMessage();
}

document.addEventListener("DOMContentLoaded", initInvitesUI);

document.addEventListener("invites:updated", () => {
    initInviteResponse();
    showDailyEmptyMessage();
});