using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using App42.Common;

namespace App42
{    
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