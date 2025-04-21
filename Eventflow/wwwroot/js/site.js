import { initMessagesUI } from "./messages/messages_ui_binder.js";
import { initRemindersUI } from "./reminder/reminder_ui_binder.js"
import { initEventModalUI } from "./event_modal.js";

document.addEventListener("DOMContentLoaded", () => {
    initMessagesUI();
    initRemindersUI();
    initEventModalUI();
});

document.addEventListener("calendar:updated", () => {
    console.log("🎯 calendar:updated triggered — rebinding UI"); // ✅ Add this!
    initEventModalUI();
});