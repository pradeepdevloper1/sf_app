using AutoMapper;
using XF.APP.ABSTRACTION;
using XF.APP.DATA;
using XF.APP.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.DAL
{
    public class LookupKeyRepository : BaseRepository, ILookupKeyRepository
    {
        public LookupKeyRepository(IMapper mapper) : base(mapper)
        {
            this._dbContext = new ApplicationContext(Constants.DbPath);
        }

        public async Task<LookupKeyDto> DeleteAsync(long Id)
        {
            this._dbContext = new ApplicationContext(Constants.DbPath);
            var model = await this._dbContext.LookupKey.FindAsync(Id);
            model.IsDeleted = true;
            this._dbContext.Entry(model).State = EntityState.Modified;
            await this._dbContext.SaveChangesAsync();
            var modelDTO = this.mapper.Map<LookupKey, LookupKeyDto>(model);
            return modelDTO;
        }

        public async Task<IEnumerable<LookupKeyDto>> GetAllAsync()
        {
            this._dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await this._dbContext.LookupKey.ToListAsync();
            var modelDTOList = this.mapper.Map<IEnumerable<LookupKey>, IEnumerable<LookupKeyDto>>(modelList);
            return modelDTOList;
        }

        public async Task<LookupKeyDto> GetByIdAsync(long Id)
        {
            this._dbContext = new ApplicationContext(Constants.DbPath);
            var model = await this._dbContext.LookupKey.FindAsync(Id);
            var modelDTO = this.mapper.Map<LookupKey, LookupKeyDto>(model);
            return modelDTO;
        } 

        public async Task<LookupKeyDto> SaveUpdateAsync(LookupKeyDto modelDTO)
        {
            this._dbContext = new ApplicationContext(Constants.DbPath);
            var model = this.mapper.Map<LookupKeyDto, LookupKey>(modelDTO);
            if (model.LookupKeyID==0)
            {
                model.IsActive = true;
                await this._dbContext.LookupKey.AddAsync(model);
            }
            else
            {
                this._dbContext.Entry(model).State = EntityState.Modified;
            }
            await this._dbContext.SaveChangesAsync();
            modelDTO = this.mapper.Map<LookupKey, LookupKeyDto>(model);
            return modelDTO;
        }
    }
}
