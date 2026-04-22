using System.Data.Entity;

namespace ClinicManagement
{
    internal class DataContext : DbContext
    {
        public DataContext() : base("name=MyConnectionString")
        {
        }

        public DbSet<tblUsers> Users { get; set; }
        public DbSet<tblPatients> Patients { get; set; }
        public DbSet<tblDoctors> Doctors { get; set; }
        public DbSet<tblSpecialties> Specialties { get; set; }
    }
}