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
        public static List<int> ObtenerEventosInscripto(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT IDEvento FROM Anotados WHERE IDUsuario = @IDUsuario";
                return connection.Query<int>(query, new { IDUsuario }).ToList();
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

        public static List<int> EstaInscripto(int IDUsuario)
        {
            using (SqlConnection A = new SqlConnection(_connectionString))
            {
                string sql = "SELECT IDEvento FROM Anotados WHERE IDUsuario = @Usuario";
                return A.Query<int>(sql, new { Usuario = IDUsuario }).ToList();
            }
        }

        public static bool DesInscribirseAEvento(int IDUsuario, int IDEvento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT 1 FROM Evento WHERE ID = @IDEvento AND Eliminada = 0";
                int evento = connection.QueryFirstOrDefault<int>(query, new { IDEvento });

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

        public static bool ModificarEvento(Evento evento)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Evento WHERE ID = @IDEvento";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDEvento = evento.ID });

                if (existe == 1)
                {
                    string queryActualizar = @"UPDATE Evento SET Titulo = @Titulo, Descripcion = @Descripcion, Duracion = @Duracion, Fecha = @Fecha, Localidad = @Localidad, Intereses = @Intereses, Capacidad = @Capacidad, Foto = @Foto WHERE ID = @IDEvento";

                    connection.Execute(queryActualizar, new { IDEvento = evento.ID, Titulo = evento.Titulo, Descripcion = evento.Descripcion, Duracion = evento.Duracion, Fecha = evento.Fecha, Localidad = evento.Localidad, Intereses = evento.Intereses, Capacidad = evento.Capacidad, Foto = evento.Foto });

                    return true;
                }

                return false;
            }
        }
        public static List<Evento> ObtenerEventosDeHoy()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Evento WHERE Eliminada = 0 AND CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE) ORDER BY Fecha ASC";

                return connection.Query<Evento>(query).ToList();
            }
        }

        public static List<Evento> ObtenerProximosEventos()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Evento WHERE Eliminada = 0 AND Fecha > GETDATE() ORDER BY Fecha ASC";

                return connection.Query<Evento>(query).ToList();
            }
        }

        public static bool ActualizarUsuario(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryExiste = "SELECT 1 FROM Usuario WHERE ID = @ID";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { ID = usuario.ID });

                if (existe == 1)
                {
                    string queryActualizar = @"UPDATE Usuario SET Username = @Username, Contraseña = @Contraseña, Localidad = @Localidad, Intereses = @Intereses, Nombre = @Nombre, Apellido = @Apellido, Foto = @Foto WHERE ID = @ID";

                    connection.Execute(queryActualizar, new { ID = usuario.ID, Username = usuario.Username, Contraseña = usuario.Contraseña, Localidad = usuario.Localidad, Intereses = usuario.Intereses, Nombre = usuario.Nombre, Apellido = usuario.Apellido, Foto = usuario.Foto });

                    return true;
                }

                return false;
            }
        }

        public static bool EditarPerfil(Usuario usuario)
        {
            return ActualizarUsuario(usuario);
        }

        // ========== SISTEMA DE AMIGOS ==========

        public static List<Usuario> ObtenerAmigos(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT u.* FROM Usuario u
                    INNER JOIN Amistad a ON (a.IDUsuario1 = u.ID OR a.IDUsuario2 = u.ID)
                    WHERE (a.IDUsuario1 = @IDUsuario OR a.IDUsuario2 = @IDUsuario)
                    AND a.Estado = 'aceptada'
                    AND u.ID != @IDUsuario";
                return connection.Query<Usuario>(query, new { IDUsuario }).ToList();
            }
        }

        public static List<Amistad> ObtenerSolicitudesPendientes(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
    SELECT a.*, u.ID, u.Nombre, u.Apellido, u.Foto, u.Localidad, u.Intereses
    FROM Amistad a
    INNER JOIN Usuario u ON a.IDUsuario1 = u.ID
    WHERE a.IDUsuario2 = @IDUsuario AND a.Estado = 'pendiente'";
                
                var solicitudes = connection.Query<Amistad, Usuario, Amistad>(
                    query,
                    (amistad, usuario) => { amistad.Usuario1 = usuario; return amistad; },
                    new { IDUsuario },
                    splitOn: "ID"
                ).ToList();
                
                return solicitudes;
            }
        }

        public static bool EnviarSolicitudAmistad(int IDUsuario1, int IDUsuario2)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Verificar que no exista ya una solicitud
                string queryExiste = @"
                    SELECT 1 FROM Amistad 
                    WHERE ((IDUsuario1 = @IDUsuario1 AND IDUsuario2 = @IDUsuario2) 
                    OR (IDUsuario1 = @IDUsuario2 AND IDUsuario2 = @IDUsuario1))
                    AND Estado != 'rechazada'";
                int existe = connection.QueryFirstOrDefault<int>(queryExiste, new { IDUsuario1, IDUsuario2 });

                if (existe == 0 && IDUsuario1 != IDUsuario2)
                {
                    string query = @"
                        INSERT INTO Amistad (IDUsuario1, IDUsuario2, Estado, FechaSolicitud)
                        VALUES (@IDUsuario1, @IDUsuario2, 'pendiente', GETDATE())";
                    connection.Execute(query, new { IDUsuario1, IDUsuario2 });
                    return true;
                }
                return false;
            }
        }

        public static bool AceptarSolicitudAmistad(int IDSolicitud, int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Amistad 
                    SET Estado = 'aceptada', FechaRespuesta = GETDATE()
                    WHERE ID = @IDSolicitud AND IDUsuario2 = @IDUsuario AND Estado = 'pendiente'";
                int filas = connection.Execute(query, new { IDSolicitud, IDUsuario });
                return filas > 0;
            }
        }

        public static bool RechazarSolicitudAmistad(int IDSolicitud, int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Amistad 
                    SET Estado = 'rechazada', FechaRespuesta = GETDATE()
                    WHERE ID = @IDSolicitud AND IDUsuario2 = @IDUsuario AND Estado = 'pendiente'";
                int filas = connection.Execute(query, new { IDSolicitud, IDUsuario });
                return filas > 0;
            }
        }

        public static bool EliminarAmigo(int IDUsuario, int IDAmigo)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    DELETE FROM Amistad 
                    WHERE ((IDUsuario1 = @IDUsuario AND IDUsuario2 = @IDAmigo) 
                    OR (IDUsuario1 = @IDAmigo AND IDUsuario2 = @IDUsuario))
                    AND Estado = 'aceptada'";
                int filas = connection.Execute(query, new { IDUsuario, IDAmigo });
                return filas > 0;
            }
        }

        public static List<Usuario> BuscarUsuarios(string busqueda, int IDUsuarioActual)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT * FROM Usuario 
                    WHERE ID != @IDUsuarioActual 
                    AND (Nombre LIKE @Busqueda OR Apellido LIKE @Busqueda OR Username LIKE @Busqueda)";
                return connection.Query<Usuario>(query, new { IDUsuarioActual, Busqueda = "%" + busqueda + "%" }).ToList();
            }
        }

        public static string ObtenerEstadoAmistad(int IDUsuario1, int IDUsuario2)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT Estado FROM Amistad 
                    WHERE ((IDUsuario1 = @IDUsuario1 AND IDUsuario2 = @IDUsuario2) 
                    OR (IDUsuario1 = @IDUsuario2 AND IDUsuario2 = @IDUsuario1))
                    AND Estado != 'rechazada'";
                return connection.QueryFirstOrDefault<string>(query, new { IDUsuario1, IDUsuario2 });
            }
        }

        public static int ContarSolicitudesPendientes(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Amistad WHERE IDUsuario2 = @IDUsuario AND Estado = 'pendiente'";
                return connection.QueryFirstOrDefault<int>(query, new { IDUsuario });
            }
        }


        // ========== SISTEMA DE MENSAJES ==========

        public static List<Mensaje> ObtenerConversacion(int IDUsuario1, int IDUsuario2)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT * FROM Mensaje 
                    WHERE (IDEmisor = @IDUsuario1 AND IDReceptor = @IDUsuario2)
                    OR (IDEmisor = @IDUsuario2 AND IDReceptor = @IDUsuario1)
                    ORDER BY FechaEnvio ASC";
                return connection.Query<Mensaje>(query, new { IDUsuario1, IDUsuario2 }).ToList();
            }
        }

        public static bool EnviarMensaje(int IDEmisor, int IDReceptor, string contenido)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO Mensaje (IDEmisor, IDReceptor, Contenido, FechaEnvio, Leido)
                    VALUES (@IDEmisor, @IDReceptor, @Contenido, GETDATE(), 0)";
                int filas = connection.Execute(query, new { IDEmisor, IDReceptor, Contenido = contenido });
                return filas > 0;
            }
        }

        public static void MarcarMensajesComoLeidos(int IDUsuario, int IDAmigo)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE Mensaje SET Leido = 1 
                    WHERE IDEmisor = @IDAmigo AND IDReceptor = @IDUsuario AND Leido = 0";
                connection.Execute(query, new { IDUsuario, IDAmigo });
            }
        }

        public static List<Usuario> ObtenerConversacionesRecientes(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT u.*, 
                        (SELECT TOP 1 Contenido FROM Mensaje 
                         WHERE (IDEmisor = u.ID AND IDReceptor = @IDUsuario) 
                         OR (IDEmisor = @IDUsuario AND IDReceptor = u.ID)
                         ORDER BY FechaEnvio DESC) as Intereses,
                        (SELECT COUNT(*) FROM Mensaje 
                         WHERE IDEmisor = u.ID AND IDReceptor = @IDUsuario AND Leido = 0) as Admin
                    FROM Usuario u
                    INNER JOIN Mensaje m ON (m.IDEmisor = u.ID OR m.IDReceptor = u.ID)
                    WHERE (m.IDEmisor = @IDUsuario OR m.IDReceptor = @IDUsuario)
                    AND u.ID != @IDUsuario";
                return connection.Query<Usuario>(query, new { IDUsuario }).ToList();
            }
        }

        public static int ContarMensajesNoLeidos(int IDUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Mensaje WHERE IDReceptor = @IDUsuario AND Leido = 0";
                return connection.QueryFirstOrDefault<int>(query, new { IDUsuario });
            }
        }
    }
}