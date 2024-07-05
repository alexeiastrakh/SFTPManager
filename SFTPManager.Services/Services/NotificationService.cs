namespace SFTPManager.Services
{
    using System;
    using Notification.Wpf;

    public class NotificationService
    {
        private readonly NotificationManager notificationManager;

        public NotificationService()
        {
            notificationManager = new NotificationManager();
        }

        public void ShowNotification(string title, string message, NotificationType type, TimeSpan? duration = null)
        {
            duration ??= TimeSpan.FromSeconds(5);
            notificationManager.Show(title, message, type, "", duration.Value);
        }
    }
}
