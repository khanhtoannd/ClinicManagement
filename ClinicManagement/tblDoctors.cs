using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblDoctors")]
    internal class tblDoctors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorID { get; set; }

        [Required]
        [StringLength(150)]
        public string DoctorName { get; set; }

        public int? SpecialtyID { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(150)]
        public string Qualification { get; set; }

        [StringLength(20)]
        public string RoomNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Status { get; set; }
    }
}