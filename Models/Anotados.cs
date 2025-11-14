using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Anotados
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public int IDEvento { get; set; }

        [JsonProperty]
        public int IDUsuario { get; set; }
    }
}