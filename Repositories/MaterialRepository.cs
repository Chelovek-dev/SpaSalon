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

        public bool AddMaterial(Material material)
        {
            string query = "INSERT INTO `расходные материалы` (`наименование`, `единица измерения`, `колво на складе`, `срок годности`) VALUES (@name, @unit, @quantity, @expiration)";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", material.Name),
                new MySqlParameter("@unit", material.Unit),
                new MySqlParameter("@quantity", material.Quantity),
                new MySqlParameter("@expiration", material.ExpirationDays ?? (object)DBNull.Value)
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
            // Создаём таблицу связи услуг и материалов, если её нет
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS `услуги_материалы` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `service_id` INT NOT NULL,
                    `material_id` INT NOT NULL,
                    `quantity_needed` INT NOT NULL,
                    PRIMARY KEY (`id`),
                    FOREIGN KEY (`service_id`) REFERENCES `услуги`(`код услуги`),
                    FOREIGN KEY (`material_id`) REFERENCES `расходные материалы`(`код материала`)
                )";

            db.ExecuteNonQuery(createTableQuery);

            string query = @"SELECT um.*, m.`наименование` as MaterialName 
                            FROM `услуги_материалы` um
                            JOIN `расходные материалы` m ON um.material_id = m.`код материала`
                            WHERE um.service_id = @serviceId";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@serviceId", serviceId)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                serviceMaterials.Add(new ServiceMaterial
                {
                    Id = Convert.ToInt32(row["id"]),
                    ServiceId = Convert.ToInt32(row["service_id"]),
                    MaterialId = Convert.ToInt32(row["material_id"]),
                    QuantityNeeded = Convert.ToInt32(row["quantity_needed"])
                });
            }
            return serviceMaterials;
        }

        public bool AddServiceMaterial(int serviceId, int materialId, int quantityNeeded)
        {
            string query = "INSERT INTO `услуги_материалы` (`service_id`, `material_id`, `quantity_needed`) VALUES (@serviceId, @materialId, @quantity)";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@serviceId", serviceId),
                new MySqlParameter("@materialId", materialId),
                new MySqlParameter("@quantity", quantityNeeded)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool RemoveServiceMaterial(int serviceMaterialId)
        {
            string query = "DELETE FROM `услуги_материалы` WHERE `id` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", serviceMaterialId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool RecordConsumption(int appointmentId, int materialId, int quantityUsed)
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS `расход_материалов` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `appointment_id` INT NOT NULL,
                    `material_id` INT NOT NULL,
                    `quantity_used` INT NOT NULL,
                    `consumption_date` DATETIME NOT NULL,
                    PRIMARY KEY (`id`)
                )";

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

        public List<MaterialConsumption> GetMaterialConsumptionByAppointment(int appointmentId)
        {
            var consumptions = new List<MaterialConsumption>();
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
            return consumptions;
        }
    }
}