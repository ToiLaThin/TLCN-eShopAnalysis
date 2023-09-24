using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Services;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ProductCatalogAPI.Controllers
{
    [Route("api/ProductAPI")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;

        public ProductController(IProductService service, IMapper mapper)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet("GetOneProduct")]
        public Product GetOneProduct(Guid productId)
        {
            var result = _service.Get(productId);
            return result;
        }

        [HttpGet("GetAllProduct")]
        public IEnumerable<Product> GetAllProduct()
        {
            var result = _service.GetAll();
            return result;
        }

        [HttpPost("AddProduct")]
        public Product GetAllProduct([FromBody] Product newProduct)
        {
            var result = _service.AddProduct(newProduct);
            return result;
        }

        [HttpPost("UpdateProductSubCatalog")]
        public Product UpdateProductSubCatalog([FromHeader] Guid productId, Guid newSubCatalogId, string newSubCatalogName) 
        {
            var result = _service.UpdateSubCatalog(productId, newSubCatalogId, newSubCatalogName);
            return result;
        }

        [HttpPost("AddNewProductModel")]
        public ProductModel AddNewProductModel([FromHeader] Guid productId, [FromBody] ProductModel newProductModel)
        {
            var result = _service.AddNewProductModel(productId, newProductModel);
            return result;
        }

    }
}
