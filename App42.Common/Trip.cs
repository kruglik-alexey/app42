using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace App42.Common
{
    public class Trip
    {
        public int Id { get; set; }

        [Required] public DateTime StartDate { get; set; }        
        [Required] public bool InProgress { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public abstract class Event
    {
        [JsonProperty(Order = -98)]
        public int Id { get; set; }

        [JsonProperty(Order = -99)]
        [Required] public DateTime Date { get; set; }

        [JsonIgnore]
        [Required] public Trip Trip { get; set; }

        [NotMapped]
        [JsonProperty(Order = -100)]
        public abstract string Type { get; }
    }

    public class LocationEvent : Event
    {
        public static readonly int Descriminator = 0;
        public static readonly string TypeStatic = "l";

        [Required] public double Lat { get; set; }
        [Required] public double Lon { get; set; }
        [Required] public float Accuracy { get; set; }
        [Required] public float Speed { get; set; }

        public override string Type => TypeStatic;
    }

    public class BatteryEvent : Event
    {
        public static readonly int Descriminator = 1;
        public static readonly string TypeStatic = "b";

        [Required] public int Charge { get; set; }

        public override string Type => TypeStatic;
    }
}
