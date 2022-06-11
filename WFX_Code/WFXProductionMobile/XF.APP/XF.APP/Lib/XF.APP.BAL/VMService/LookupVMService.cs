using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.BAL
{
    public class LookupVMService : BaseService, ILookupVMService
    {
        ILookupService lookupService;

        public LookupVMService()
        {
            this.lookupService= ServiceLocator.Resolve<ILookupService>();
        }

        public Task<LookupKeyDto> DeleteAsync(long Id)
        {
            return this.lookupService.DeleteAsync(Id);
        }

        public Task<IEnumerable<LookupKeyDto>> GetAllAsync()
        {
            return this.lookupService.GetAllAsync();
        }

        public Task<LookupKeyDto> GetByIdAsync(long Id)
        {
            return this.lookupService.GetByIdAsync(Id);
        }

        public Task<LookupKeyDto> SaveUpdateAsync(LookupKeyDto modelDTO)
        {
            return this.lookupService.SaveUpdateAsync(modelDTO);
        }
    }
}
