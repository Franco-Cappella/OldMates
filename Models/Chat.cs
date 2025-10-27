using Newtonsoft.Json;

namespace OldMates.Models
{
    public class Chat
    {
        [JsonProperty]
        public int ID { get; set; }

         [JsonProperty]
        public string Tipo { get; set; }
    }
}