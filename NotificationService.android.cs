using System;
using System.IO;
using System.Xml.Serialization;

using Android.App;
using Android.Content;

namespace Plugin.NotificationService
{
	/// <summary>
	/// Interface for NotificationService
	/// </summary>
	public class NotificationServiceImplementation : INotificationService
	{
		private static Context AppContext = Application.Context;

		private long NotifyTimeInMilliseconds(DateTimeOffset notifyTime) => notifyTime.ToUnixTimeMilliseconds();

		private Intent CreateScheduledAlarmHandlerIntent() => new Intent(AppContext, typeof(ScheduledAlarmHandler));

		private AlarmManager GetAlarmManager() => AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

		public static NotificationManager GetNotificationManager() => NotificationManager.FromContext(AppContext);

		public long[] VibratePattern { get; set; } = { 0, 200, 200, 200, 200 };

		public void ShowNotification(string title, string subtitle, string body, int id, TimeSpan waitInterval)
		{
			var alarmManager = GetAlarmManager();
			var initialIntent = CreateScheduledAlarmHandlerIntent();
			var builder = new LocalNotification(title, body, Application.Context.Resources.GetIdentifier("icon", "mipmap", Application.Context.PackageName), (int)NotificationImportance.Max, subtitle, VibratePattern);
			initialIntent.PutExtra(ScheduledAlarmHandler.AlarmNotificationKey, SerializeNotification(builder));
			var pendingIntent = PendingIntent.GetBroadcast(context: AppContext,
														   requestCode: 0,
														   intent: initialIntent,
														   flags: PendingIntentFlags.CancelCurrent);
			var triggerTime = NotifyTimeInMilliseconds(DateTimeOffset.Now.Add(waitInterval));
			alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
		}

		public void ShowNotification(string title, string subtitle, string body, int id, DateTimeOffset notificationTime)
		{
			ShowNotification(title, subtitle, body, id, notificationTime.Subtract(DateTimeOffset.Now));
		}



		public static void ShowPresetNotification(Notification.Builder builder)
		{

			using (Intent resultIntent = new Intent(AppContext, typeof(Activity)))
			{
				PendingIntent resultPendingIntent;
				var notificationManager = GetNotificationManager();
				resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
				using (TaskStackBuilder stackBuilder = TaskStackBuilder.Create(AppContext))
				{
					stackBuilder.AddNextIntent(resultIntent);
					resultPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
				}
				builder.SetContentIntent(resultPendingIntent);
				notificationManager.Notify(1000, builder.Build());
			}
		}

		public void CancelNotification(int id)
		{
			var pendingIntent = PendingIntent.GetBroadcast(AppContext, 0, CreateScheduledAlarmHandlerIntent(), PendingIntentFlags.CancelCurrent);
			var alarmManager = GetAlarmManager();
			var notificationManager = GetNotificationManager();
			alarmManager.Cancel(pendingIntent);
			notificationManager.Cancel(id);
		}

		private string SerializeNotification(LocalNotification builder)
		{
			var xmlSerializer = new XmlSerializer(builder.GetType());
			using (var stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, builder);
				return stringWriter.ToString();
			}
		}

	}


}