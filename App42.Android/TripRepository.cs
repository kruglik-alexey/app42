using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace App42
{
    public class Trip
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public bool InProgress { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public abstract class Event
    {
        protected Event()
        {
            Date = DateTime.UtcNow;
        }

        public DateTime Date { get; }
        public abstract string Type { get; }
    }

    public class LocationEvent : Event
    {
        public override string Type => "l";

        public double Lat { get; set; }
        public double Lon { get; set; }
        public float Accuracy { get; set; }
        public float Speed { get; set; }
    }

    class TripRepository
    {       
        public Task<Trip> CurrentTrip() => Transport.Get<Trip>("trips/current");

        public Task<Trip> StartTrip() => Transport.Post<Trip>("trips/start");

        public Task StopTrip(int id) => Transport.Post($"trips/{id}/stop");
    }

    class EventRepository
    {
        public async Task PostEvents(int tripId, IReadOnlyCollection<Event> evts) 
        {
            Logger.I($"EventRepository.PostEvents, {evts.Count}");
            await Transport.Post($"trips/{tripId}/events", evts);
        }
    }
}