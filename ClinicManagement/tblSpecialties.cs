using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblSpecialties")]
    internal class tblSpecialties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecialtyID { get; set; }

        [Required]
        [StringLength(100)]
        public string SpecialtyName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}