export function showDailyEmptyMessage() {
    const dailyMessage = () => {
        const container = document.getElementById("dailyReminderMessage");
        if (!container) {
            return;
        }

        const messages = [
            "Your reminder list is feeling light today!",
            "Looks like you’re all caught up!",
            "No reminders today — enjoy the peace.",
            "All clear for now, you rock!",
            "Just breeze through your day, nothing pending!",
            "Nothing here... maybe time for a break?",
            "Ebi si maikata!"
        ];

        const today = new Date().getDate();
        const index = today % messages.length;

        container.textContent = messages[index];
    };

    setTimeout(dailyMessage, 100);
}