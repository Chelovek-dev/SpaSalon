using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class JournalRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public void AddEntry(int userId, string userName, string action, string details)
        {
            try
            {
                // Создаем таблицу для журнала, если её нет
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS `журнал_действий` (
                        `id` INT(10) NOT NULL AUTO_INCREMENT,
                        `user_id` INT(10) NOT NULL,
                        `user_name` VARCHAR(100) NOT NULL,
                        `action` VARCHAR(255) NOT NULL,
                        `details` TEXT,
                        `timestamp` DATETIME NOT NULL,
                        PRIMARY KEY (`id`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

                db.ExecuteNonQuery(createTableQuery);

                // Добавляем запись
                string insertQuery = @"
                    INSERT INTO `журнал_действий` 
                    (`user_id`, `user_name`, `action`, `details`, `timestamp`) 
                    VALUES (@userId, @userName, @action, @details, @timestamp)";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@userName", userName),
                    new MySqlParameter("@action", action),
                    new MySqlParameter("@details", details),
                    new MySqlParameter("@timestamp", DateTime.Now)
                };

                db.ExecuteNonQuery(insertQuery, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка записи в журнал: {ex.Message}");
            }
        }

        public List<JournalEntry> GetEntries(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                string query = "SELECT * FROM `журнал_действий` WHERE 1=1";
                var parameters = new List<MySqlParameter>();

                if (fromDate.HasValue)
                {
                    query += " AND `timestamp` >= @fromDate";
                    parameters.Add(new MySqlParameter("@fromDate", fromDate.Value));
                }

                if (toDate.HasValue)
                {
                    query += " AND `timestamp` <= @toDate";
                    parameters.Add(new MySqlParameter("@toDate", toDate.Value));
                }

                query += " ORDER BY `timestamp` DESC";

                DataTable result = db.ExecuteQuery(query, parameters.ToArray());
                var entries = new List<JournalEntry>();

                foreach (DataRow row in result.Rows)
                {
                    entries.Add(new JournalEntry
                    {
                        Id = Convert.ToInt32(row["id"]),
                        UserId = Convert.ToInt32(row["user_id"]),
                        UserName = row["user_name"].ToString(),
                        Action = row["action"].ToString(),
                        Details = row["details"].ToString(),
                        Timestamp = Convert.ToDateTime(row["timestamp"])
                    });
                }

                return entries;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения журнала: {ex.Message}", ex);
            }
        }
    }
}