import { initMessagesUI } from "./messages/messages_ui_binder.js";
import { initRemindersUI } from "./reminder/reminder_ui_binder.js"
import { initEventModalUI } from "./event_modal.js";
import { initInviteModal } from "./invite_modal.js"

document.addEventListener("DOMContentLoaded", () => {
    initMessagesUI();
    initRemindersUI();
    initEventModalUI();
    initInviteModal();
});

document.addEventListener("calendar:updated", () => {
    initEventModalUI();
    initInviteModal();
});