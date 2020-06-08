using System;
using System.Collections.Generic;
using System.Text;

using AudioToolbox;

using UserNotifications;

namespace Plugin.NotificationService
{
	/// <summary>
	/// Interface for NotificationService
	/// </summary>
	public class NotificationServiceImplementation : INotificationService
	{
		public void CancelNotification(int id)
		{
			UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
			UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
			UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new string[] { id.ToString() });
		}

		private static UNMutableNotificationContent CreateNotificationContent(string title, string subtitle, string body)
		{
			return new UNMutableNotificationContent
			{
				Title = title,
				Subtitle = subtitle,
				Body = body,
				Badge = 1,
				Sound = UNNotificationSound.DefaultCriticalSound,
			};
		}


		public void ShowNotification(string title, string subtitle, string body, int id, DateTimeOffset notificationTime)
		{
			ShowNotification(title, subtitle, body, id, notificationTime.Subtract(DateTimeOffset.Now));
		}

		public void ShowNotification(string title, string subtitle, string body, int id, TimeSpan waitInterval)
		{
			try
			{
				UNMutableNotificationContent content = CreateNotificationContent(title, subtitle, body);
				var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(waitInterval.TotalSeconds, false);
				var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

				UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
				{
					if (err == null)
					{
						SystemSound.Vibrate.PlayAlertSound();
					}
					else
					{
						Console.WriteLine(err.ToString());
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

	}

	//public class NotificationsConfiguration : UNUserNotificationCenterDelegate
	//{
	//	public NotificationsConfiguration()
	//	{
	//	}

	//	#region Override Methods
	//	public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
	//	{
	//		// Do something with the notification
	//		Console.WriteLine("Active Notification: {0}", notification);

	//		// Tell system to display the notification anyway or use
	//		// `None` to say we have handled the display locally.
	//		completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge);
	//	}
	//	#endregion
	//}
}
