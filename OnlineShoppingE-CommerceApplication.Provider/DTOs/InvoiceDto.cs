using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class InvoiceDto
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public double SummaryTotal { get; set; }
        public double SummaryTax { get; set; }
        public double SummaryNetTotal { get; set; }
        public List<OrderInvoice>? Orders { get; set; }
       
    }
    public class OrderInvoice
    {
        public string ProductName { get; set; }
        public int ProductVariantId { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
    }
}
