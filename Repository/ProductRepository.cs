using CoreWebAPIstore.DTO;
using CoreWebAPIstore.Interfaces;
using CoreWebAPIstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace CoreWebAPIstore.Repository
{
    public class ProductRepository : IProductRepository
    {

        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //---------------------- CRUD Methods ----------------------------

        public bool AddProduct(Product product)
        {
            _context.Add(product);
            return Save();
        }

        public bool UpdateProduct(Product product) 
        {
            _context.Update(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


        //------------------------ Existion checks ---------------------------------

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }

        public bool ProductOfCategoryExists(string categoryName)
        {
            return _context.Products.Any(p => p.Category.Name == categoryName);
        }


        //--------------------------- Mapping DTO -----------------------------------

        public Product MapFromDTO(ProductDTO productDTO)
        {
            Product product = new Product();

            product.Id = productDTO.Id;
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Quantity = productDTO.Quantity;
            product.Price = productDTO.Price;
            product.Category = _context.Categories.Where(c => c.Name == productDTO.Category).FirstOrDefault();

            return product;
        }

        public ProductDTO MapToDTO(Product product)
        {
            ProductDTO productDTO = new ProductDTO();

            productDTO.Id = product.Id;
            productDTO.Name = product.Name;
            productDTO.Description = product.Description;
            productDTO.Quantity = product.Quantity;
            productDTO.Price = product.Price;
            productDTO.Category = product.Category.Name;

            return productDTO;
        }


        //--------------------------- Get Products -----------------------------------

        public ICollection<Product> GetProducts()
        {
            return _context.Products.OrderBy(p => p.Id).ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();

        }

        public ICollection<Product> GetProductsByCategoryName(string categoryName)
        {
            return _context.Products.Include(p => p.Category).Where(p => p.Category.Name == categoryName).ToList();
        }


    }
}
