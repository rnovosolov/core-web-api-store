using CoreWebAPIstore.DTO;
using CoreWebAPIstore.Interfaces;
using CoreWebAPIstore.Models;
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

            this._productRepository = productRepository;

        }

        //GET api/Products/4
        [HttpGet("{id}")]
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
        [HttpGet]
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

            var mappedProduct = _productRepository.MapFromDTO(newProductDTO);

            if (!_productRepository.AddProduct(mappedProduct))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Product added");

        }


        //PATCH api/Product/9
        [HttpPatch("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(int id, [FromBody]ProductDTO updatedProductDTO)
        {
            if (updatedProductDTO == null || id != updatedProductDTO.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(id))
                return NotFound();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mappedProduct = _productRepository.MapFromDTO(updatedProductDTO);

            if (!_productRepository.UpdateProduct(mappedProduct))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE api/Products/9
        [HttpDelete("{id}")]
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

            return Ok(); 

        }

    }
}
