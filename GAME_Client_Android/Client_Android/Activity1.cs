using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Serialization;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Client_PC.UI;
using Client_PC.Utilities;
using Java.IO;
using Java.Lang;
using Java.Nio;
using Microsoft.Xna.Framework.Graphics;
using File = Java.IO.File;

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
        private Keycode lastKeycode;
        public string Path;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != (int) Permission.Granted)
            {
                RequestPermissions(new string[] {Manifest.Permission.WriteExternalStorage}, 0);
            }

            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != (int) Permission.Granted)
            {
                RequestPermissions(new string[] {Manifest.Permission.ReadExternalStorage}, 0);
            }

            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int) Permission.Granted)
            {
                RequestPermissions(new string[] {Manifest.Permission.AccessCoarseLocation}, 0);
            }

            if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != (int) Permission.Granted)
            {
                RequestPermissions(new string[] {Manifest.Permission.AccessFineLocation}, 0);
            }

            g = new Game1(this);
            pView = (View) g.Services.GetService(typeof(View));
            pView.KeyPress += OnKeyPress;

            SetContentView(pView);
            SetContentView((View) g.Services.GetService(typeof(View)));
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
            InputMethodManager inputMethodManager =
                Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
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
                        else if (e.KeyCode == Keycode.ShiftLeft || e.KeyCode == Keycode.ShiftRight ||
                                 e.KeyCode == Keycode.AltLeft || e.KeyCode == Keycode.AltRight)
                        {

                        }
                        else
                        {
                            char input;
                            if (lastKeycode == Keycode.ShiftLeft || lastKeycode == Keycode.ShiftRight)
                            {
                                input = (char) e.Event.GetUnicodeChar(MetaKeyStates.ShiftLeftOn);
                            }
                            else if (lastKeycode == Keycode.AltLeft || lastKeycode == Keycode.AltRight)
                            {
                                input = (char) e.Event.GetUnicodeChar(MetaKeyStates.AltLeftOn);
                            }
                            else
                            {
                                input = (char) e.Event.GetUnicodeChar(MetaKeyStates.NumLockOn);
                            }

                            var inp2 = ((char) e.KeyCode).ToString();
                            inputBox.Text += input.ToString();
                        }

                    }
                    else if (e.KeyCode == Keycode.Del)
                    {
                        if (inputBox.Text.Length > 0)
                            inputBox.Text = inputBox.Text.Substring(0, inputBox.Text.Length - 1);
                    }

                    lastKeycode = e.KeyCode;
                }

                counter++;
            }
        }

        public void getFile(string shipName)
        {
            Intent intent = new Intent(Intent.ActionOpenDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("image/*");
            Path = shipName;
            StartActivityForResult(intent, 42);
        }
        
        protected override void OnActivityResult(int requestCode, Result resultCode,
            Intent resultData)
        {
            if (resultCode == Result.Ok)
            {
                // The document selected by the user won't be returned in the intent.
                // Instead, a URI to that document will be contained in the return intent
                // provided to this method as a parameter.
                // Pull that URI using resultData.getData().
                Uri uri = null;
                if (resultData != null)
                {
                    uri = resultData.Data;

                    ParcelFileDescriptor parcelFileDescriptor =
                        ContentResolver.OpenFileDescriptor(uri, "r");
                    FileDescriptor fileDescriptor = parcelFileDescriptor.FileDescriptor;
                    Bitmap image = BitmapFactory.DecodeFileDescriptor(fileDescriptor);
                    parcelFileDescriptor.Close();
                    
                     var store = IsolatedStorageFile.GetUserStoreForApplication();
                    if (store.FileExists(Path))
                    {
                        store.DeleteFile(Path);
                    }

                    var fs = store.CreateFile(Path);
                    MemoryStream buffer = new MemoryStream();
                    image.Compress(Bitmap.CompressFormat.Png,100,buffer);
                    buffer.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = new byte[(int)buffer.Length];
                    buffer.Read(bytes, 0, (int)buffer.Length);
                    
                    fs.Write(bytes, 0,bytes.Length);
                    fs.Close();
                    Texture2D text = null;
                    var file2 = store.OpenFile(Path, FileMode.Open);

                    text = Texture2D.FromStream(Game1.self.GraphicsDevice, file2);
                    file2.Close();
                    file2.Dispose();
                    
                    Game1.self.config.listOfCards.Single(p => p.Name == Path).SkinPath = Path;
                    Game1.self.ShipsSkins.Single(p => p.ship == Path).skin = Game1.self.loadTexture2D(Path);
                    var file = store.OpenFile("Config_Cards", FileMode.Open);
                    TextWriter writer = new StreamWriter(file);
                    XmlSerializer xml = new XmlSerializer(typeof(Cards));
                    xml.Serialize(writer, Game1.self.config);
                    writer.Close();
                    file.Close();
                    store.Close();
                }
            }
        }
    }
}

