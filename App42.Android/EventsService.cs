using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Widget;

namespace App42
{
    [Service(Name = EventsService.Name)]
    public class EventsService : Service, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        public const string Name = "jgdh.app4.eventsservice";
        public const string Reason = "reason";
        public const string ReasonStartTrip = "startTrip";
        public const string ReasonBoot = "boot";

        private GoogleApiClient _googleApi;
        private EventQueue _events;
        private int _tripId;
        private bool _running;

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Logger.I($"EventsService.OnStartCommand, running: {_running}");
            var reason = intent.GetStringExtra(Reason);
            Logger.I($"EventsService.OnStartCommand, reason: {reason}");
            
            if (!_running)
            {
                Start();
            }
                    
            return StartCommandResult.Sticky;
        }

        private void Start()
        {
            _running = true;            
            StartForeground(1, CreateNotification(""));
            ShowNotification("Initializing");
            ConnectToGoogleApi();
        }

        public override async void OnDestroy()
        {
            Logger.I("EventsService.OnDestroy");
            base.OnDestroy();
            ShowNotification("Stopping");

            // TODO Flush?
            // TODO what if flushing?
            _events.Dispose();

            await Task.WhenAll(
                LocationServices.FusedLocationApi.RemoveLocationUpdatesAsync(_googleApi, this),
                new TripRepository().StopTrip(_tripId));
            Toast.MakeText(this, $"Trip {_tripId} finished", ToastLength.Long);
            ((NotificationManager)GetSystemService(NotificationService)).CancelAll();
        }

        private void ShowNotification(string text)
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(1, CreateNotification(text));
        }

        private Notification CreateNotification(string text)
        {
            return new Notification.Builder(this)
                .SetContentTitle("App42")
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true)
                .SetContentIntent(PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0))
                .Build();
        }

        private void ConnectToGoogleApi()
        {
            _googleApi = new GoogleApiClient.Builder(this)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            _googleApi.Connect();
        }

        public async void OnConnected(Bundle connectionHint)
        {
            ShowNotification("Creating trip");
            await StartTrip();

            var locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(Consts.LocationRequestInterval);
            locationRequest.SetFastestInterval(Consts.FastestLocationRequestInterval);

            _events = new EventQueue(_tripId);
            _events.OnFlush += OnFlush;
            await LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApi, locationRequest, this);
            ShowNotification("Tripping");
        }

        private void OnFlush()
        {
            ShowNotification($"Tripping, last flush {DateTime.Now.ToShortTimeString()}");
        }

        private async Task StartTrip()
        {
            _tripId = (await new TripRepository().StartTrip()).Id;
            Toast.MakeText(this, $"Trip {_tripId} started", ToastLength.Long);
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new System.NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new System.NotImplementedException();
        }

        public void OnLocationChanged(Location location)
        {
            _events.Add(new LocationEvent
            {
                Lat = location.Latitude,
                Lon = location.Longitude,
                Accuracy = location.Accuracy,
                Speed = location.Speed
            });
        }
    }
}