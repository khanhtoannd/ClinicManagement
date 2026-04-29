using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblInvoiceDetails")]
    public class tblInvoiceDetails
    {
        [Key]
        public int InvoiceDetailID { get; set; }

        public int InvoiceID { get; set; }

        public int ServiceID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string Note { get; set; }
    }
}