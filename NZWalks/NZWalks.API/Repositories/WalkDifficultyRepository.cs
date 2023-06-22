using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using System.Runtime.InteropServices;

namespace NZWalks.API.Repositories
{
    public class WalkDifficultyRepository : IWalkDifficultyRepository
    {
        private readonly NZWalksDBContext nZWalksDBContext;
        public WalkDifficultyRepository(NZWalksDBContext nZWalksDBContext)
        {
            this.nZWalksDBContext = nZWalksDBContext;
        }

        public async Task<IEnumerable<WalkDifficulty>> GetAllAsync()
        {
            return await nZWalksDBContext
                .WalkDifficulties
                .ToListAsync();
        }

        public async Task<WalkDifficulty> GetAsync(Guid id)
        {
            return await nZWalksDBContext.WalkDifficulties
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<WalkDifficulty> AddAsync(WalkDifficulty walkDifficulty)
        {
            walkDifficulty.Id = Guid.NewGuid();
            await nZWalksDBContext.AddAsync(walkDifficulty);
            await nZWalksDBContext.SaveChangesAsync();
            return walkDifficulty;
        }

        public async Task<WalkDifficulty> DeleteAsync(Guid id)
        {
            var walkDifficulty = await nZWalksDBContext.WalkDifficulties
                .FirstOrDefaultAsync(x => x.Id == id);

            if(walkDifficulty == null)
            {
                return null;
            }

            nZWalksDBContext.WalkDifficulties.Remove(walkDifficulty);
            await nZWalksDBContext.SaveChangesAsync();
            return walkDifficulty;
        }

        public async Task<WalkDifficulty> UpdateAsync(Guid id, WalkDifficulty walkDifficulty)
        {
            var walkDifficultyDB = await nZWalksDBContext.WalkDifficulties
                .FirstOrDefaultAsync(x => x.Id == id);

            if (walkDifficultyDB == null)
            {
                return null;
            }
            walkDifficultyDB.Code= walkDifficulty.Code;
            
            nZWalksDBContext.WalkDifficulties.Update(walkDifficultyDB);
            await nZWalksDBContext.SaveChangesAsync();
            return walkDifficultyDB;
        }
    }
}
