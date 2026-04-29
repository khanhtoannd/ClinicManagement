using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblAppointments")]
    internal class tblAppointments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentID { get; set; }

        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }

        [StringLength(255)]
        public string Reason { get; set; }

        public int Status { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}