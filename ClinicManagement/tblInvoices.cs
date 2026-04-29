using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement
{
    [Table("tblInvoices")]
    public class tblInvoices
    {
        [Key]
        public int InvoiceID { get; set; }

        public int AppointmentID { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public int Status { get; set; }

        public string Note { get; set; }
    }
}