using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Interes
    {
        [JsonProperty]
        public int ID { get; set; }

         [JsonProperty]
        public string Tipo { get; set; }
    }
}