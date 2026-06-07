using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
	public class ServiceRepository
	{
		private DatabaseHelper db = new DatabaseHelper();

		public List<Service> GetAllServices()
		{
			var services = new List<Service>();
			string query = "SELECT `код услуги`, `наименование услуги`, `стоимость`, `длительность` FROM `услуги`";

			DataTable result = db.ExecuteQuery(query);

			foreach (DataRow row in result.Rows)
			{
				services.Add(new Service
				{
					Id = Convert.ToInt32(row["код услуги"]),
					Name = row["наименование услуги"].ToString(),
					Cost = Convert.ToDecimal(row["стоимость"]),
					Duration = Convert.ToInt32(row["длительность"])
				});
			}
			return services;
		}

		public bool AddService(Service service)
		{
			string query = @"INSERT INTO `услуги` (`наименование услуги`, `стоимость`, `длительность`) 
                            VALUES (@name, @cost, @duration)";

			var parameters = new MySqlParameter[]
			{
				new MySqlParameter("@name", service.Name),
				new MySqlParameter("@cost", service.Cost),
				new MySqlParameter("@duration", service.Duration)
			};

			return db.ExecuteNonQuery(query, parameters) > 0;
		}

		public bool UpdateService(Service service)
		{
			string query = @"UPDATE `услуги` 
                            SET `наименование услуги` = @name, `стоимость` = @cost, `длительность` = @duration 
                            WHERE `код услуги` = @id";

			var parameters = new MySqlParameter[]
			{
				new MySqlParameter("@id", service.Id),
				new MySqlParameter("@name", service.Name),
				new MySqlParameter("@cost", service.Cost),
				new MySqlParameter("@duration", service.Duration)
			};

			return db.ExecuteNonQuery(query, parameters) > 0;
		}

		public bool DeleteService(int id)
		{
			string query = "DELETE FROM `услуги` WHERE `код услуги` = @id";
			var parameters = new MySqlParameter[]
			{
				new MySqlParameter("@id", id)
			};
			return db.ExecuteNonQuery(query, parameters) > 0;
		}
	}
}