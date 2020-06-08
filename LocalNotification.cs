
using Android.App;
using Android.Content;
using Android.Provider;

#if MONOANDROID90 || MONOANDROID
namespace Plugin.NotificationService
{
	public class LocalNotification
	{
		private static Context AppContext = global::Android.App.Application.Context;

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

#pragma warning disable CS0618 // Type or member is obsolete
		public Notification.Builder GenerateNotification()
		{
			var Generator = new Notification.Builder(AppContext, "1000");
			Generator.SetContentTitle(ContentTitle)
				.SetContentText(ContentText)
				.SetPriority(Priority)
				.SetSubText(SubText)
				.SetSmallIcon(SmallIcon)
				.SetSound(Settings.System.DefaultNotificationUri)
				.SetVibrate(VibratePattern);
			return Generator;
		}
#pragma warning restore CS0618 // Type or member is obsolete
	}


}
#endif