using System;
using System.Collections.Generic;
using System.Linq;
using App42.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App42.Server.Controllers
{
    [Route("trips")]
    public class TripController : Controller
    {
        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            using (var db = new App42Context())
            {
                db.Trips.RemoveRange(db.Trips);
                db.SaveChanges();
                return Ok();
            }
        }

        [HttpPost]
        [Route("start")]
        public IActionResult StartTrip()
        {
            using (var db = new App42Context())
            {
                var trip = new Trip {InProgress = true, StartDate = DateTime.UtcNow};
                db.Trips.Add(trip);
                db.SaveChanges();
                return Json(trip);
            }            
        }

        [HttpPost]
        [Route("{id}/stop")]
        public IActionResult StopTrip(int id)
        {
            using (var db = new App42Context())
            {
                var trip = db.Trips.Find(id);

                if (trip == null)
                {
                    return BadRequest($"No trip with id {id}");
                }

                if (!trip.InProgress)
                {
                    return BadRequest($"Trip {id} already stopped");
                }

                trip.InProgress = false;
                trip.EndDate = DateTime.UtcNow;                
                db.SaveChanges();

                return Json(trip);
            }
        }

        [HttpGet]
        [Route("current")]        
        public IActionResult CurrentTrip()
        {
            using (var db = new App42Context())
            {
                var trip = db.Trips.OrderByDescending(t => t.StartDate).FirstOrDefault(t => t.InProgress);
                return Json(trip);
            }
        }        

        [HttpPost]
        [Route("{tripId}/events")]
        public IActionResult AddEvents(int tripId, [FromBody] IList<dynamic> data)
        {
            using (var db = new App42Context())
            {
                var trip = db.Trips.Find(tripId);
                if (trip == null)
                {
                    return BadRequest($"No trip with id {tripId}");
                }

                if (!trip.InProgress)
                {
                    return BadRequest($"Trip {tripId} already stopped");
                }

                foreach (dynamic d in data)
                {
                    switch (((string)d.type).ToLowerInvariant())
                    {
                        case var t when t == LocationEvent.TypeStatic.ToLowerInvariant():
                        {
                            var evt = new LocationEvent
                            {
                                Trip = trip,
                                Date = d.date,
                                Lat = d.lat,
                                Lon = d.lon,
                                Accuracy = d.accuracy,
                                Speed = d.speed
                            };
                            db.Events.Add(evt);

                            break;
                        }

                        case var t when t == BatteryEvent.TypeStatic.ToLowerInvariant():
                        {
                            var evt = new BatteryEvent
                            {
                                Trip = trip,
                                Date = d.date,
                                Charge = d.charge
                            };
                            db.Events.Add(evt);
                            
                            break;
                        }
                        default: return BadRequest($"Unknown event type {d.type}");
                    }
                }

                db.SaveChanges();
            }

            return Ok();
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetTrips()
        {
            using (var db = new App42Context())
            {
                var trips = db.Trips.OrderByDescending(t => t.StartDate).ToArray();
                return Json(trips);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTrip(int id)
        {
            using (var db = new App42Context())
            {
                var trip = db.Trips.Find(id);
                if (trip == null)
                {
                    return BadRequest($"No trip with id {id}");
                }

                return Json(trip);
            }
        }

        [HttpGet]
        [Route("{tripId}/events/{afterEventId?}")]
        public IActionResult GetTripEvent(int tripId, int? afterEventId)
        {
            using (var db = new App42Context())
            {
                var q = db.Events.Where(e => e.Trip.Id == tripId);
                if (afterEventId != null)
                {
                    q = q.Where(e => e.Id > afterEventId);
                }
                var evts = q.OrderByDescending(e => e.Date).ToArray();

                return Json(evts);
            }
        }       
    }    
}
