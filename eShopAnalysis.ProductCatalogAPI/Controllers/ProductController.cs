using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Application.Services;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using eShopAnalysis.ProductCatalogAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Net;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<Product>> GetOneProduct(Guid productId)
        {
            var serviceResult = _service.Get(productId);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Product, ProductDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpGet("GetAllProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>) , StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        //not use as attribute but to get the service from DI container
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            var serviceResult = _service.GetAll();
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult); 
                //will create http response error with error message(the oen pass in NotFound) in angular
                //https://angular.io/api/common/http/HttpErrorResponse#description
            }
            var resultDto = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(serviceResult.Data);
            if (resultDto.Count()  > 0)
            {
                return Ok(resultDto);
            }
            return NoContent();
        }

        [HttpPost("AddProduct")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.AdminPolicy,
            Roles = RoleType.Admin
           )
        ]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductDto>> AddProduct([FromBody] Product newProduct)
        {
            var serviceResult = await _service.AddProduct(newProduct);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ? 
                                         Ok(_mapper.Map<Product, ProductDto>(serviceResult.Data)) : 
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPost("UpdateProductSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductDto>> UpdateProductSubCatalog([FromHeader] Guid productId, Guid newSubCatalogId, string newSubCatalogName) 
        {
            var serviceResult = _service.UpdateSubCatalog(productId, newSubCatalogId, newSubCatalogName);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Product, ProductDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpGet("GetAllProductModels")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<ProductModelDto>>> GetAllProductModels([FromHeader] Guid productId)
        {
            var serviceResult = _service.GetAllProductModels(productId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<ProductModel>, IEnumerable<ProductModelDto>>(serviceResult.Data);
            if (resultDto.Any())
                return Ok(resultDto);
            return NoContent();

        }

        [HttpGet("GetProductModel")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductModelDto>> GetProductModel([FromHeader] Guid productId, [FromHeader] Guid pModelId)
        {
            var serviceResult = _service.GetProductModel(productId, pModelId);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<ProductModel, ProductModelDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPost("AddNewProductModel")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductModelDto>> AddNewProductModel([FromHeader] Guid productId, [FromBody] ProductModel newProductModel)
        {
            var serviceResult = _service.AddNewProductModel(productId, newProductModel);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<ProductModel, ProductModelDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }


        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/UpdateProductToOnSale")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public BackChannelResponseDto<ProductDto> UpdateProductToOnSale([FromBody] ProductUpdateToSaleRequestDto productUpdateToSaleRequestDto)
        {
            var serviceResult = _service.UpdateProductToOnSale(productUpdateToSaleRequestDto.ProductId,
                                                        productUpdateToSaleRequestDto.ProductModelId,
                                                        productUpdateToSaleRequestDto.SaleItemId,
                                                        productUpdateToSaleRequestDto.DiscountType,
                                                        productUpdateToSaleRequestDto.DiscountValue);

            if (serviceResult.IsFailed)
            {
                return BackChannelResponseDto<ProductDto>.Failure(serviceResult.Error);
            } else if (serviceResult.IsException)
            {
                return BackChannelResponseDto<ProductDto>.Exception(serviceResult.Error);
            }
            var productDto = _mapper.Map<ProductDto>(serviceResult.Data);
            return BackChannelResponseDto<ProductDto>.Success(productDto);
        }
    }
}
