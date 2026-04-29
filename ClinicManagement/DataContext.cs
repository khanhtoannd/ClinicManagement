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
        public DbSet<tblAppointments> Appointments { get; set; }
        public DbSet<tblMedicalRecords> MedicalRecords { get; set; }
        public DbSet<tblServices> Services { get; set; }
        public DbSet<tblInvoices> Invoices { get; set; }
        public DbSet<tblInvoiceDetails> InvoiceDetails { get; set; }


    }
}