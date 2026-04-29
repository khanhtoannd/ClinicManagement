using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblServices")]
    public class tblServices
    {
        [Key]
        public int ServiceID { get; set; }

        public string ServiceName { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }
    }
}