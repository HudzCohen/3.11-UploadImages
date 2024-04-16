using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadImage.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (Password, ImagePath, Views) " +
                              "VALUES (@password, @imagePath, 0) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@imagePath", image.ImagePath);
            connection.Open();
            var id = (int)(decimal)cmd.ExecuteScalar();
            return id;
        }

        public Image GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Image image = new Image()
            {
                Id = id,
                Password = (string)reader["Password"],
                ImagePath = (string)reader["ImagePath"],
                Views = (int)reader["Views"]
            };
            return image;
        }
       
        public void IncrementView(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = views + 1 WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
    }
}
