using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebsite.Models
{
    public class EmployeeDataContext : DbContext
    {        
        public DbSet<Employee> Employees { get; set; }

        public EmployeeDataContext(DbContextOptions<EmployeeDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
