using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NZWalks.API.Data;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Diagnostics.Tracing;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionsController : Controller
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await regionRepository.GetAllAsync();
            var regionsDTO = mapper.Map<List<Models.DTO.Region>>(regions);
            return Ok(regionsDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetRegionAsync")]
        public async Task<IActionResult> GetRegionAsync(Guid id)
        {
            var region = await regionRepository.GetAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            var regionDTO = mapper.Map<Models.DTO.Region>(region);
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegionAsync(Models.DTO.AddRegionRequest regionDTO)
        {
            //if (!ValidateAddRegionAsync(regionDTO))
            //{
            //    return BadRequest(ModelState);
            //}

            var region = new Models.Domain.Region()
            {
                Code = regionDTO.Code,
                Area = regionDTO.Area,
                Lat = regionDTO.Lat,
                Long = regionDTO.Long,
                Name = regionDTO.Name,
                Population = regionDTO.Population,
            };

            var r = await regionRepository.AddAsync(region);

            var response = mapper.Map<Models.DTO.Region>(r);
            return CreatedAtAction(nameof(GetRegionAsync), new { id = response.Id }, response);
        }

        
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegionAsync(Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if (region == null)
            {
                NotFound();
            }
            var regionDTO = mapper.Map<Models.DTO.Region>(region);
            return Ok(regionDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegionAsync([FromRoute] Guid id, 
                        [FromBody] Models.DTO.UpdateRegionRequest updateRegionRequest)
        {

            //if (!ValidateUpdateRegionAsync(updateRegionRequest))
            //{
            //    return BadRequest(ModelState);
            //}

            var region = new Models.Domain.Region()
            {
                Code = updateRegionRequest.Code,
                Area = updateRegionRequest.Area,
                Lat = updateRegionRequest.Lat,
                Long = updateRegionRequest.Long,
                Name = updateRegionRequest.Name,
                Population = updateRegionRequest.Population,
            };
            region = await regionRepository.UpdateAsync(id, region);
            if (region == null)
            {
                return NotFound();
            }
            var regionDTO = mapper.Map<Models.DTO.Region>(region);
            return Ok(regionDTO);
        }

        #region private methods

        private bool ValidateAddRegionAsync(AddRegionRequest regionDTO)
        {
            if(regionDTO == null){
                ModelState.AddModelError(nameof(regionDTO),"Add Region Data required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(regionDTO.Code)){
                ModelState.AddModelError(nameof(regionDTO.Code), 
                    $"{nameof(regionDTO.Code)} cannot be null or white space.");
            }
            if (string.IsNullOrWhiteSpace(regionDTO.Name)){
                ModelState.AddModelError(nameof(regionDTO.Name), 
                    $"{nameof(regionDTO.Name)} cannot be null or white space.");
            }
            if (regionDTO.Area <= 0){
                ModelState.AddModelError(nameof(regionDTO.Area), 
                    $"{nameof(regionDTO.Area)} cannot be less than or equal to zero.");
            }
            if (regionDTO.Population < 0){
                ModelState.AddModelError(nameof(regionDTO.Population), 
                    $"{nameof(regionDTO.Population)} cannot be less than zero.");
            }
            if(ModelState.ErrorCount > 0)
                return false;
            return true;
        }

        private bool ValidateUpdateRegionAsync(UpdateRegionRequest regionDTO)
        {
            if (regionDTO == null)
            {
                ModelState.AddModelError(nameof(regionDTO), "Add Region Data required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(regionDTO.Code))
            {
                ModelState.AddModelError(nameof(regionDTO.Code),
                    $"{nameof(regionDTO.Code)} cannot be null or white space.");
            }
            if (string.IsNullOrWhiteSpace(regionDTO.Name))
            {
                ModelState.AddModelError(nameof(regionDTO.Name),
                    $"{nameof(regionDTO.Name)} cannot be null or white space.");
            }
            if (regionDTO.Area <= 0)
            {
                ModelState.AddModelError(nameof(regionDTO.Area),
                    $"{nameof(regionDTO.Area)} cannot be less than or equal to zero.");
            }
            if (regionDTO.Population < 0)
            {
                ModelState.AddModelError(nameof(regionDTO.Population),
                    $"{nameof(regionDTO.Population)} cannot be less than zero.");
            }
            if (ModelState.ErrorCount > 0)
                return false;
            return true;
        }

        #endregion
    }
}
    
