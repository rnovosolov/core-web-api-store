using Microsoft.EntityFrameworkCore;

namespace CoreWebAPIstore.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public string Category { get; set; }
}
}
