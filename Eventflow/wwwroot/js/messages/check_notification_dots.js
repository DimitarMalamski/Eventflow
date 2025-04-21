export async function checkNotificationDots() {
    try {
        const res = await fetch('/Messages/HasNewMessages');
        const data = await res.json();

        const messageDot = document.querySelector('#messages-notification-dot');

        if (messageDot) {
            messageDot.classList.toggle('d-none', !(data.hasNotifications));
        }

        const invitesDot = document.querySelector('#invites-notification-dot');
        if (invitesDot) {
            invitesDot.classList.toggle('d-none', !data.hasPendingInvites);
        }

        const remindersDot = document.querySelector('#reminders-notification-dot');
        if (remindersDot) {
            remindersDot.classList.toggle('d-none', !data.hasUnreadReminders);
        }
    } catch (error) {
        console.error('Failed to load notification dot state:', error)
    }
}