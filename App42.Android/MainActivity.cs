using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;

namespace App42
{
    [Activity(Label = "App42", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _startButton;
        private Button _stopButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _startButton = FindViewById<Button>(Resource.Id.startButton);
            _startButton.Click += OnStartClick;

            _stopButton = FindViewById<Button>(Resource.Id.stopButton);
            _stopButton.Click += OnStopClick;            
        }

        private bool IsServiceRunning()
        {
            var manager = (ActivityManager)GetSystemService(ActivityService);
            return manager.GetRunningServices(int.MaxValue).Select(s => s.Service.ClassName).Contains(EventsService.Name);
        }        

        private void OnStartClick(object sender, EventArgs eventArgs)
        {
            if (IsServiceRunning())
            {
                Toast.MakeText(this, "Already running", ToastLength.Short).Show();
                return;
            }
            if (!RequestPermissions())
            {
                return;
            }
            var intent = new Intent(this, typeof(EventsService));
            intent.PutExtra(EventsService.Reason, EventsService.ReasonStartTrip);
            StartService(intent);            
        }

        private void OnStopClick(object sender, EventArgs eventArgs)
        {
            if (!IsServiceRunning())
            {
                Toast.MakeText(this, "Not running", ToastLength.Short).Show();
                return;
            }
            StopService(new Intent(this, typeof(EventsService)));            
        }

        private bool RequestPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveBootCompleted) == Permission.Granted)
            {
                return true;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                // TODO
            }
            ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.AccessFineLocation, Manifest.Permission.ReceiveBootCompleted}, 0);
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            StartService(new Intent(this, typeof(EventsService)));
        }
    }
}

