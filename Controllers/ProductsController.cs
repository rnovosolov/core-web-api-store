﻿using CoreWebAPIstore.DTO;
using CoreWebAPIstore.Interfaces;
using CoreWebAPIstore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoreWebAPIstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {

            _productRepository = productRepository;

        }

        //GET api/Products/4
        [HttpGet("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [ProducesResponseType(200, Type = typeof(Product))]
        public IActionResult GetProduct(int id)
        {

            if (!_productRepository.ProductExists(id))
                return NotFound();

            var product = _productRepository.GetProductById(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var mappedProductDTO = _productRepository.MapToDTO(product);

            return Ok(mappedProductDTO);
        }


        //GET api/Products?category=Phones
        [HttpGet("{category}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDTO>))]
        public IActionResult GetProductsByCategoryName(string category)
        {

            if (!_productRepository.ProductOfCategoryExists(category))
                return NotFound();

            var products = _productRepository.GetProductsByCategoryName(category);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ProductDTO> mappedProductsDTO = new List<ProductDTO>();

            foreach (Product p in products)
            {
                var mappedProductDTO = _productRepository.MapToDTO(p);
                mappedProductsDTO.Add(mappedProductDTO);
            }

            return Ok(mappedProductsDTO);
        }


        //
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(204)] //FromBody
        public IActionResult AddProduct(ProductDTO newProductDTO)
        {
            if (newProductDTO == null)
            {
                return BadRequest(ModelState);
            }

            var product = _productRepository.GetProducts().Where(p => p.Name.Trim().ToUpper() == newProductDTO.Name.TrimEnd().ToUpper()).FirstOrDefault();

            if (product != null)
            {
                ModelState.AddModelError("", "Product already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mappedProduct = _productRepository.MapFromDTO(null, newProductDTO);

            if (!_productRepository.AddProduct(mappedProduct))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Product added");

        }


        [HttpPost("AddMultiple")]
        public IActionResult AddMultipleProducts([FromBody] List<ProductDTO> newProductsDTO)
        {
            if (newProductsDTO == null || !newProductsDTO.Any())
            {
                return BadRequest("No products provided.");
            }

            var addedProductIds = new List<int>();

            foreach (var newProductDTO in newProductsDTO)
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Invalid product data.");
                    continue; // Skip adding this product and proceed to the next one
                }

                var existingProductNames = _productRepository.GetProducts().Select(p => p.Name.Trim().ToUpper()).ToList();

                if (existingProductNames.Contains(newProductDTO.Name.TrimEnd().ToUpper()))
                {
                    ModelState.AddModelError("", $"Product '{newProductDTO.Name}' already exists.");
                    continue; // Skip adding this product and proceed to the next one
                }

                var mappedProduct = _productRepository.MapFromDTO(null, newProductDTO);

                if (_productRepository.AddProduct(mappedProduct))
                {
                    addedProductIds.Add(mappedProduct.Id);
                }
            }

            if (addedProductIds.Any())
            {
                return Ok($"Products with IDs: {string.Join(", ", addedProductIds)} added.");
            }
            else
            {
                ModelState.AddModelError("", "Failed to add any products.");
                return StatusCode(500, ModelState);
            }
        }


        //PATCH api/Product/9
        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(int id, [FromBody]ProductDTO updatedProductDTO)
        {
            if (updatedProductDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(id))
                return NotFound();

            var mappedProduct = _productRepository.MapFromDTO(id, updatedProductDTO);

            if (!_productRepository.UpdateProduct(mappedProduct))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Product updated.");
        }

        //POST api/Products/9/Quantity
        [HttpPost("{id:int}/Quantity")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult UpdateProductQuantity(int id, [FromBody] int newQuantity)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Quantity = newQuantity;
            _productRepository.UpdateProduct(product);

            return Ok("Product quantity updated.");
        }


        // DELETE api/Products/9
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteProduct(int id)
        {

            if (!_productRepository.ProductExists(id))
                return NotFound();

            var product = _productRepository.GetProductById(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Product deleted"); 

        }

    }
}
