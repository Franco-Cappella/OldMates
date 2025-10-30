using Microsoft.Data.SqlClient;
using Dapper;


namespace OldMates.Models
{
    public static class BD
    {
        private static string _connectionString = @"Server=localhost;DataBase SQL Ort; Integrated Security=True; TrustServer Certificate=True;";

        public static Usuario ObtenerPorUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE Username = @Username";
                return connection.QueryFirstOrDefault<Usuario>(query, new { Username = username });
            }
        }
        public static bool VerificarContraseña(string Username, string Contraseña)
        {

            Usuario x = new Usuario();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE Username = @Username";
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
                string QueryExiste = "SELECT * FROM Usuarios WHERE Contraseña = @Contraseña";
                int existe = connection.QueryFirstOrDefault<int>(QueryExiste, new { Username = usuario.Username });
                if (existe == null)
                {
                    string query = @"INSERT INTO Usuarios(Username, Contraseña, Localidad, Intereses, Nombre, Apellido)
                               VALUES (@Username, @Contraseña, @Localidad, @Intereses, @Nombre, @Apellido)";

                    connection.Execute(query, new { usuario.Username, usuario.Contraseña, usuario.Localidad, usuario.Intereses, usuario.Nombre, usuario.Apellido });
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
                string query = "SELECT * FROM Eventos WHERE ID = @IDEvento";
                return connection.QueryFirstOrDefault<Evento>(query, new { IDEvento = IDEvento });
            }
        }

        public static List<Evento> ObtenerEventos()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Eventos";
                return connection.Query<Evento>(query).ToList();
            }
        }


        public static bool BorrarEvento(int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Eventos WHERE ID = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDEvento });

                if (existe != 1)
                    return false;

                string borrarEvento = "DELETE FROM Eventos WHERE ID = @IDEvento";
                connection.Execute(borrarEvento, new { IDEvento });

                return true;
            }
        }

        public static bool EstaInscripto(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDEvento = @IDEvento";

                List<int> listarUsuarios = connection.QueryFirstOrDefault<List<int>>(query, new { IDEvento });

                return listarUsuarios.Contains(IDUsuario);
            }
        }


        public static bool InscribirseAEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Evento WHERE ID = @IDEvento";
                bool evento = connection.QueryFirstOrDefault(query, new { IDEvento });

                if (evento)
                {
                    string queryUsuario = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                    bool usuarioInscripto = connection.QueryFirstOrDefault(queryUsuario, new { IDUsuario, IDEvento });

                    if (!usuarioInscripto)
                    {
                        string insertQuery = "INSERT INTO Anotados (IDUsuario, IDEvento) VALUES (@IDUsuario, @IDEvento)";
                        connection.Execute(insertQuery, new { IDUsuario, IDEvento });
                        return true;
                    }
                }
                return false;
            }
        }



        public static bool DesinscribirseDeEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryEvento = "SELECT * FROM Evento WHERE ID = @IDEvento";
                var evento = connection.QueryFirstOrDefault(queryEvento, new { IDEvento });

                if (evento != null)
                {
                    string queryUsuario = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                    var usuarioInscripto = connection.QueryFirstOrDefault(queryUsuario, new { IDUsuario, IDEvento });

                    if (usuarioInscripto != null)
                    {
                        string deleteQuery = "DELETE FROM Anotados WHERE IDUsuario = @IDUsuario AND IDEvento = @IDEvento";
                        connection.Execute(deleteQuery, new { IDUsuario, IDEvento });
                        return true;
                    }
                }

                return false;
            }
        }

        public static List<Evento> ObtenerEventosInscripto(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Anotados WHERE IDUsuario = @IDUsuario";
                return connection.Query<Evento>(query, new { IDUsuario }).ToList();
            }
        }

        public static bool CrearEvento(Evento evento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Eventos WHERE Nombre = @Nombre";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { Nombre = evento.Titulo });

                if (existe != 1)
                {
                    string queryInsertar = @"INSERT INTO Eventos (Nombre, Descripcion, Fecha, Localidad, Intereses) 
                                            VALUES (@Nombre, @Descripcion, @Fecha, @Localidad, @Intereses)";
                    connection.Execute(queryInsertar, new { evento.Titulo, evento.Descripcion, evento.Fecha, evento.Localidad, evento.Intereses });
                    return true;
                }

                return false;
            }
        }

        public static bool ModificarEvento(Evento eventoEditado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Verificamos si el evento existe
                string queryExiste = "SELECT 1 FROM Eventos WHERE ID = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDEvento = eventoEditado.ID });

                if (existe == 1)
                {
                    // Actualizamos el evento con los nuevos datos
                    string queryActualizar = @"UPDATE Eventos SET Titulo = @Titulo, Descripcion = @Descripcion, Fecha = @Fecha, Localidad = @Localidad, Intereses = @Intereses WHERE ID = @IDEvento";
                    connection.Execute(queryActualizar, new
                    {
                        eventoEditado.ID,
                        eventoEditado.Titulo,
                        eventoEditado.Descripcion,
                        eventoEditado.Fecha,
                        eventoEditado.Localidad,
                        eventoEditado.Intereses
                    });
                    return true;  // Evento actualizado exitosamente
                }

                return false;  // El evento no existe
            }
        }

    }
}