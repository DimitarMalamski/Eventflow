namespace Eventflow.Utilities
{
    public static class SidebarHelper
    {
        public static string GetSidebarContext(string controller, string action)
        {
            return (controller, action) switch
            {
                ("Event", "Create") => "Create",
                ("Event", "Edit") => "Edit",
                ("Event", "MyEvents") => "MyEvents",
                ("Invite", "Index") => "Invite",
                ("Reminder", "Index") => "Reminder",
                ("Messages", "Index") => "Messages",
                ("Admin", "Index") => "Admin",
                _ => "Calendar"
            };
        }
        public static bool NeedsVerticalCentering(string controller)
        {
            var noVerticalCenterPage = new[] { "Invite", "Reminder", "Messages" };
            return !noVerticalCenterPage.Contains(controller);
        }
    }
}
