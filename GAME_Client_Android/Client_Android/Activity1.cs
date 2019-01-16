using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Client_Android
{
    [Activity(Label = "Client_Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.Landscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (CheckSelfPermission( Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                RequestPermissions( new string[] { Manifest.Permission.WriteExternalStorage }, 0);
            }

            if (CheckSelfPermission( Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
               RequestPermissions( new string[] { Manifest.Permission.ReadExternalStorage }, 0);
            }

            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.AccessCoarseLocation }, 0);
            }
            if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation }, 0);
            }
            var g = new Game1();
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

