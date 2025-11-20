using Microsoft.Data.SqlClient;
using Dapper;
using OldMates.Models;
using System.Linq;
using System.Data;

namespace OldMates.Models
{
    public static class BD
    {
        private static string _connectionString = @"Server=localhost;DataBase=OldMates; Integrated Security=True; TrustServerCertificate=True;";

        public static Usuario ObtenerPorUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Username = @Username";
                return connection.QueryFirstOrDefault<Usuario>(query, new { Username = username });
            }
        }

        public static bool VerificarContraseña(string Username, string Contraseña)
        {

            Usuario x = ObtenerPorUsername(Username);
            if (x == null || x.Contraseña != Contraseña)
            {

                return false;
            }

            else return true;
        }

        public static bool Registro(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string QueryExiste = "SELECT * FROM Usuario WHERE Username = @Username";
                int existe = connection.QueryFirstOrDefault<int>(QueryExiste, new { Username = usuario.Username });
                if (existe == 0)
                {
                    string query = @"INSERT INTO Usuario (Username, Contraseña, Localidad, Intereses, Nombre, Apellido, Admin)
                               VALUES (@Username, @Contraseña, @Localidad, @Intereses, @Nombre, @Apellido, @Admin)";

                    connection.Execute(query, new { usuario.Username, usuario.Contraseña, usuario.Localidad, usuario.Intereses, usuario.Nombre, usuario.Apellido, usuario.Admin });
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static Evento ObtenerEventoPorId(int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Evento WHERE ID = @IDEvento AND Eliminada = 0";
                return connection.QueryFirstOrDefault<Evento>(query, new { IDEvento = IDEvento });
            }
        }

        public static List<Evento> ObtenerEventos()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Evento WHERE Eliminada = 0";
                return connection.Query<Evento>(query).ToList();
            }
        }

        public static List<Evento> MisActividades(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string consultaAnotaciones = @" SELECT IDEvento FROM Anotados WHERE IDUsuario = @IDUsuario AND DesInscribirse = 0";

                List<int> listaDeIDEventos = connection.Query<int>(consultaAnotaciones, new { IDUsuario }).ToList();

                if (listaDeIDEventos.Count == 0)
                    return new List<Evento>();

                List<Evento> eventos = new List<Evento>();
                foreach (var eventoID in listaDeIDEventos)
                {
                    string consultaEventos = @"SELECT * FROM Evento WHERE ID = @IDEvento AND Eliminada = 0";

                    var evento = connection.QueryFirstOrDefault<Evento>(consultaEventos, new { IDEvento = eventoID });
                    if (evento != null)
                        eventos.Add(evento);
                }

                return eventos;
            }
        }


        public static bool BorrarEvento(int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Evento WHERE ID = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDEvento });

                if (existe != 1)
                    return false;
                string borrarEvento = "UPDATE Evento SET Eliminada = 1 WHERE ID = @IDEvento";
                connection.Execute(borrarEvento, new { IDEvento });

                return true;
            }
        }

        public static bool EstaInscripto(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDEvento = @IDEvento AND IDUsuario = @IDUsuario";

                List<int> listarUsuarios = connection.QueryFirstOrDefault<List<int>>(query, new { IDEvento, IDUsuario });

                return listarUsuarios.Contains(IDUsuario);
            }
        }


        public static bool DesInscribirseAEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT 1 FROM Evento WHERE ID = @IDEvento AND Eliminada = 0";
                int? evento = connection.QueryFirstOrDefault<int?>(query, new { IDEvento });

                if (evento == 1)
                {
                    string queryUsuario = "SELECT 1 FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                    int usuarioInscripto = connection.QueryFirstOrDefault<int>(queryUsuario, new { IDUsuario, IDEvento });

                    string queryEliminada = "SELECT 1 FROM Evento WHERE ID = @IDEvento AND Eliminada = 0";
                    int eventoNoEliminado = connection.QueryFirstOrDefault<int>(queryEliminada, new { IDEvento });

                    if (usuarioInscripto == 0 && eventoNoEliminado == 1)
                    {
                        string updateQuery = "UPDATE Anotados SET DesInscribirse = 0 WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        bool DesInscribirse = false;
                        connection.Execute(updateQuery, new { IDUsuario, IDEvento, DesInscribirse });
                        string insertQuery = "INSERT INTO Anotados (IDUsuario, IDEvento, DesInscribirse) VALUES (@IDUsuario, @IDEvento, @DesInscribirse)";
                        connection.Execute(insertQuery, new { IDUsuario, IDEvento, DesInscribirse });
                        return true;
                    }
                    else
                    {
                        string updateQuery = "UPDATE Anotados SET DesInscribirse = 1 WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        int DesInscribirse = 1;
                        connection.Execute(updateQuery, new { IDUsuario, IDEvento, DesInscribirse });
                        string deleteQuery = "DELETE FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        connection.Execute(deleteQuery, new { IDUsuario, IDEvento, DesInscribirse });
                        return false;
                    }
                }
                return false;
            }
        }


        public static List<Anotados> MisAnotados(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string consulta = @"SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario AND DesInscribirse = 0";

                return connection.Query<Anotados>(consulta, new { IDUsuario }).ToList();
            }
        }


        public static bool CrearEvento(Evento NuevoEvento)
        {
            Anotados anotados = new Anotados();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Evento WHERE Titulo = @Titulo";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { Titulo = NuevoEvento.Titulo });
                DateTime fechaMinima = new DateTime(1800, 1, 1);
                if (existe != 1 && NuevoEvento.Fecha > fechaMinima)
                {
                    string query = "CrearEvento";
                    using (SqlConnection connection2 = new SqlConnection(_connectionString))
                    {
                        Evento evento2 = connection2.QueryFirstOrDefault<Evento>(query, new { NuevoEvento.IDCreador, NuevoEvento.Titulo, NuevoEvento.Descripcion, NuevoEvento.Duracion, NuevoEvento.Fecha, NuevoEvento.Localidad, NuevoEvento.Intereses, NuevoEvento.Foto, NuevoEvento.DesInscribirse, NuevoEvento.Eliminada, NuevoEvento.Capacidad }, commandType: CommandType.StoredProcedure);
                        BD.DesInscribirseAEvento(evento2.IDCreador, evento2.ID);
                    }
                    return true;
                }

                return false;
            }
        }

        public static bool ModificarEvento(Evento eventoEditado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Evento WHERE ID = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDEvento = eventoEditado.ID });

                if (existe == 1)
                {
                    string queryActualizar = @"UPDATE Evento SET Titulo = @Titulo, Descripcion = @Descripcion, Duracion = @Duracion ,Fecha = @Fecha, Localidad = @Localidad, Intereses = @Intereses, Foto = @Foto, DesInscribirse = @DesInscribirse, Eliminada = @Eliminada WHERE ID = @IDEvento";
                    connection.Execute(queryActualizar, new { eventoEditado.ID, eventoEditado.Titulo, eventoEditado.Descripcion, eventoEditado.Duracion, eventoEditado.Fecha, eventoEditado.Localidad, eventoEditado.Intereses, eventoEditado.Foto, eventoEditado.DesInscribirse, eventoEditado.Eliminada });
                    return true;
                }

                return false;
            }
        }
    }
}