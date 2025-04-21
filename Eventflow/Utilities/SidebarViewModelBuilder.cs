using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Utilities
{
    public static class SidebarViewModelBuilder
    {
        public static List<SidebarButtonViewModel> Build(string context,
            bool isLoggedIn
            )
        {
            var buttons = new List<SidebarButtonViewModel>();

            switch (context)
            {
                case "Calendar":
                    if (isLoggedIn)
                    {
                        buttons.Add(new SidebarButtonViewModel
                        {
                            Label = "My events",
                            Url = "/Event/MyEvents",
                            CssClass = "btn btn-primary w-100"
                        });
                    }
                break;

                case "MyEvents":
                    buttons.Add(new SidebarButtonViewModel
                    {
                        Label = "Create Event",
                        Url = "/Event/Create",
                        CssClass = "btn btn-primary w-100"
                    });

                    buttons.Add(new SidebarButtonViewModel
                    {
                        Label = "My messages",
                        Url = "/Messages/Index",
                        CssClass = "btn btn-primary w-100 position-relative",
                        NotificationDotId = "messages-notification-dot"
                    });

                    buttons.Add(new SidebarButtonViewModel
                    {
                        Label = "Back to Calendar",
                        Url = "/Calendar/Index",
                        CssClass = "btn btn-primary w-100"
                    });
                break;

                case "Messages":
                    buttons.Add(new SidebarButtonViewModel 
                    { 
                        Label = "Invites",
                        Url = "/Invite/Index",
                        CssClass = "btn btn-primary w-100",
                        NotificationDotId = "invites-notification-dot"
                    });
                    buttons.Add(new SidebarButtonViewModel
                    {
                        Label = "Reminders", 
                        Url = "/Reminder/Index",
                        CssClass = "btn btn-primary w-100",
                        NotificationDotId = "reminders-notification-dot"
                    });
                    buttons.Add(new SidebarButtonViewModel
                    { 
                        Label = "Back To My Events", 
                        Url = "/Event/MyEvents",
                        CssClass = "btn btn-primary w-100"
                    });
                break;

                case "Invite":
                case "Reminder":
                    buttons.Add(new SidebarButtonViewModel 
                    { 
                        Label = "Back to Messages",
                        Url = "/Messages/Index",
                        CssClass = "btn btn-primary w-100"
                    });
                break;

                case "Create":
                case "Edit":
                    buttons.Add(new SidebarButtonViewModel
                    { 
                        Label = "Back To My Events", 
                        Url = "/Event/MyEvents",
                        CssClass = "btn btn-primary w-100"
                    });
                break;
            }

            return buttons;
        }
    }
}
