using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using System.Runtime.InteropServices;

namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDBContext nZWalksDBContext;
        public RegionRepository(NZWalksDBContext nZWalksDBContext)
        {
            this.nZWalksDBContext = nZWalksDBContext;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await nZWalksDBContext.Regions.ToListAsync();
        }

        public async Task<Region> GetAsync(Guid id)
        {
            return await nZWalksDBContext.Regions
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Region> AddAsync(Region region)
        {
            region.Id = Guid.NewGuid();
            await nZWalksDBContext.AddAsync(region);
            await nZWalksDBContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region> DeleteAsync(Guid id)
        {
            var region = await nZWalksDBContext.Regions
                .FirstOrDefaultAsync(x => x.Id == id);

            if(region == null)
            {
                return null;
            }

            nZWalksDBContext.Regions.Remove(region);
            await nZWalksDBContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region> UpdateAsync(Guid id, Region region)
        {
            var regionDB = await nZWalksDBContext.Regions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (regionDB == null)
            {
                return null;
            }
            regionDB.Code = region.Code;
            regionDB.Area = region.Area;
            regionDB.Lat = region.Lat;
            regionDB.Long = region.Long;
            regionDB.Name = region.Name;
            regionDB.Population = region.Population;

            nZWalksDBContext.Regions.Update(regionDB);
            await nZWalksDBContext.SaveChangesAsync();
            return region;
        }
    }
}
