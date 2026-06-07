using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class ClientRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();
            string query = "SELECT `код клиента`, `фио`, `телефон`, `дата рождения` FROM `клиенты` ORDER BY `фио`";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                clients.Add(new Client
                {
                    Id = Convert.ToInt32(row["код клиента"]),
                    FullName = row["фио"].ToString(),
                    Phone = row["телефон"].ToString(),
                    BirthDate = row["дата рождения"] != DBNull.Value ? Convert.ToDateTime(row["дата рождения"]) : (DateTime?)null,
                    RegistrationDate = DateTime.Now // В базе нет поля, устанавливаем текущую дату
                });
            }
            return clients;
        }

        public List<Client> SearchClients(string searchText)
        {
            var clients = new List<Client>();
            string query = @"SELECT `код клиента`, `фио`, `телефон`, `дата рождения` 
                            FROM `клиенты` 
                            WHERE `фио` LIKE @search OR `телефон` LIKE @search 
                            ORDER BY `фио`";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@search", $"%{searchText}%")
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                clients.Add(new Client
                {
                    Id = Convert.ToInt32(row["код клиента"]),
                    FullName = row["фио"].ToString(),
                    Phone = row["телефон"].ToString(),
                    BirthDate = row["дата рождения"] != DBNull.Value ? Convert.ToDateTime(row["дата рождения"]) : (DateTime?)null,
                    RegistrationDate = DateTime.Now
                });
            }
            return clients;
        }

        public bool AddClient(Client client)
        {
            string query = @"INSERT INTO `клиенты` (`фио`, `телефон`, `дата рождения`) 
                            VALUES (@name, @phone, @birthDate)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", client.FullName),
                new MySqlParameter("@phone", client.Phone),
                new MySqlParameter("@birthDate", client.BirthDate ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateClient(Client client)
        {
            string query = @"UPDATE `клиенты` 
                            SET `фио` = @name, `телефон` = @phone, `дата рождения` = @birthDate 
                            WHERE `код клиента` = @id";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", client.Id),
                new MySqlParameter("@name", client.FullName),
                new MySqlParameter("@phone", client.Phone),
                new MySqlParameter("@birthDate", client.BirthDate ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteClient(int id)
        {
            string query = "DELETE FROM `клиенты` WHERE `код клиента` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}