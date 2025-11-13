using Microsoft.Data.SqlClient;
using Dapper;


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

            Usuario x = new Usuario();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Username = @Username";
                x = connection.QueryFirstOrDefault<Usuario>(query, new { Username = Username });
            }
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

        public static List<Evento> MisActividades(int IDUsuario){
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario";
                return connection.Query<Evento>(query).ToList();
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

                string updateQuery = "UPDATE Anotados SET DesInscribirse = 1 WHERE IDEvento = @IDEvento";
                connection.Execute(updateQuery, new { IDEvento });
                string borrarEvento = "UPDATE Evento SET Eliminada = 1 WHERE ID = @IDEvento";
                connection.Execute(borrarEvento, new { IDEvento });

                return true;
            }
        }

        public static bool EstaInscripto(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDEvento = @IDEvento AND IDUsuario = @IDUsuario AND DesInscribirse = 1 AND Eliminada = 0";

                List<int> listarUsuarios = connection.QueryFirstOrDefault<List<int>>(query, new { IDEvento, IDUsuario });

                return listarUsuarios.Contains(IDUsuario);
            }
        }


        public static bool DesInscribirseAEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Evento WHERE ID = @IDEvento";
                bool evento = connection.QueryFirstOrDefault(query, new { IDEvento });

                if (evento)
                {
                    string queryUsuario = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento AND Eliminada = 0";
                    bool usuarioInscripto = connection.QueryFirstOrDefault(queryUsuario, new { IDUsuario, IDEvento });

                    if (!usuarioInscripto)
                    {
                        string updateQuery = "UPDATE Anotados SET DesInscribirse = 1 WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        connection.Execute(updateQuery, new { IDUsuario, IDEvento });
                        string insertQuery = "INSERT INTO Anotados (IDUsuario, IDEvento) VALUES (@IDUsuario, @IDEvento)";
                        connection.Execute(insertQuery, new { IDUsuario, IDEvento });
                        return true;
                    }
                    else
                    {
                        string updateQuery = "UPDATE Anotados SET DesInscribirse = 0 WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        connection.Execute(updateQuery, new { IDUsuario, IDEvento });
                        string insertQuery = "DELETE FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        connection.Execute(insertQuery, new { IDUsuario, IDEvento });
                        return false;
                    }
                }
                return false;
            }
        }

        public static List<Evento> ObtenerEventosInscripto(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario AND DesInscribirse = 1 AND Eliminada = 0";
                return connection.Query<Evento>(query, new { IDUsuario }).ToList();
            }
        }

        public static bool CrearEvento(Evento evento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Evento WHERE Titulo = @Titulo";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { Titulo = evento.Titulo });

                if (existe != 1)
                {
                    string queryInsertar = @"INSERT INTO Evento (Titulo, Descripcion, Fecha, Localidad, Intereses, Foto, DesInscribirse, Eliminada, Capacidad) 
                                            VALUES (@Titulo, @Descripcion, @Fecha, @Localidad, @Intereses, @Foto, @DesInscribirse, @Eliminada, @Capacidad)";
                    connection.Execute(queryInsertar, new { evento.Titulo, evento.Descripcion, evento.Fecha, evento.Localidad, evento.Intereses, evento.Foto, evento.DesInscribirse, evento.Eliminada, evento.Capacidad  });
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
                    string queryActualizar = @"UPDATE Evento SET Titulo = @Titulo, Descripcion = @Descripcion, Fecha = @Fecha, Localidad = @Localidad, Intereses = @Intereses, Foto = @Foto, DesInscribirse = @DesInscribirse, Eliminada = @Eliminada WHERE ID = @IDEvento";
                    connection.Execute(queryActualizar, new { eventoEditado.ID, eventoEditado.Titulo, eventoEditado.Descripcion, eventoEditado.Fecha, eventoEditado.Localidad, eventoEditado.Intereses, eventoEditado.Foto, eventoEditado.DesInscribirse, eventoEditado.Eliminada });
                    return true;
                }

                return false;
            }
        }
         

    }
}