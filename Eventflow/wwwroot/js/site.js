import { initMessagesUI } from "./messages/messages_ui_binder.js";
import { initRemindersUI } from "./reminder/reminder_ui_binder.js";
import { initInvitesUI } from "./invite/invite_ui_binder.js";
import { initEventModalUI } from "./event_modal.js";
import { initInviteModal } from "./invite_modal.js";

document.addEventListener("DOMContentLoaded", () => {
    initEventModalUI();
    initInviteModal();
    initMessagesUI();
    initRemindersUI();
    initInvitesUI();
});

document.addEventListener("calendar:updated", () => {
    initEventModalUI();
    initInviteModal();
});