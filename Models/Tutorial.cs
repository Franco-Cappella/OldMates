using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Tutorial
    {
        [JsonProperty]
        public int ID { get; set; }

         [JsonProperty]
        public string Titulo { get; set; }

         [JsonProperty]
        public string Duracion { get; set; }

         [JsonProperty]
        public string Descripcion { get; set; }
    }
}