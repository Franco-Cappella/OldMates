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
        public DateTime Fecha { get; set; }

        [JsonProperty]
        public string Titulo { get; set; }

        [JsonProperty]
        public string Descripcion { get; set; }

         [JsonProperty]
        public TimeSpan Duracion { get; set; }

        [JsonProperty]
        public string Localidad { get; set; }

        [JsonProperty]
        public int Capacidad { get; set; }

        [JsonProperty]
        public string Intereses { get; set; }

        [JsonProperty]
        public string Foto { get; set; }

        [JsonProperty]
        public bool DesInscribirse { get; set; }

        [JsonProperty]
        public bool Eliminada { get; set; }
        
        // Constructor sin parámetros requerido por Dapper
        public Evento()
        {
        }
        
        public Evento (int IDCreador, string titulo, string descripcion, TimeSpan duracion, string localidad, int capacidad, DateTime fecha, string intereses, string foto)
        {
            this.IDCreador = IDCreador;
            this.Titulo = titulo;
            this.Descripcion = descripcion;
            this.Duracion = duracion;
            this.Localidad = localidad;
            this.Capacidad = capacidad;
            this.Intereses = intereses;
            this.Fecha = fecha;
            this.Foto = foto;
            this.DesInscribirse = true;
            this.Eliminada = false;
        }
    }
}