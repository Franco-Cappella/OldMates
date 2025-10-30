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

        //REVISAR ESTO ESTA MAL
        public static bool EstaInscripto(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT 1 FROM Eventos WHERE IDUsuario = @IDUsuario AND IDContraseña = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(query, new { IDUsuario, IDEvento });
                return existe == 1;
            }
        }


        // Inscribirse a un evento
        public static void InscribirseAEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Usuarios_Eventos (idUsuario, idContraseña) VALUES (@idUsuario, @idEvento)";
                connection.Execute(query, new { idUsuario, idEvento });
            }
        }

        // Desinscribirse de un evento
        public static void DesinscribirseDeEvento(int idUsuario, int idEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Usuarios_Eventos WHERE idUsuario = @idUsuario AND idContraseña = @idEvento";
                connection.Execute(query, new { idUsuario, idEvento });
            }
        }

        // Obtener todos los eventos en los que el usuario está inscripto
        public static List<Evento> ObtenerEventosInscripto(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT e.* 
                                 FROM Eventos e
                                 INNER JOIN Usuarios_Eventos ue ON ue.idContraseña = e.ID
                                 WHERE ue.idUsuario = @idUsuario";
                return connection.Query<Evento>(query, new { idUsuario }).ToList();
            }
        }
    }
}