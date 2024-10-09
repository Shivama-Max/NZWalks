using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        public readonly NZWalksDbContext dbContext;
        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<List<Walk>> GetAllAsync()
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var prevWalk = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x=>x.Id == id);
            if (prevWalk == null) {
                return null;
            }
            prevWalk.WalkImageUrl = walk.WalkImageUrl;
            prevWalk.Name = walk.Name;
            prevWalk.Description = walk.Description;
            prevWalk.LengthInKm = walk.LengthInKm;
            prevWalk.DifficultyId = walk.DifficultyId;
            prevWalk.RegionId = walk.RegionId;

            await dbContext.SaveChangesAsync();
            return prevWalk;

        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var prevWalk = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if (prevWalk == null) {
                return null;
            }
            dbContext.Walks.Remove(prevWalk);
            await dbContext.SaveChangesAsync();
            return prevWalk;
        }
    }
}
