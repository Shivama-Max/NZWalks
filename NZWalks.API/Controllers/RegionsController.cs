using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this._dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // GET ALL : https://localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var regions = new List<Region>
            //{
            //    new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Wellington Region",
            //        Code = "WLG",
            //        RegionImageUrl = "https://media.istockphoto.com/id/464378599/photo/wellington-bay-and-harbour.jpg?s=612x612&w=0&k=20&c=XDTrWiAHhiltcbG2cd6e9j78UxWTWiKjFB0vOnZmfA0="
            //    },
            //    new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Auckland Region",
            //        Code = "AKL",
            //        RegionImageUrl = "https://cdn.britannica.com/99/61399-050-B867F67F/skyline-Auckland-New-Zealand-Westhaven-Marina.jpg"
            //    }
            //};

            //Get Data From Database
            //var regionsDomain = await _dbContext.Regions.ToListAsync();

            //We will now use repository pattern
            var regionsDomain = await regionRepository.GetAllAsync();

            //Map Domain Models to DTOs
            //var regionsDTO = new List<RegionDTO>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDTO.Add(new RegionDTO()
            //    {
            //        Id = regionDomain.Id,
            //        Code = regionDomain.Code,
            //        Name = regionDomain.Name,
            //        RegionImageUrl = regionDomain.RegionImageUrl,
            //    });
            //}

            //Using Mapper
            var regionsDTO = mapper.Map<List<RegionDTO>>(regionsDomain);


            //Return DTOs
            return Ok(regionsDTO);
        }

        // GET BY ID : https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id) {
            //var region = _dbContext.Regions.Find(id);  //Works only with primary key (here, id)
            //Get Region Domain Model from DB
            //var regionDomain = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            //We will now use repository pattern
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            //Map Domain Model To DTO
            //var regionDTO = new RegionDTO
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl,
            //};

            //Using Mapper
            var regionDTO = mapper.Map<RegionDTO>(regionDomain);

            //Return DTO back to Client
            return Ok(regionDTO);

        }

        // POST : https://localhost:portnumber/api/regions
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionDTO addRegionDTO)
        {
            //Map DTO to Domain
            //var regionDomain = new Region
            //{
            //    Code = addRegionDTO.Code,
            //    Name = addRegionDTO.Name,
            //    RegionImageUrl = addRegionDTO.RegionImageUrl,
            //};

            //Using Mapper
            var regionDomain = mapper.Map<Region>(addRegionDTO);

            //Use Domain Model to create region
            //await _dbContext.Regions.AddAsync(regionDomain);
            //await _dbContext.SaveChangesAsync();

            //Now, using repository pattern...
            regionDomain = await regionRepository.CreateAsync(regionDomain);


            //Map Domain model back to DTO
            //var regionDTO = new RegionDTO
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl,
            //};

            //Using AutoMapper
            var regionDTO = mapper.Map<RegionDTO>(regionDomain);

            return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);

        }

        //PUT : https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute]Guid id,[FromBody] UpdateRegionDTO updateRegionDTO)
        {
            //Map DTO to Domain Model
            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionDTO.Code,
            //    Name = updateRegionDTO.Name,
            //    RegionImageUrl = updateRegionDTO.RegionImageUrl,
            //};

            //Using AutoMapper
            var regionDomainModel = mapper.Map<Region>(updateRegionDTO);

            //var regionDomainModel= await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            //Now, using repository pattern...
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            //Check if region exists
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //Map DTO to the Domain Model
            //regionDomainModel.Code = updateRegionDTO.Code;
            //regionDomainModel.Name = updateRegionDTO.Name;
            //regionDomainModel.RegionImageUrl = updateRegionDTO.RegionImageUrl;

            //await _dbContext.SaveChangesAsync();

            //Convert DomainModel to DTO
            //var regionDTO = new RegionDTO
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //};

            //Using AutoMapper
            var regionDTO = mapper.Map<RegionDTO>(regionDomainModel);

            return Ok(regionDTO);
            
            
        }

        //DELETE : https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:guid}")]

        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            //var regionDomainModel = await _dbContext.Regions.FirstOrDefaultAsync(x=>x.Id == id);

            //Now, using repository pattern...
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            //Check if exists
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //_dbContext.Regions.Remove(regionDomainModel);
            //await _dbContext.SaveChangesAsync();

            //Map DomainModel to DTO
            //var regionDTO = new RegionDTO
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //};

            //Using AutoMapper
            var regionDTO = mapper.Map<RegionDTO>(regionDomainModel);


            return Ok(regionDTO);
        }

    }
}
