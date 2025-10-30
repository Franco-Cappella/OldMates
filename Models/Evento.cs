using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Evento
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]

        public int IDCreador { get; set; }

        [JsonProperty]

        public List<int> Anotados { get; set; }

        [JsonProperty]
        public DateTime Fecha { get; set; }

        [JsonProperty]
        public string Titulo { get; set; }

        [JsonProperty]
        public string Descripcion { get; set; }

        [JsonProperty]
        public string Localidad { get; set; }

        [JsonProperty]
        public int Capacidad { get; set; }

        [JsonProperty]
        public string Intereses { get; set; }

    }
}