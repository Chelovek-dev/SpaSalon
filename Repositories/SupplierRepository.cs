using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class SupplierRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public List<Supplier> GetAllSuppliers()
        {
            var suppliers = new List<Supplier>();
            string query = "SELECT `код поставщика` as Id, `наименование` as Name, `телефон` as Phone FROM `поставщики` ORDER BY `наименование`";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                suppliers.Add(new Supplier
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Phone = row["Phone"].ToString()
                });
            }
            return suppliers;
        }

        public Supplier GetSupplierById(int id)
        {
            string query = "SELECT `код поставщика` as Id, `наименование` as Name, `телефон` as Phone FROM `поставщики` WHERE `код поставщика` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                var row = result.Rows[0];
                return new Supplier
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Phone = row["Phone"].ToString()
                };
            }
            return null;
        }

        public bool AddSupplier(Supplier supplier)
        {
            string query = "INSERT INTO `поставщики` (`наименование`, `телефон`) VALUES (@name, @phone)";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", supplier.Name),
                new MySqlParameter("@phone", supplier.Phone)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            string query = "UPDATE `поставщики` SET `наименование` = @name, `телефон` = @phone WHERE `код поставщика` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", supplier.Id),
                new MySqlParameter("@name", supplier.Name),
                new MySqlParameter("@phone", supplier.Phone)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteSupplier(int id)
        {
            string query = "DELETE FROM `поставщики` WHERE `код поставщика` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public List<Purchase> GetAllPurchases()
        {
            var purchases = new List<Purchase>();
            string query = @"
                SELECT 
                    z.`код закупки` as Id,
                    z.`код поставщика` as SupplierId,
                    p.`наименование` as SupplierName,
                    z.`код материала` as MaterialId,
                    m.`наименование` as MaterialName,
                    z.`колво` as Quantity,
                    z.`цена` as Price,
                    z.`дата закупки` as PurchaseDate
                FROM `закупки` z
                JOIN `поставщики` p ON z.`код поставщика` = p.`код поставщика`
                JOIN `расходные материалы` m ON z.`код материала` = m.`код материала`
                ORDER BY z.`дата закупки` DESC";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                purchases.Add(new Purchase
                {
                    Id = Convert.ToInt32(row["Id"]),
                    SupplierId = Convert.ToInt32(row["SupplierId"]),
                    SupplierName = row["SupplierName"].ToString(),
                    MaterialId = Convert.ToInt32(row["MaterialId"]),
                    MaterialName = row["MaterialName"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    PurchaseDate = Convert.ToDateTime(row["PurchaseDate"])
                });
            }
            return purchases;
        }

        public bool AddPurchase(Purchase purchase)
        {
            string query = @"
                INSERT INTO `закупки` (`код поставщика`, `код материала`, `колво`, `цена`, `дата закупки`) 
                VALUES (@supplierId, @materialId, @quantity, @price, @date)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@supplierId", purchase.SupplierId),
                new MySqlParameter("@materialId", purchase.MaterialId),
                new MySqlParameter("@quantity", purchase.Quantity),
                new MySqlParameter("@price", purchase.Price),
                new MySqlParameter("@date", purchase.PurchaseDate)
            };

            bool result = db.ExecuteNonQuery(query, parameters) > 0;

            if (result)
            {
                // Обновляем количество материала на складе
                var materialRepo = new MaterialRepository();
                var material = materialRepo.GetMaterialById(purchase.MaterialId);
                if (material != null)
                {
                    materialRepo.UpdateMaterialQuantity(purchase.MaterialId, material.Quantity + purchase.Quantity);
                }
            }

            return result;
        }
    }
}