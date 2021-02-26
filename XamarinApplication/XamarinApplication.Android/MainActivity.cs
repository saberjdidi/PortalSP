using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.DownloadManager;
using System.IO;
using Plugin.DownloadManager.Abstractions;
using Plugin.CurrentActivity;
using Plugin.Permissions;

namespace XamarinApplication.Droid
{
    [Activity(Label = "PortalSP", Icon = "@drawable/logo", Theme = "@style/MainTheme.Base", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState); //add this
            base.OnCreate(savedInstanceState);
            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState); // < ---- Add for radio button
            Downloaded(); //use for download file from url

            CrossCurrentActivity.Current.Init(this, savedInstanceState); //use for image
            Plugin.MaterialDesignControls.Android.Renderer.Init(); //Material Design
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(
                 requestCode,
                 permissions,
                 grantResults);
        }
        //method for download file from Url
        public void Downloaded()
        {
            CrossDownloadManager.Current.PathNameForDownloadedFile = new System.Func<IDownloadFile, string>(file =>
            {
                /*Intent intent = new Intent(DownloadManager.ActionViewDownloads);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);*/ //open your file downloaded
                // string fileName = Android.Net.Uri.Parse(file.Url).Path.Split('/').Last();
                string fileName = "test-portalsp.pdf";
                return Path.Combine(ApplicationContext.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName);
            });
        }
        //close popup with buck button
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}