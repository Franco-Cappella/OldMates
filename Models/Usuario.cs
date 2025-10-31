using System.Collections.Generic;
using System.Linq;
using System.Web;
using OldMates.Models;
using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Usuario
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]
        public string Contraseña { get; set; }

        [JsonProperty]
        public string Localidad { get; set; }

        [JsonProperty]
        public string Intereses { get; set; }

        [JsonProperty]
        public string Nombre { get; set; }

        [JsonProperty]
        public string Apellido { get; set; }

        [JsonProperty]
        public bool Admin { get; set; }

        public Usuario()
        {

        }
    }
}
