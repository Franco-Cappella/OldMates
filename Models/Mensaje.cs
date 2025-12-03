using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Mensaje
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public int IDEmisor { get; set; }

        [JsonProperty]
        public int IDReceptor { get; set; }

        [JsonProperty]
        public string Contenido { get; set; }

        [JsonProperty]
        public DateTime FechaEnvio { get; set; }

        [JsonProperty]
        public bool Leido { get; set; }

        // Propiedades de navegación
        public Usuario Emisor { get; set; }
        public Usuario Receptor { get; set; }

        public Mensaje() { }
    }
}
