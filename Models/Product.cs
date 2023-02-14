using Microsoft.EntityFrameworkCore;

namespace CoreWebAPIstore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        //FOREIGN KEY MISSING?
        public Category Category { get; set; }
        
    }
}
