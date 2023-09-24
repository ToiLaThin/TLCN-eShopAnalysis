using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Application.Services;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using eShopAnalysis.ProductCatalogAPI.Utilities.Behaviors;

namespace eShopAnalysis.ProductCatalogAPI.Controllers
{
    [Route("api/ProductCatalog/CatalogAPI")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _service;
        private readonly IMapper _mapper;
        public CatalogController(ICatalogService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        #region Catalog Api
        [HttpGet("GetAllCatalog")]
        //not use as attribute but to get the service from DI container
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public ResponseDto<string> GetAll()
        {
            var result = _service.GetAll();
            //IEnumerable<CatalogDto> resultDto = _mapper.Map<IEnumerable<CatalogDto>>(result);
            //if (resultDto != null)
            //{
            //    return resultDto;

            //}
            //return new List<CatalogDto> { };
            if (result.IsSuccess)
            {
                IEnumerable<CatalogDto> resultDto = _mapper.Map <IEnumerable<CatalogDto>>(result.Data);
                string jsonResult = JsonConvert.SerializeObject(resultDto);
                return ResponseDto<string>.Success(jsonResult);
            }
            else 
                return ResponseDto<string>.Failure("No catalog");
        }

        [HttpGet("GetOneCatalog")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public CatalogDto GetOne([FromHeader] Guid catalogId)
        {
            var result = _service.Get(catalogId);
            CatalogDto resultDto = _mapper.Map<CatalogDto>(result);
            if (resultDto != null)
            {
                return resultDto;

            }
            return null;
        }

        [HttpPost("CreateCatalog")]
        public CatalogDto CreateCatalog([FromBody] Catalog newCatalog)
        {
            var result = _service.AddCatalog(newCatalog);
            var resultDto = _mapper.Map<CatalogDto>(result);
            if (resultDto != null)
            {
                return resultDto;
            }
            return null;
        }

        [HttpPut("UpdateCatalog")]
        public CatalogDto UpdateCatalog([FromBody] Catalog updateCatalog)
        {
            var result = _service.UpdateCatalog(updateCatalog);
            var resultDto = _mapper.Map<CatalogDto>(result);
            if (resultDto != null)
            {
                return resultDto;
            }
            return null;
        }

        [HttpDelete("DeleteCatalog")]
        public string DeleteCatalog([FromBody] Guid deleteCatalogId)
        {
            var result = _service.DeleteCatalog(deleteCatalogId);
            if (result == true)
            {
                return "deleted";
            }
            return "not deleted success";
        }
        #endregion


        #region SubCatalog Api
        [HttpGet("GetAllSubCatalogs")]
        public IEnumerable<SubCatalogDto> GetAllSubCatalogs([FromHeader] Guid catalogId)
        {
            var result = _service.GetAllSubCatalogs(catalogId);
            var resultDto = _mapper.Map<IEnumerable<SubCatalogDto>>(result);
            if (resultDto is not null)
            {
                return resultDto;
            }
            return null;
        }

        [HttpGet("GetOneSubCatalog")]
        public SubCatalogDto GetOneSubCatalog([FromHeader] Guid catalogId, [FromHeader] Guid subCatalogId)
        {
            var result = _service.GetSubCatalog(catalogId, subCatalogId);
            var resultDto = _mapper.Map<SubCatalogDto>(result);
            if (resultDto is not null)
            {
                return resultDto;
            }
            return null;
        }

        [HttpPost("CreateSubCatalog")]
        public bool CreateSubCatalog([FromBody] SubCatalog newSubCatalog, [FromHeader] Guid catalogId)
        {
            var result = _service.AddNewSubCatalog(catalogId, newSubCatalog);
            return result;
        }

        [HttpDelete("DeleteSubCatalog")]
        public SubCatalogDto DeleteSubCatalog([FromHeader] Guid catalogId, [FromHeader] Guid subCatalogId)
        {
            var result = _service.DeleteSubCatalog(catalogId, subCatalogId);
            var resultDto = _mapper.Map<SubCatalogDto>(result);
            if (resultDto != null)
            {
                return resultDto;
            }
            return null;
        }

        [HttpPost("UpdateSubCatalog")]
        public SubCatalogDto UpdateSubCatalog([FromHeader] Guid catalogId, [FromBody] SubCatalog newSubCatalog)
        {
            var result = _service.UpdateSubCatalog(catalogId, newSubCatalog);
            var resultDto = _mapper.Map<SubCatalogDto>(result);
            if (resultDto != null)
            {
                return resultDto;
            }
            return null;
        }
        #endregion

    }
}
