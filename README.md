# Archived

Please use https://github.com/thudugala/Plugin.LocalNotification for Notifications in Xamarin




<br>
<br>
<br>
<br>
<br>




<!-- TABLE OF CONTENTS -->

## Table of Contents

* [Getting Started](#getting-started)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)



<!-- ABOUT THE PROJECT -->
## About The Project

NotificationService aims to be a plugin that will simplify the process of adding Notifications within your Xamarin app. 
Simply provide a time and let the plugin work out the rest under the hood.


<!-- GETTING STARTED -->
## Getting Started

### Installation


**Android Setup**

For a base setup, first we need to create our notification channel in *MainActivity.cs*
here is an example
```
public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
public static readonly int NOTIFICATION_ID = 1000; <-- Important that it is 1000 in current plugin
public static readonly string CHANNEL_ID = "YourChannel";
internal static readonly string COUNT_KEY = "count";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            CreateNotificationChannel(); <--- important to call the setup function!

....
void CreateNotificationChannel()
  {
      if (Build.VERSION.SdkInt < BuildVersionCodes.O)
      {
          // Notification channels are new in API 26 (and not a part of the
          // support library). There is no need to create a notification
          // channel on older versions of Android.
          return;
      }

      var name = Resources.GetString(Resource.String.channel_name);
      var description = GetString(Resource.String.channel_description);
      var channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Max)
      {
          Description = description,
          LockscreenVisibility = NotificationVisibility.Public,
          Importance = NotificationImportance.Max,

      };

      var notificationManager = (NotificationManager)GetSystemService(NotificationService);
      notificationManager.CreateNotificationChannel(channel);
  }
}
```

to note, in the current iteration, the plugin will use `Application.Context.Resources.GetIdentifier("icon", "mipmap", Application.Context.PackageName)`
to collect the icon used for Notifications.
**iOS Setup**

By default, if the app is open, iOS will not show a notification, you can choose to override this behaviour with the following
```
public class NotificationsConfiguration : UNUserNotificationCenterDelegate
{
    public NotificationsConfiguration()
    {
    }

    #region Override Methods
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        // Do something with the notification
        Console.WriteLine("Active Notification: {0}", notification);

        // Tell system to display the notification anyway or use
        // `None` to say we have handled the display locally.
        completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge);
    }
    #endregion
}
```

You may wish to keep it as none if you show notifications internally in the app.

then, in the iOS `FinishedLaunching` override, add the following

```
  if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
  {
      // Ask the user for permission to get notifications on iOS 10.0+
      UNUserNotificationCenter.Current.RequestAuthorization(
              UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound | UNAuthorizationOptions.CriticalAlert,
              (approved, error) => { Console.WriteLine(approved); });
  }
  else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
  {
      // Ask the user for permission to get notifications on iOS 8.0+
      var settings = UIUserNotificationSettings.GetSettingsForTypes(
              UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
              new NSSet());

      UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
  }

  UNUserNotificationCenter.Current.Delegate = new NotificationsConfiguration();
```
**Usage**


<!-- USAGE EXAMPLES -->
## Usage

*Current limitations of this plugin is that you can queue only one notification at a time, with new notifications overwriting previous ones, this will be fixed in later versions*

the API for usage is as follows
```
NotificationService.ShowNotification(title: "NotificationTitle",
                                     subtitle: "Quick Notification",
                                      body: "Alarming information",
                                      id: {currentlyUnusedSoJustPut1},
                                      notificationTime: TimeSpan.FromMinutes(30));
```
This will show the notification in 30 minutes from the current time.
```
NotificationService.ShowNotification(title: "NotificationTitle",
                                     subtitle: "Quick Notification",
                                      body: "Alarming information",
                                      id: {currentlyUnusedSoJustPut1},
                                      notificationTime: new DateTimeOffset({someTimeHere));
```
This will show the alarm at {someTimeHere}. Please ensure that this time is in the future.

```
NotificationService.CancelNotification({justleaveas1})
```
This will cancel the notification that you have queue'd.


It is setup this way as the future of this plugin involves specifying ID's and having multiple notifications lined up. 

***PLEASE BE WARNED, Android Doze means that once the app will bunch notifications together to save power. This currently does not affect this plugin as you can only queue one notification, but in the future this may become an issue***

<!-- LICENSE -->
## License

This project uses the MIT License



<!-- CONTACT -->
## Contact

My [Github](https://github.com/LuckyDucko),
or reach me on the [Xamarin Slack](https://xamarinchat.herokuapp.com/),
or on my [E-mail](tyson@logchecker.com.au)

Project Link: [NotificationService](https://github.com/LuckyDucko/NotificationService)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [Brimmick](https://github.com/brminnick) guided me in making some choice decisions concerning notifications.)
* [Edsnider](https://github.com/edsnider) for providing the initial [framework](https://github.com/edsnider/LocalNotificationsPlugin)
* [JamesMontemagno](https://github.com/jamesmontemagno) for having this [amazing visual studio plugin](https://montemagno.com/new-plugin-for-xamarin-multi-target-templates-for-visual-studio-2017/) which makes creating plugins much MUCH easier
