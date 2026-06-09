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
                    z.`код записи` as Id,
                    z.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    z.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    z.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    z.`дата и время` as DateTime,
                    z.`статус записи` as Status
                FROM `записи` z
                JOIN `клиенты` c ON z.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON z.`код мастера` = m.`код мастера`
                ORDER BY z.`дата и время` DESC";

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
                    z.`код записи` as Id,
                    z.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    z.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    z.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    z.`дата и время` as DateTime,
                    z.`статус записи` as Status
                FROM `записи` z
                JOIN `клиенты` c ON z.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON z.`код мастера` = m.`код мастера`
                WHERE z.`код мастера` = @masterId
                ORDER BY z.`дата и время` DESC";

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
                    z.`код записи` as Id,
                    z.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    z.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    z.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    z.`дата и время` as DateTime,
                    z.`статус записи` as Status
                FROM `записи` z
                JOIN `клиенты` c ON z.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON z.`код мастера` = m.`код мастера`
                WHERE DATE(z.`дата и время`) = @date
                ORDER BY z.`дата и время`";

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
                    z.`код записи` as Id,
                    z.`код клиента` as ClientId,
                    c.`фио` as ClientName,
                    c.`телефон` as ClientPhone,
                    z.`код услуги` as ServiceId,
                    u.`наименование услуги` as ServiceName,
                    u.`стоимость` as ServiceCost,
                    u.`длительность` as ServiceDuration,
                    z.`код мастера` as MasterId,
                    m.`фио` as MasterName,
                    z.`дата и время` as DateTime,
                    z.`статус записи` as Status
                FROM `записи` z
                JOIN `клиенты` c ON z.`код клиента` = c.`код клиента`
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                JOIN `мастера` m ON z.`код мастера` = m.`код мастера`
                WHERE DATE(z.`дата и время`) BETWEEN @startDate AND @endDate
                ORDER BY z.`дата и время`";

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

        public bool CheckMasterAvailability(int masterId, DateTime dateTime, int durationMinutes, int? excludeAppointmentId = null)
        {
            DateTime startTime = dateTime;
            DateTime endTime = dateTime.AddMinutes(durationMinutes);

            string query = @"
                SELECT COUNT(*) FROM `записи` z
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                WHERE z.`код мастера` = @masterId 
                AND z.`статус записи` NOT IN ('отменена', 'выполнена')
                AND (
                    (z.`дата и время` BETWEEN @startTime AND @endTime)
                    OR (DATE_ADD(z.`дата и время`, INTERVAL u.`длительность` MINUTE) BETWEEN @startTime AND @endTime)
                    OR (z.`дата и время` <= @startTime AND DATE_ADD(z.`дата и время`, INTERVAL u.`длительность` MINUTE) >= @endTime)
                )";

            if (excludeAppointmentId.HasValue)
            {
                query += " AND z.`код записи` != @excludeId";
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
                SELECT z.`дата и время`, u.`длительность`
                FROM `записи` z
                JOIN `услуги` u ON z.`код услуги` = u.`код услуги`
                WHERE z.`код мастера` = @masterId 
                AND DATE(z.`дата и время`) = @date
                AND z.`статус записи` NOT IN ('отменена', 'выполнена')";

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
                INSERT INTO `записи` 
                (`код клиента`, `код услуги`, `код мастера`, `дата и время`, `статус записи`) 
                VALUES (@clientId, @serviceId, @masterId, @dateTime, @status)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@clientId", appointment.ClientId),
                new MySqlParameter("@serviceId", appointment.ServiceId),
                new MySqlParameter("@masterId", appointment.MasterId),
                new MySqlParameter("@dateTime", appointment.DateTime),
                new MySqlParameter("@status", "новая")
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateAppointment(Appointment appointment)
        {
            string query = @"
                UPDATE `записи` 
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
            string query = "UPDATE `записи` SET `статус записи` = 'отменена' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool ConfirmAppointment(int appointmentId)
        {
            string query = "UPDATE `записи` SET `статус записи` = 'подтверждена' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool CompleteAppointment(int appointmentId, decimal actualCost)
        {
            string query = "UPDATE `записи` SET `статус записи` = 'выполнена' WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", appointmentId)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteAppointment(int id)
        {
            string query = "DELETE FROM `записи` WHERE `код записи` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}