﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.NotificationService
{
	public interface INotificationService
	{
		void CancelNotification(int id);
		void ShowNotification(string title, string subtitle, string body, int id, TimeSpan waitInterval);
		void ShowNotification(string title, string subtitle, string body, int id, DateTimeOffset notificationTime);
	}
}
