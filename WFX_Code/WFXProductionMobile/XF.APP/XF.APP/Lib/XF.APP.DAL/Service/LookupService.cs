using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.DAL
{
    public class LookupService : BaseService, ILookupService
    {
        readonly ILookupKeyRepository lookupKeyRepository;
        public LookupService(ILookupKeyRepository lookupKeyRepository)
        {
            this.lookupKeyRepository = lookupKeyRepository;
        }

        public Task<LookupKeyDto> DeleteAsync(long Id)
        {
            return this.lookupKeyRepository.DeleteAsync(Id);
        }

        public Task<IEnumerable<LookupKeyDto>> GetAllAsync()
        {
            return this.lookupKeyRepository.GetAllAsync();
        }

        public Task<LookupKeyDto> GetByIdAsync(long Id)
        {
            return this.lookupKeyRepository.GetByIdAsync(Id);
        }

        public Task<LookupKeyDto> SaveUpdateAsync(LookupKeyDto modelDTO)
        {
            return this.lookupKeyRepository.SaveUpdateAsync(modelDTO);
        }
    }
}
