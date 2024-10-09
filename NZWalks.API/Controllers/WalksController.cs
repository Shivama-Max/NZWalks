using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Data.Common;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        //CREATE WALKS
        //POST
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkDTO addWalkDTO)
        {
            //Map DTO to Domain Model using AutoMapper
            var walkDomainModel = mapper.Map<Walk>(addWalkDTO);

            walkDomainModel = await walkRepository.CreateAsync(walkDomainModel);

            var walkDTO = mapper.Map<WalkDTO>(walkDomainModel);

            return Ok();

        }

        //READ WALKS
        //GET
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walkDomain = await walkRepository.GetAllAsync();
            return Ok(mapper.Map<List<WalkDTO>>(walkDomain));
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var walkDomain = await walkRepository.GetByIdAsync(id);
            if (walkDomain == null) { 
                return NotFound();
            }
            return Ok(mapper.Map<WalkDTO>(walkDomain));
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute]Guid id,[FromBody]UpdateWalkDTO updateWalkDTO)
        {
                var walkDomain = mapper.Map<Walk>(updateWalkDTO);
                walkDomain = await walkRepository.UpdateAsync(id, walkDomain);
                if (walkDomain == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<WalkDTO>(walkDomain));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.DeleteAsync(id);
            if (walkDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDTO>(walkDomain));
        }
    }
}
