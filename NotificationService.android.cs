using System;
using System.IO;
using System.Xml.Serialization;

using Android.App;
using Android.Content;
using Android.Provider;
using Android.Support.V4.App;

using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

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

		public void ShowPresetNotification(Notification.Builder builder)
		{

			using (Intent resultIntent = new Intent(AppContext, typeof(Activity)))
			{
				PendingIntent resultPendingIntent;
				var notificationManager = GetNotificationManager();
				resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
				using (TaskStackBuilder stackBuilder = TaskStackBuilder.Create(AppContext))
				{
					stackBuilder.AddNextIntent(resultIntent);
					resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
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

	[BroadcastReceiver(Enabled = true, Label = "NotificationService Broadcast Receiver")]
	public class ScheduledAlarmHandler : BroadcastReceiver
	{
		private static Context AppContext = Application.Context;
		public static NotificationManager GetNotificationManager() => NotificationManager.FromContext(AppContext);
		public const string AlarmNotificationKey = "AlarmNotification";
		public override void OnReceive(Context context, Intent intent)
		{
			var extra = intent.GetStringExtra(AlarmNotificationKey);
			LocalNotification notification = DeserializeNotification(extra);
			var builder = notification.GenerateNotification();
			using (Intent resultIntent = new Intent(AppContext, typeof(Activity)))
			{
				PendingIntent resultPendingIntent;
				var notificationManager = GetNotificationManager();
				resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
				using (TaskStackBuilder stackBuilder = TaskStackBuilder.Create(AppContext))
				{
					stackBuilder.AddNextIntent(resultIntent);
					resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
				}
				builder.SetContentIntent(resultPendingIntent);
				notificationManager.Notify(1000, builder.Build());
			}
		}

		private LocalNotification DeserializeNotification(string notificationString)
		{
			var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
			using (var stringReader = new StringReader(notificationString))
			{
				return (LocalNotification)xmlSerializer.Deserialize(stringReader);
			}
		}
	}

	public class LocalNotification
	{
		private static Context AppContext = Application.Context;

		public string ContentTitle { get; set; }
		public string ContentText { get; set; }
		public int SmallIcon { get; set; }
		public int Priority { get; set; }
		public string SubText { get; set; }
		public long[] VibratePattern { get; set; }

		public LocalNotification()
		{
		}

		public LocalNotification(string contentTitle, string contentText, int smallIcon, int priority, string subText, long[] vibratePattern)
		{
			ContentTitle = contentTitle;
			ContentText = contentText;
			SmallIcon = smallIcon;
			Priority = priority;
			SubText = subText;
			VibratePattern = vibratePattern;
		}

		public NotificationCompat.Builder GenerateNotification()
		{
			return new NotificationCompat.Builder(AppContext, "1000")
				.SetContentTitle(ContentTitle)
				.SetContentText(ContentText)
				.SetSmallIcon(SmallIcon)
				.SetPriority(Priority)
				.SetSubText(SubText)
				.SetSound(Settings.System.DefaultNotificationUri)
				.SetVibrate(VibratePattern);
		}
	}
}