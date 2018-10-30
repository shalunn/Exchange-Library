using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Models
{
    public class FileEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public double Size { get; set; }
        public string Guid { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class FilesDataContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; }

        public FilesDataContext(DbContextOptions<FilesDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
