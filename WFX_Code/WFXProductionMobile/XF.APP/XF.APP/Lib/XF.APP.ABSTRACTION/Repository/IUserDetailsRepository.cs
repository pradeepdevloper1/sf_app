using System.Collections.Generic;
using System.Threading.Tasks;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IUserDetailsRepository : IBaseRepository
    {
        Task DeleteAsync();
        Task<UserDetailsDto> DeleteAsync(long Id);
        Task<IEnumerable<UserDetailsDto>> GetAllAsync();
        Task<UserDetailsDto> GetByIdAsync(long Id);
        Task<UserDetailsDto> SaveUpdateAsync(UserDetailsDto modelDTO);
    }
}
