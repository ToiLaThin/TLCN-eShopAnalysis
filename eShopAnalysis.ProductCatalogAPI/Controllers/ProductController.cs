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
using System.Collections.Generic;
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
            var serviceResult = await _service.Get(productId);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Product, ProductDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPost("GetProductsLazyLoad")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(PaginatedProductDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsLazyLoad([FromBody] ProductLazyLoadRequestDto lazyLoadRequestDto)
        {
            var serviceResult = await _service.GetAllMatching(lazyLoadRequestDto);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            PaginatedProductDto paginatedProductDto = new PaginatedProductDto() {
                Products = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(serviceResult.Data),
                PageCount = serviceResult.Data.Count() / lazyLoadRequestDto.ProductPerPage + 1,
                PageNumber = lazyLoadRequestDto.PageOffset                
            };
            var resultDto = paginatedProductDto;
            if (resultDto.Products.Count() > 0) {
                return Ok(resultDto);
            }
            return NoContent();
        }

        [HttpGet("SearchProductByName")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductDto>> SearchProductByName([FromQuery] string searchPhrase)
        {
            if (string.IsNullOrWhiteSpace(searchPhrase)) { 
                throw new ArgumentNullException(nameof(searchPhrase));
            }
            var serviceResult = await _service.SearchProductByName(searchPhrase);
            var resultDto = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(serviceResult.Data);
            if (resultDto.Count() > 0) {
                return Ok(resultDto);
            }
            return NoContent();
        }

        [HttpGet("GetAllProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            var serviceResult = _service.GetAll();
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error); 
                //will create http response error with error message(the oen pass in NotFound) in angular
                //https://angular.io/api/common/http/HttpErrorResponse#description
            }
            //var resultDto = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(serviceResult.Data);
            var resultDto = serviceResult.Data;
            if (resultDto.Count()  > 0)
            {
                return Ok(resultDto);
            }
            return NoContent();
        }        

        [HttpPost("UpdateProductSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductDto>> UpdateProductSubCatalog([FromHeader] Guid productId, Guid newSubCatalogId, string newSubCatalogName) 
        {
            var serviceResult = await _service.UpdateSubCatalog(productId, newSubCatalogId, newSubCatalogName);
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
            var serviceResult = await _service.GetAllProductModels(productId);
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
            var serviceResult = await _service.GetProductModel(productId, pModelId);
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
            var serviceResult = await _service.AddNewProductModel(productId, newProductModel);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<ProductModel, ProductModelDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPost("UpdatePriceProductModel")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProductModelDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProductModelDto>> UpdatePriceProductModel([FromBody] ProductModelUpdatePriceRequestDto productModelUpdatePriceRequestDto)
        {
            var serviceResult = await _service.UpdateProductModelPrice(
                    productId: productModelUpdatePriceRequestDto.ProductId,
                    productModelId: productModelUpdatePriceRequestDto.ProductModelId,
                    newPrice: productModelUpdatePriceRequestDto.NewPrice);

            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<ProductModel, ProductModelDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/UpdateProductToOnSale")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<ProductDto>> UpdateProductToOnSale([FromBody] ProductUpdateToSaleRequestDto productUpdateToSaleRequestDto)
        {
            var serviceResult = await _service.UpdateProductToOnSale(productUpdateToSaleRequestDto.ProductId,
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

        [HttpPost("BackChannel/AddProduct")]
        //TODO how to auth jwt in backchannel
        //[Authorize(
        //    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = PolicyNames.AdminPolicy,
        //    Roles = RoleType.Admin
        //   )
        //]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<ProductDto>> AddProduct([FromBody] ProductDto newProductDto)
        {
            var newProduct = _mapper.Map<ProductDto,Product>(newProductDto);
            if (newProduct == null) {
                throw new InvalidCastException("please review this");
            }
            var serviceResult = await _service.AddProduct(newProduct);
            var response = (serviceResult.IsSuccess == true) ?
                                         BackChannelResponseDto<ProductDto>.Success(_mapper.Map<Product, ProductDto>(serviceResult.Data)) :
                                         BackChannelResponseDto<ProductDto>.Failure(serviceResult.Error);
            return response;
        }

        [HttpPost("BackChannel/GetProductModelInfosOfProvider")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProvider([FromBody] IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoReqMetas)
        {
            var serviceResult = await _service.GetProductModelInfosOfProvider(productModelInfoReqMetas);
            var response = (serviceResult.IsSuccess == true) ?
                                         BackChannelResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Success((serviceResult.Data)) :
                                         BackChannelResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure(serviceResult.Error);
            return response;
        }
    }
}
