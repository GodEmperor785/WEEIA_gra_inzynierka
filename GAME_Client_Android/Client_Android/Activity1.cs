using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Client_PC.UI;

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
        View pView = null;
        private Game1 g;
        private int counter = 0;
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

            g = new Game1(this);
            pView = (View)g.Services.GetService(typeof(View));
            pView.KeyPress += OnKeyPress;
            
            SetContentView(pView);
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }

        public void ShowKeyboard()
        {
            

            var pView = g.Services.GetService<View>();
            var inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public void HideKeyboard()
        {
            InputMethodManager inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
        }
        private void OnKeyPress(object sender, View.KeyEventArgs e)
        {
            if (counter == 1)
            {
                counter--;
            }
            else if (g.FocusedElement != null)
            {
                var z = g.FocusedElement;
                if (z is InputBox)
                {
                    var inputBox = (InputBox) z;
                    if (inputBox.Text.Length < inputBox.TextLimit)
                    {
                        if (e.KeyCode == Keycode.Enter)
                        {
                            HideKeyboard();
                        }
                        else if (e.KeyCode == Keycode.Space)
                        {
                            inputBox.Text += " ";
                        }
                        else if (e.KeyCode == Keycode.Del)
                        {
                            if (inputBox.Text.Length > 0)
                                inputBox.Text = inputBox.Text.Substring(0, inputBox.Text.Length - 1);
                        }
                        else
                        {
                            var inp =  (char)e.Event.GetUnicodeChar(MetaKeyStates.NumLockOn);
                            var inp2 = ((char)e.KeyCode).ToString();
                            inputBox.Text += inp.ToString();
                        }

                    }
                    else if (e.KeyCode == Keycode.Back)
                    {
                        if (inputBox.Text.Length > 0)
                            inputBox.Text = inputBox.Text.Substring(0, inputBox.Text.Length - 1);
                    }
                }

                counter++;
            }
        }
    }
}

