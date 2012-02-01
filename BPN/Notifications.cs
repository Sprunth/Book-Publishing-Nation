using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen.Controls;

namespace BPN
{
    using P = Program;
    class Notifications
    {
        private static ListBox notificationbox;
        public static ListBox NotificationBox { get { return notificationbox; } }

        public static void Initialize()
        {
            notificationbox = new ListBox(P._Canvas);
            notificationbox.SetBounds(4, 2 * P.ScreenSize.Y / 3 + 16, P.ScreenSize.X - 8, P.ScreenSize.Y - (2 * P.ScreenSize.Y / 3 + 16) - 24);
            notificationbox.EnableScroll(false, true);

            AddNotification("           -----Notifications Show Up Here-----");
        }

        public static void AddNotification(String notification)
        {
            notificationbox.AddItem(notification);
            notificationbox.ScrollToBottom();
        }
    }
}
