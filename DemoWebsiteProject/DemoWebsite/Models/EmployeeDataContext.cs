using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebsite.Models
{
    public class Employee
    {
        public long id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Office { get; set; }
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
        public double Salary { get; set; }
    }

    public class EmployeeDataContext : DbContext
    {        
        public DbSet<Employee> Employees { get; set; }

        public EmployeeDataContext(DbContextOptions<EmployeeDataContext> options) : base(options)
        {          
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception exception)
            {
                if (exception is SqlException)
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Connection to SQL Server is not available");
                }
                else
                {
                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("Unhandled exception");
                }
                throw;
            }
        }
    }
}
