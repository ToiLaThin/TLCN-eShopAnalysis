using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
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
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Controllers
{
    [Route("api/ProductCatalogAPI/CatalogAPI")]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CatalogDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CatalogDto>>> GetAll()
        {
            var serviceResult = _service.GetAll();
            var resultDto = _mapper.Map<IEnumerable<Catalog>, IEnumerable<CatalogDto>>(serviceResult.Data);
            if (resultDto?.Count() > 0) {
                return Ok(resultDto);
            }
            return NotFound();
        }


        [HttpGet("GetOneCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CatalogDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CatalogDto>> GetOne([FromHeader] Guid catalogId)
        {
            var serviceResult = _service.Get(catalogId);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Catalog, CatalogDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPost("CreateCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CatalogDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CatalogDto>> CreateCatalog([FromBody] Catalog newCatalog)
        {
            var serviceResult = _service.AddCatalog(newCatalog);
            if (serviceResult.IsFailed || serviceResult.IsException) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<CatalogDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPut("UpdateCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CatalogDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CatalogDto>> UpdateCatalog([FromBody] Catalog updateCatalog)
        {
            var serviceResult = _service.UpdateCatalog(updateCatalog);
            if (serviceResult.IsFailed || serviceResult.IsException)
            {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<CatalogDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpDelete("DeleteCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> DeleteCatalog([FromBody] Guid deleteCatalogId)
        {
            var serviceResult = _service.DeleteCatalog(deleteCatalogId);
            if (serviceResult == false) {
                return NotFound("not deleted success");
            }
            return Ok("deleted");
        }
        #endregion


        #region SubCatalog Api
        [HttpGet("GetAllSubCatalogs")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<SubCatalogDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<SubCatalogDto>>> GetAllSubCatalogs([FromHeader] Guid catalogId)
        {
            var serviceResult = _service.GetAllSubCatalogs(catalogId);
            if (serviceResult.IsException || serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<SubCatalogDto>>(serviceResult.Data);
            if (resultDto is null || resultDto.Count() == 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpGet("GetOneSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SubCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SubCatalogDto>> GetOneSubCatalog([FromHeader] Guid catalogId, [FromHeader] Guid subCatalogId)
        {
            var serviceResult = _service.GetSubCatalog(catalogId, subCatalogId);
            if (serviceResult.IsFailed || serviceResult.IsException) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<SubCatalogDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("CreateSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> CreateSubCatalog([FromBody] SubCatalog newSubCatalog, [FromHeader] Guid catalogId)
        {
            var serviceResult = _service.AddNewSubCatalog(catalogId, newSubCatalog);
            if (serviceResult == false) {
                return NotFound("cannot add subcatalog");
            }
            return Ok("catalog added");
        }

        [HttpDelete("DeleteSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SubCatalogDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<SubCatalogDto>> DeleteSubCatalog([FromHeader] Guid catalogId, [FromHeader] Guid subCatalogId)
        {
            var serviceResult = _service.DeleteSubCatalog(catalogId, subCatalogId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<SubCatalogDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("UpdateSubCatalog")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SubCatalogDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<SubCatalogDto>> UpdateSubCatalog([FromHeader] Guid catalogId, [FromBody] SubCatalog newSubCatalog)
        {
            var serviceResult = _service.UpdateSubCatalog(catalogId, newSubCatalog);
            if (serviceResult.IsFailed)
            {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<SubCatalogDto>(serviceResult.Data);
            return Ok(resultDto);
        }
        #endregion

    }
}
