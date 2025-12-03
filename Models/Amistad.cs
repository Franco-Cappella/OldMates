using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Amistad
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public int IDUsuario1 { get; set; }

        [JsonProperty]
        public int IDUsuario2 { get; set; }

        [JsonProperty]
        public string Estado { get; set; }

        [JsonProperty]
        public DateTime FechaSolicitud { get; set; }

        [JsonProperty]
        public DateTime? FechaRespuesta { get; set; }
        public Usuario Usuario1 { get; set; }
        public Usuario Usuario2 { get; set; }

        public Amistad() { }
    }
}
