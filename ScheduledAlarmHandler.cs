using System.IO;
using System.Xml.Serialization;

using Android.Content;
#if MONOANDROID90 || MONOANDROID
namespace Plugin.NotificationService
{
	[BroadcastReceiver(Enabled = true, Label = "NotificationService Broadcast Receiver")]
	public class ScheduledAlarmHandler : BroadcastReceiver
	{
		public const string AlarmNotificationKey = "AlarmNotification";
		public override void OnReceive(Context context, Intent intent)
		{
			var extra = intent.GetStringExtra(AlarmNotificationKey);
			LocalNotification notification = DeserializeNotification(extra);
			NotificationServiceImplementation.ShowPresetNotification(notification.GenerateNotification());
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
}
#endif