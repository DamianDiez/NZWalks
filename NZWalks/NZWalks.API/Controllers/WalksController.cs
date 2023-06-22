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
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, 
            IMapper mapper, 
            IRegionRepository regionRepository,
            IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWalks()
        {
            var walks = await walkRepository.GetAllAsync();
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            var walk = await walkRepository.GetAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            var walkDTO = mapper.Map<Models.DTO.Walk>(walk);
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync(Models.DTO.AddWalkRequest walkDTO)
        {

            if (!await ValidateAddWalkAsync(walkDTO))
            {
                return BadRequest(ModelState);
            }

            var walk = new Models.Domain.Walk()
            {
                Name = walkDTO.Name,
                Length = walkDTO.Length,
                RegionId = walkDTO.RegionId,
                WalkDifficultyId = walkDTO.WalkDifficultyId,
            };

            var r = await walkRepository.AddAsync(walk);

            var response = mapper.Map<Models.DTO.Walk>(r);
            return CreatedAtAction(nameof(GetWalkAsync), new { id = response.Id }, response);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            var walk = await walkRepository.DeleteAsync(id);
            if (walk == null)
            {
                NotFound();
            }
            var walkDTO = mapper.Map<Models.DTO.Walk>(walk);
            return Ok(walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, 
                        [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {

            if (!await ValidateUpdateWalkAsync(updateWalkRequest))
            {
                return BadRequest(ModelState);
            }
            var walk = new Models.Domain.Walk()
            {
                Name = updateWalkRequest.Name,
                Length = updateWalkRequest.Length,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId,
            };
            walk = await walkRepository.UpdateAsync(id, walk);
            if (walk == null)
            {
                return NotFound();
            }
            var walkDTO = mapper.Map<Models.DTO.Walk>(walk);
            return Ok(walkDTO);
        }

        #region private methods

        private async Task<bool> ValidateAddWalkAsync(AddWalkRequest walkDTO)
        {
            if (walkDTO == null)
            {
                ModelState.AddModelError(nameof(walkDTO),
                    $"{nameof(walkDTO)} cannot be empty.");
                return false;
            }
            if(string.IsNullOrWhiteSpace(walkDTO.Name))
            {
                ModelState.AddModelError(nameof(walkDTO.Name),
                    $"{nameof(walkDTO.Name)} is required.");
            }
            if (walkDTO.Length <= 0)
            {
                ModelState.AddModelError(nameof(walkDTO.Length),
                    $"{nameof(walkDTO.Length)} should be greater than zero.");
            }
            var region = await regionRepository.GetAsync(walkDTO.RegionId);
            if(region == null)
            {
                ModelState.AddModelError(nameof(walkDTO.RegionId),
                    $"{nameof(walkDTO.RegionId)} is invalid.");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(walkDTO.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(walkDTO.WalkDifficultyId),
                    $"{nameof(walkDTO.WalkDifficultyId)} is invalid.");
            }

            if(ModelState.ErrorCount > 0){
                return false;}
            return true;
        }

        private async Task<bool> ValidateUpdateWalkAsync(UpdateWalkRequest walkDTO)
        {
            if (walkDTO == null)
            {
                ModelState.AddModelError(nameof(walkDTO),
                    $"{nameof(walkDTO)} cannot be empty.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(walkDTO.Name))
            {
                ModelState.AddModelError(nameof(walkDTO.Name),
                    $"{nameof(walkDTO.Name)} is required.");
            }
            if (walkDTO.Length <= 0)
            {
                ModelState.AddModelError(nameof(walkDTO.Length),
                    $"{nameof(walkDTO.Length)} should be greater than zero.");
            }
            var region = await regionRepository.GetAsync(walkDTO.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(walkDTO.RegionId),
                    $"{nameof(walkDTO.RegionId)} is invalid.");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(walkDTO.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(walkDTO.WalkDifficultyId),
                    $"{nameof(walkDTO.WalkDifficultyId)} is invalid.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
    
