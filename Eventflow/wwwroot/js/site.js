import { initMessagesUI } from "./messages/messages_ui_binder.js";
import { initRemindersUI } from "./reminder/reminder_ui_binder.js"

document.addEventListener("DOMContentLoaded", () => {
    initMessagesUI();
    initRemindersUI();
});
