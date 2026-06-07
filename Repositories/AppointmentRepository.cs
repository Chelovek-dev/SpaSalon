using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class AppointmentRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public List<Appointment> GetAllAppointments()
        {
            var appointments = new List<Appointment>();
            string query = @"
                SELECT 
                    a.`код записи` as Id,
                    a.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    a.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    a.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    a.`дата и время` as DateTime,
                    a.`статус записи` as Status
                FROM `администратор` a
                JOIN `клиенты` c ON a.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON a.`код мастера` = m.`код мастера`
                ORDER BY a.`дата и время` DESC";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                appointments.Add(new Appointment
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    ClientName = row["ClientName"].ToString(),
                    ClientPhone = row["ClientPhone"].ToString(),
                    ServiceId = Convert.ToInt32(row["ServiceId"]),
                    ServiceName = row["ServiceName"].ToString(),
                    ServiceCost = Convert.ToDecimal(row["ServiceCost"]),
                    ServiceDuration = Convert.ToInt32(row["ServiceDuration"]),
                    MasterId = Convert.ToInt32(row["MasterId"]),
                    MasterName = row["MasterName"].ToString(),
                    DateTime = Convert.ToDateTime(row["DateTime"]),
                    Status = row["Status"].ToString(),
                    ActualCost = Convert.ToDecimal(row["ServiceCost"])
                });
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByMaster(int masterId)
        {
            var appointments = new List<Appointment>();
            string query = @"
                SELECT 
                    a.`код записи` as Id,
                    a.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    a.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    a.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    a.`дата и время` as DateTime,
                    a.`статус записи` as Status
                FROM `администратор` a
                JOIN `клиенты` c ON a.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON a.`код мастера` = m.`код мастера`
                WHERE a.`код мастера` = @masterId
                ORDER BY a.`дата и время` DESC";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@masterId", masterId)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                appointments.Add(new Appointment
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    ClientName = row["ClientName"].ToString(),
                    ClientPhone = row["ClientPhone"].ToString(),
                    ServiceId = Convert.ToInt32(row["ServiceId"]),
                    ServiceName = row["ServiceName"].ToString(),
                    ServiceCost = Convert.ToDecimal(row["ServiceCost"]),
                    ServiceDuration = Convert.ToInt32(row["ServiceDuration"]),
                    MasterId = Convert.ToInt32(row["MasterId"]),
                    MasterName = row["MasterName"].ToString(),
                    DateTime = Convert.ToDateTime(row["DateTime"]),
                    Status = row["Status"].ToString(),
                    ActualCost = Convert.ToDecimal(row["ServiceCost"])
                });
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            var appointments = new List<Appointment>();
            string query = @"
                SELECT 
                    a.`код записи` as Id,
                    a.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    a.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    a.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    a.`дата и время` as DateTime,
                    a.`статус записи` as Status
                FROM `администратор` a
                JOIN `клиенты` c ON a.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON a.`код мастера` = m.`код мастера`
                WHERE DATE(a.`дата и время`) = @date
                ORDER BY a.`дата и время`";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@date", date)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                appointments.Add(new Appointment
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    ClientName = row["ClientName"].ToString(),
                    ClientPhone = row["ClientPhone"].ToString(),
                    ServiceId = Convert.ToInt32(row["ServiceId"]),
                    ServiceName = row["ServiceName"].ToString(),
                    ServiceCost = Convert.ToDecimal(row["ServiceCost"]),
                    ServiceDuration = Convert.ToInt32(row["ServiceDuration"]),
                    MasterId = Convert.ToInt32(row["MasterId"]),
                    MasterName = row["MasterName"].ToString(),
                    DateTime = Convert.ToDateTime(row["DateTime"]),
                    Status = row["Status"].ToString(),
                    ActualCost = Convert.ToDecimal(row["ServiceCost"])
                });
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByDateRange(DateTime startDate, DateTime endDate)
        {
            var appointments = new List<Appointment>();
            string query = @"
                SELECT 
                    a.`код записи` as Id,
                    a.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    a.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    a.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    a.`дата и время` as DateTime,
                    a.`статус записи` as Status
                FROM `администратор` a
                JOIN `клиенты` c ON a.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON a.`код мастера` = m.`код мастера`
                WHERE DATE(a.`дата и время`) BETWEEN @startDate AND @endDate
                ORDER BY a.`дата и время`";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@startDate", startDate),
                new MySqlParameter("@endDate", endDate)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                appointments.Add(new Appointment
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    ClientName = row["ClientName"].ToString(),
                    ClientPhone = row["ClientPhone"].ToString(),
                    ServiceId = Convert.ToInt32(row["ServiceId"]),
                    ServiceName = row["ServiceName"].ToString(),
                    ServiceCost = Convert.ToDecimal(row["ServiceCost"]),
                    ServiceDuration = Convert.ToInt32(row["ServiceDuration"]),
                    MasterId = Convert.ToInt32(row["MasterId"]),
                    MasterName = row["MasterName"].ToString(),
                    DateTime = Convert.ToDateTime(row["DateTime"]),
                    Status = row["Status"].ToString(),
                    ActualCost = Convert.ToDecimal(row["ServiceCost"])
                });
            }
            return appointments;
        }

        // ПРОВЕРКА ЗАНЯТОСТИ МАСТЕРА
        public bool CheckMasterAvailability(int masterId, DateTime dateTime, int durationMinutes, int? excludeAppointmentId = null)
        {
            DateTime startTime = dateTime;
            DateTime endTime = dateTime.AddMinutes(durationMinutes);

            string query = @"
                SELECT COUNT(*) FROM `администратор` a
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                WHERE a.`код мастера` = @masterId 
                AND a.`статус записи` NOT IN ('cancelled', 'completed')
                AND (
                    (a.`дата и время` BETWEEN @startTime AND @endTime)
                    OR (DATE_ADD(a.`дата и время`, INTERVAL u.`длительность` MINUTE) BETWEEN @startTime AND @endTime)
                    OR (a.`дата и время` <= @startTime AND DATE_ADD(a.`дата и время`, INTERVAL u.`длительность` MINUTE) >= @endTime)
                )";

            if (excludeAppointmentId.HasValue)
            {
                query += " AND a.`код записи` != @excludeId";
            }

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@masterId", masterId),
                new MySqlParameter("@startTime", startTime),
                new MySqlParameter("@endTime", endTime)
            };

            if (excludeAppointmentId.HasValue)
            {
                parameters.Add(new MySqlParameter("@excludeId", excludeAppointmentId.Value));
            }

            DataTable result = db.ExecuteQuery(query, parameters.ToArray());
            int count = Convert.ToInt32(result.Rows[0][0]);
            return count == 0;
        }

        public List<DateTime> GetMasterBusySlots(int masterId, DateTime date)
        {
            var busySlots = new List<DateTime>();
            string query = @"
                SELECT a.`дата и время`, u.`длительность`
                FROM `администратор` a
                JOIN `услуги` u ON a.`код услуги` = u.`код услуги`
                WHERE a.`код мастера` = @masterId 
                AND DATE(a.`дата и время`) = @date
                AND a.`статус записи` NOT IN ('cancelled', 'completed')";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@masterId", masterId),
                new MySqlParameter("@date", date)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                busySlots.Add(Convert.ToDateTime(row["дата и время"]));
            }
            return busySlots;
        }

        public bool AddAppointment(Appointment appointment)
        {
            string query = @"
                INSERT INTO `администратор` 
                (`код клиента`, `код услуги`, `код мастера`, `дата и время`, `статус записи`) 
                VALUES (@clientId, @serviceId, @masterId, @dateTime, @status)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@clientId", appointment.ClientId),
                new MySqlParameter("@serviceId", appointment.ServiceId),
                new MySqlParameter("@masterId", appointment.MasterId),
                new MySqlParameter("@dateTime", appointment.DateTime),
                new MySqlParameter("@status", "new")
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateAppointment(Appointment appointment)
        {
            string query = @"
                UPDATE `администратор` 
                SET `код клиента` = @clientId, 
                    `код услуги` = @serviceId, 
                    `код мастера` = @masterId, 
                    `дата и время` = @dateTime,
                    `статус записи` = @status
                WHERE `код записи` = @id";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointment.Id),
                new MySqlParameter("@clientId", appointment.ClientId),
                new MySqlParameter("@serviceId", appointment.ServiceId),
                new MySqlParameter("@masterId", appointment.MasterId),
                new MySqlParameter("@dateTime", appointment.DateTime),
                new MySqlParameter("@status", appointment.Status)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool CancelAppointment(int appointmentId, string reason)
        {
            string query = "UPDATE `администратор` SET `статус записи` = 'cancelled' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool ConfirmAppointment(int appointmentId)
        {
            string query = "UPDATE `администратор` SET `статус записи` = 'confirmed' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool CompleteAppointment(int appointmentId, decimal actualCost)
        {
            string query = "UPDATE `администратор` SET `статус записи` = 'completed' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteAppointment(int id)
        {
            string query = "DELETE FROM `администратор` WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}