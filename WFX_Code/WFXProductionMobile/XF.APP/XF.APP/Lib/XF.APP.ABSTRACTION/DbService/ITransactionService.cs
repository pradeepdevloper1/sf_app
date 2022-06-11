using XF.APP.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface ITransactionService : IBaseService
    {
        Task<IEnumerable<PurchaseOrderDto>> GetAllPoAsync();
        Task<IEnumerable<PurchaseOrderDto>> GetByIdPoAsync(string soNo, string poNo);
        Task<PurchaseOrderDto> GetByIdPoAsync(string soNo, string poNo, string color, string hexcode, string shift);
        Task<IEnumerable<PurchaseOrderDto>> SaveUpdatePoAsync(IEnumerable<PurchaseOrderDto> modelDTO, IEnumerable<Shift> shifts);
        Task<PurchaseOrderDto> SaveUpdatePoAsync(PurchaseOrderDto modelDTO);
        Task<PurchaseOrderDto> GetByIdPoAsync(long Id);
        Task<PurchaseOrderDto> DeletePoAsync(long Id);
        Task DeleteAllRecords();
    }
}
