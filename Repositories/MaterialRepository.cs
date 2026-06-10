using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class MaterialRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public List<Material> GetAllMaterials()
        {
            var materials = new List<Material>();
            string query = "SELECT `код материала` as Id, `наименование` as Name, `единица измерения` as Unit, `колво на складе` as Quantity, `срок годности` as ExpirationDays FROM `расходные материалы` ORDER BY `наименование`";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                materials.Add(new Material
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Unit = row["Unit"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ExpirationDays = row["ExpirationDays"] != DBNull.Value ? Convert.ToInt32(row["ExpirationDays"]) : (int?)null
                });
            }
            return materials;
        }

        public Material GetMaterialById(int id)
        {
            string query = "SELECT `код материала` as Id, `наименование` as Name, `единица измерения` as Unit, `колво на складе` as Quantity, `срок годности` as ExpirationDays FROM `расходные материалы` WHERE `код материала` = @id";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                var row = result.Rows[0];
                return new Material
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Unit = row["Unit"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ExpirationDays = row["ExpirationDays"] != DBNull.Value ? Convert.ToInt32(row["ExpirationDays"]) : (int?)null
                };
            }
            return null;
        }

        public bool UpdateMaterialQuantity(int materialId, int newQuantity)
        {
            string query = "UPDATE `расходные материалы` SET `колво на складе` = @quantity WHERE `код материала` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", materialId),
                new MySqlParameter("@quantity", newQuantity)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool ConsumeMaterial(int materialId, int quantity)
        {
            var material = GetMaterialById(materialId);
            if (material == null || material.Quantity < quantity)
                return false;

            return UpdateMaterialQuantity(materialId, material.Quantity - quantity);
        }

        public List<ServiceMaterial> GetServiceMaterials(int serviceId)
        {
            var serviceMaterials = new List<ServiceMaterial>();

            try
            {
                // Проверяем, существует ли таблица расход_услуг
                string checkTableQuery = "SHOW TABLES LIKE 'расход_услуг'";
                DataTable tableExists = db.ExecuteQuery(checkTableQuery);

                if (tableExists.Rows.Count == 0)
                {
                    // Таблицы нет - возвращаем пустой список
                    return serviceMaterials;
                }

                string query = @"SELECT ru.`код расхода` as Id, ru.`код услуги` as ServiceId, 
                                        ru.`код материала` as MaterialId, ru.`количество` as QuantityNeeded,
                                        m.`наименование` as MaterialName
                                FROM `расход_услуг` ru
                                JOIN `расходные материалы` m ON ru.`код материала` = m.`код материала`
                                WHERE ru.`код услуги` = @serviceId";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@serviceId", serviceId)
                };

                DataTable result = db.ExecuteQuery(query, parameters);

                foreach (DataRow row in result.Rows)
                {
                    serviceMaterials.Add(new ServiceMaterial
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ServiceId = Convert.ToInt32(row["ServiceId"]),
                        MaterialId = Convert.ToInt32(row["MaterialId"]),
                        MaterialName = row["MaterialName"].ToString(),
                        QuantityNeeded = Convert.ToInt32(row["QuantityNeeded"])
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetServiceMaterials: {ex.Message}");
            }

            return serviceMaterials;
        }

        public bool AddServiceMaterial(int serviceId, int materialId, int quantityNeeded)
        {
            try
            {
                string query = "INSERT INTO `расход_услуг` (`код услуги`, `код материала`, `количество`) VALUES (@serviceId, @materialId, @quantity)";
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@serviceId", serviceId),
                    new MySqlParameter("@materialId", materialId),
                    new MySqlParameter("@quantity", quantityNeeded)
                };
                return db.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка AddServiceMaterial: {ex.Message}");
                return false;
            }
        }

        public bool RecordConsumption(int appointmentId, int materialId, int quantityUsed)
        {
            try
            {
                // Создаём таблицу для расхода материалов, если её нет
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS `расход_материалов` (
                        `id` INT NOT NULL AUTO_INCREMENT,
                        `appointment_id` INT NOT NULL,
                        `material_id` INT NOT NULL,
                        `quantity_used` INT NOT NULL,
                        `consumption_date` DATETIME NOT NULL,
                        PRIMARY KEY (`id`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

                db.ExecuteNonQuery(createTableQuery);

                string query = @"INSERT INTO `расход_материалов` 
                                (`appointment_id`, `material_id`, `quantity_used`, `consumption_date`) 
                                VALUES (@appointmentId, @materialId, @quantity, @consumptionDate)";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@appointmentId", appointmentId),
                    new MySqlParameter("@materialId", materialId),
                    new MySqlParameter("@quantity", quantityUsed),
                    new MySqlParameter("@consumptionDate", DateTime.Now)
                };

                bool result = db.ExecuteNonQuery(query, parameters) > 0;
                if (result)
                {
                    ConsumeMaterial(materialId, quantityUsed);
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка RecordConsumption: {ex.Message}");
                return false;
            }
        }

        public List<MaterialConsumption> GetMaterialConsumptionByAppointment(int appointmentId)
        {
            var consumptions = new List<MaterialConsumption>();
            try
            {
                string query = @"SELECT rc.*, m.`наименование` as MaterialName 
                                FROM `расход_материалов` rc
                                JOIN `расходные материалы` m ON rc.material_id = m.`код материала`
                                WHERE rc.appointment_id = @appointmentId";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@appointmentId", appointmentId)
                };

                DataTable result = db.ExecuteQuery(query, parameters);

                foreach (DataRow row in result.Rows)
                {
                    consumptions.Add(new MaterialConsumption
                    {
                        Id = Convert.ToInt32(row["id"]),
                        AppointmentId = Convert.ToInt32(row["appointment_id"]),
                        MaterialId = Convert.ToInt32(row["material_id"]),
                        MaterialName = row["MaterialName"].ToString(),
                        QuantityUsed = Convert.ToInt32(row["quantity_used"]),
                        ConsumptionDate = Convert.ToDateTime(row["consumption_date"])
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetMaterialConsumptionByAppointment: {ex.Message}");
            }
            return consumptions;
        }
    }
}