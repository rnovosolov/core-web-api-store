using CoreWebAPIstore.DTO;
using CoreWebAPIstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CoreWebAPIstore.Interfaces
{
    public interface IProductRepository
    {

        //--------------- CRUD Methods -----------------------
        public bool AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public bool DeleteProduct(Product product);
        public bool Save();


        //----------------- Existion checks ------------------
        public bool ProductExists(int id);
        public bool ProductOfCategoryExists(string categoryName);


        //------------------- Mapping DTO --------------------
        public Product MapFromDTO(ProductDTO productDTO);
        public ProductDTO MapToDTO(Product product);


        //----------------- Get Products ---------------------
        public ICollection<Product> GetProducts();
        public Product GetProductById(int id);
        public ICollection<Product> GetProductsByCategoryName(string categoryName);



        //- изменение количества товаров на складе
        //POST api/Products/{Id}/Quantity

        //- добавить/изменить атрибуты товара
        //POST api/Product/{Id}/Attribute
        //PATCH api/Products/{Id}/ Attribute+




    }
}
