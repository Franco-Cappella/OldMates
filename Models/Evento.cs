using System.Collections.Generic;
using System.Linq;
using System.Web;
using TP10.Models;
using Newtonsoft.Json;

namespace TP10.Models
{
    public class Evento
    {
        [JsonProperty]
        public int ID { get; set; }

         [JsonProperty]
        public DateTime Fecha { get; set; }

         [JsonProperty]
        public string Localidad { get; set; }

         [JsonProperty]
        public int Capacidad { get; set; }

         [JsonProperty]
        public string Intereses { get; set; }

    }

}