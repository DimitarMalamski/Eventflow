import { initMarkAsRead } from "./mark_reminder_read.js";
import { initLikeToggle } from "./toggle_like_reminder.js";
import { initReminderFilter } from "./reminder_filter.js";
import { showDailyEmptyMessage } from "./empty_reminder_message.js";
import { openSetReminderModal } from "./set_reminder_modal.js";

export function initRemindersUI() {
    initMarkAsRead();
    initLikeToggle();
    initReminderFilter();
    showDailyEmptyMessage();
}

window.openSetReminderModal = openSetReminderModal;

document.addEventListener("DOMContentLoaded", initRemindersUI);

document.addEventListener("reminders:updated", () => {
    initMarkAsRead();
    initLikeToggle();
    showDailyEmptyMessage();
});