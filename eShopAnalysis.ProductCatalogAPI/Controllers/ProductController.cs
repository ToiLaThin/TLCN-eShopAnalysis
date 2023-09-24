using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
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
        public ProductDto GetOneProduct(Guid productId)
        {
            var result = _service.Get(productId);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<Product, ProductDto>(result.Data) : null;
            return resultDto;
        }

        [HttpGet("GetAllProduct")]
        public IEnumerable<ProductDto> GetAllProduct()
        {
            var result = _service.GetAll();
            var resultDto = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(result.Data);
            return resultDto;
        }

        [HttpPost("AddProduct")]
        public ProductDto AddProduct([FromBody] Product newProduct)
        {
            var result = _service.AddProduct(newProduct);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<Product,ProductDto>(result.Data) : null;
            return resultDto;
        }

        [HttpPost("UpdateProductSubCatalog")]
        public ProductDto UpdateProductSubCatalog([FromHeader] Guid productId, Guid newSubCatalogId, string newSubCatalogName) 
        {
            var result = _service.UpdateSubCatalog(productId, newSubCatalogId, newSubCatalogName);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<Product, ProductDto>(result.Data) : null;
            return resultDto;
        }

        [HttpGet("GetAllProductModels")]
        public IEnumerable<ProductModelDto> GetAllProductModels([FromHeader] Guid productId)
        {
            var result = _service.GetAllProductModels(productId);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<IEnumerable<ProductModel>, IEnumerable<ProductModelDto>>(result.Data) : null;
            return resultDto;
        }

        [HttpGet("GetProductModel")]
        public ProductModelDto GetProductModel([FromHeader] Guid productId, [FromHeader] Guid pModelId)
        {
            var result = _service.GetProductModel(productId, pModelId);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<ProductModel, ProductModelDto>(result.Data) : null;
            return resultDto;
        }

        [HttpPost("AddNewProductModel")]
        public ProductModelDto AddNewProductModel([FromHeader] Guid productId, [FromBody] ProductModel newProductModel)
        {
            var result = _service.AddNewProductModel(productId, newProductModel);
            var resultDto = (result.IsSuccess == true) ? _mapper.Map<ProductModel, ProductModelDto>(result.Data) : null;
            return resultDto;
        }

    }
}
