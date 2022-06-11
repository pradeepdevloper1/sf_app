using System.Collections.Generic;
using System.Threading.Tasks;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IPurchaseOrderRepository : IBaseRepository
    {
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync();
        Task<IEnumerable<PurchaseOrderDto>> GetByIdPoAsync(string soNo, string poNo);
        Task<PurchaseOrderDto> GetByIdPoAsync(string soNo, string poNo, string color, string hexcode, string shift);
        Task<IEnumerable<PurchaseOrderDto>> SaveUpdateAsync(IEnumerable<PurchaseOrderDto> modelDTO, IEnumerable<Shift> shifts);
        Task<PurchaseOrderDto> SaveUpdateAsync(PurchaseOrderDto modelDTO);
        Task<PurchaseOrderDto> GetByIdAsync(long Id);
        Task<PurchaseOrderDto> DeleteAsync(long Id);
        Task DeleteAllRecords();
    }
}
