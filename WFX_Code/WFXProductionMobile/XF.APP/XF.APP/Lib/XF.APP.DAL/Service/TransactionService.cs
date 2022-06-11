using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XF.APP.DAL
{
    public class TransactionService : BaseService, ITransactionService
    {
        readonly IPurchaseOrderRepository purchaseOrderRepository;
        public TransactionService(IPurchaseOrderRepository purchaseOrderRepository)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
        }

        public Task DeleteAllRecords()
        {
            return this.purchaseOrderRepository.DeleteAllRecords();
        }

        public Task<PurchaseOrderDto> DeletePoAsync(long Id)
        {
            return this.purchaseOrderRepository.DeleteAsync(Id);
        }

        public Task<IEnumerable<PurchaseOrderDto>> GetAllPoAsync()
        {
            return this.purchaseOrderRepository.GetAllAsync();
        }

        public Task<IEnumerable<PurchaseOrderDto>> GetByIdPoAsync(string soNo, string poNo)
        {
            return this.purchaseOrderRepository.GetByIdPoAsync(soNo, poNo);
        }

        public Task<PurchaseOrderDto> GetByIdPoAsync(string soNo, string poNo, string color, string hexcode, string shift)
        {
            return this.purchaseOrderRepository.GetByIdPoAsync(soNo, poNo, color, hexcode, shift);
        }

        public Task<PurchaseOrderDto> GetByIdPoAsync(long Id)
        {
            return this.purchaseOrderRepository.GetByIdAsync(Id);
        }

        public async Task<IEnumerable<PurchaseOrderDto>> SaveUpdatePoAsync(IEnumerable<PurchaseOrderDto> modelDTO, IEnumerable<Shift> shifts)
        {
            return await this.purchaseOrderRepository.SaveUpdateAsync(modelDTO, shifts);
        }

        public async Task<PurchaseOrderDto> SaveUpdatePoAsync(PurchaseOrderDto modelDTO)
        {
            return await this.purchaseOrderRepository.SaveUpdateAsync(modelDTO);
        }
    }
}
