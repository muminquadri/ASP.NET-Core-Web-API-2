using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using EFCore.CityInfo.Api.Entities;
using EFCore.CityInfo.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EFCore.CityInfo.Api.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace EFCore.CityInfo.Api.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private ICityInfoRepository _cityInfoRepository;
        public PointsOfInterestController(ICityInfoRepository cityInfoRepository,
            ILogger<PointsOfInterestController> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _logger = logger;
        }

        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} not found while accessing points of interest");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var results = Mapper.Map<IEnumerable<Models.PointOfInterestDto>>(pointsOfInterestForCity);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occurred while getting POI's for city with id {cityId}", ex);
                return StatusCode(500, "Error");

            }

        }

        [HttpGet("{cityId}/pointsOfInterest/{id}", Name = "GetPointOfInterest")]

        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} not found while accessing points of interest");
                    return NotFound();
                }
                var GetPointOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
                var result = Mapper.Map<Models.PointOfInterestDto>(GetPointOfInterestForCity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occurred while getting POI for city with id {cityId} & POI id {id}", ex);
                return StatusCode(500, "Error");
            }
        }

        [HttpPost("{cityId}/pointsOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointofinterest)
        {
            if (pointofinterest == null)
            {
                return BadRequest();
            }
            if (pointofinterest.Name == pointofinterest.Description)
            {
                ModelState.AddModelError("Description", "The name & description must not be same");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointofinterest);
            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }
            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                id = finalPointOfInterest.Id
            }, createdPointOfInterestToReturn);
        }

        [HttpPut("{cityId}/pointsOfInterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The name & description must not be same");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            Mapper.Map(pointOfInterest, pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }
            return NoContent();
        }

        [HttpPatch("{cityId}/pointsOfInterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            var pointOfInterestToPatch = Mapper.Map<Models.PointOfInterestForUpdateDto>(pointOfInterestEntity);
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The name & description must not be same");
            }
            TryValidateModel(pointOfInterestToPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }
            return NoContent();
        }

        [HttpDelete("{cityId}/pointsOfInterest/{id}")]

        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _cityInfoRepository.DeletePointOfInterestForCity(pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }
            return NoContent();
        }
    }
}