using System.Collections.Generic;
using System.Linq;
using System.Web;
using OldMates.Models;
using Newtonsoft.Json;
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
        public static bool VerificarContraseña(string Username, string password)
        {

            Usuario x = new Usuario();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE Username = @Username";
                x = connection.QueryFirstOrDefault<Usuario>(query, new { Username = Username });
            }
            if (x == null || x.Contraseña != contraseña)
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
    }
}