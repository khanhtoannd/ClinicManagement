using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblMedicalRecords")]
    public class tblMedicalRecords
    {
        [Key]
        public int RecordID { get; set; }

        public int AppointmentID { get; set; }

        public string Symptoms { get; set; }

        public string Diagnosis { get; set; }

        public string Treatment { get; set; }

        public string Conclusion { get; set; }

        public DateTime RecordDate { get; set; }
    }
}