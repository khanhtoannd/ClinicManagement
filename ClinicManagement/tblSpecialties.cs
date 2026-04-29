using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblSpecialties")]
    public class tblSpecialties
    {
        [Key]
        public int SpecialtyID { get; set; }

        public string SpecialtyName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}