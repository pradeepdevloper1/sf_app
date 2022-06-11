using AutoMapper;
using XF.APP.ABSTRACTION;
using XF.APP.DATA;
using XF.APP.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.DAL
{
    public class UserDetailsRepository : BaseRepository, IUserDetailsRepository
    {
        public UserDetailsRepository(IMapper mapper) : base(mapper)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
        }

        public async Task<UserDetailsDto> DeleteAsync(long Id)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.UserDetails.FindAsync(Id);
            _dbContext.Entry(model).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
            var modelDTO = mapper.Map<UserDetails, UserDetailsDto>(model);
            return modelDTO;
        }

        public async Task DeleteAsync()
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await _dbContext.UserDetails.ToListAsync();
            _dbContext.UserDetails.RemoveRange(modelList);
            _dbContext.Entry(modelList).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllAsync()
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await _dbContext.UserDetails.ToListAsync();
            var modelDTOList = mapper.Map<IEnumerable<UserDetails>, IEnumerable<UserDetailsDto>>(modelList);
            return modelDTOList;
        }

        public async Task<UserDetailsDto> GetByIdAsync(long Id)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.UserDetails.FindAsync(Id);
            var modelDTO = mapper.Map<UserDetails, UserDetailsDto>(model);
            return modelDTO;
        }

        public async Task<UserDetailsDto> SaveUpdateAsync(UserDetailsDto modelDTO)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = mapper.Map<UserDetailsDto, UserDetails>(modelDTO);
            if (model.Id == 0)
            {
                //model.IsActive = true;
                await _dbContext.UserDetails.AddAsync(model);
            }
            else
            {
                _dbContext.Entry(model).State = EntityState.Modified;
            }
            await _dbContext.SaveChangesAsync();
            modelDTO = mapper.Map<UserDetails, UserDetailsDto>(model);
            return modelDTO;
        }
    }
}
