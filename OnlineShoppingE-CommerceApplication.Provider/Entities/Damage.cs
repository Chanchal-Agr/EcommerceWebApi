using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities;
public class Damage
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int StockId { get; set; }
    [ForeignKey("StockId")]
    public Stock? Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
}
