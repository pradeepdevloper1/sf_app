using XF.APP.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface ILookupVMService : IBaseService
    {
        Task<IEnumerable<LookupKeyDto>> GetAllAsync();
        Task<LookupKeyDto> SaveUpdateAsync(LookupKeyDto modelDTO);
        Task<LookupKeyDto> GetByIdAsync(long Id);
        Task<LookupKeyDto> DeleteAsync(long Id);
    }
}
