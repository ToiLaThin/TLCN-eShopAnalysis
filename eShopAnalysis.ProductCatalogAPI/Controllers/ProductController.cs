using AutoMapper;
using eShopAnalysis.IdentityServer.Utilities;
using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Application.Services;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Controllers
{
    [Route("api/ProductCatalogAPI/ProductAPI")]
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
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.AdminPolicy,
            Roles = RoleType.Admin
           )
        ]
        public async Task<ProductDto> AddProduct([FromBody] Product newProduct)
        {
            var result = await _service.AddProduct(newProduct);
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


        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/UpdateProductToOnSale")]
        public BackChannelResponseDto<ProductDto> UpdateProductToOnSale([FromBody] ProductUpdateToSaleRequestDto productUpdateToSaleRequestDto)
        {
            var result = _service.UpdateProductToOnSale(productUpdateToSaleRequestDto.ProductId,
                                                        productUpdateToSaleRequestDto.ProductModelId,
                                                        productUpdateToSaleRequestDto.DiscountType,
                                                        productUpdateToSaleRequestDto.DiscountValue);
            if (result.IsFailed)
            {
                return BackChannelResponseDto<ProductDto>.Failure(result.Error);
            } else if (result.IsException)
            {
                return BackChannelResponseDto<ProductDto>.Exception(result.Error);
            }
            var productDto = _mapper.Map<ProductDto>(result.Data);
            return BackChannelResponseDto<ProductDto>.Success(productDto);
        }
    }
}
