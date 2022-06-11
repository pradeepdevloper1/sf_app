using AutoMapper;
using XF.APP.ABSTRACTION;
using XF.APP.DATA;
using XF.APP.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace XF.APP.DAL
{
    public class PurchaseOrderRepository : BaseRepository, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(IMapper mapper) : base(mapper)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
        }

        public async Task DeleteAllRecords()
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await _dbContext.PurchaseOrder.ToListAsync();
            _dbContext.PurchaseOrder.RemoveRange(modelList);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PurchaseOrderDto> DeleteAsync(long Id)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.PurchaseOrder.FindAsync(Id);
            _dbContext.Entry(model).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            var modelDTO = mapper.Map<PurchaseOrder, PurchaseOrderDto>(model);
            return modelDTO;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync()
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await _dbContext.PurchaseOrder.ToListAsync();
            var modelDTOList = mapper.Map<IEnumerable<PurchaseOrder>, IEnumerable<PurchaseOrderDto>>(modelList);
            return modelDTOList;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetByIdPoAsync(string soNo, string poNo)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.PurchaseOrder.Where(p => p.SoNo == soNo && p.PoNo ==  poNo).ToListAsync();
            var modelDTO = mapper.Map<IEnumerable<PurchaseOrder>, IEnumerable<PurchaseOrderDto>>(model);
            return modelDTO;
        }

        public async Task<PurchaseOrderDto> GetByIdPoAsync(string soNo, string poNo, string color, string hexcode, string shift)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.PurchaseOrder.Where(p => p.SoNo == soNo && p.PoNo == poNo && p.Color == color).FirstOrDefaultAsync();
            var modelDTO = mapper.Map<PurchaseOrder, PurchaseOrderDto>(model);
            return modelDTO;
        }

        public async Task<PurchaseOrderDto> GetByIdAsync(long Id)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var model = await _dbContext.PurchaseOrder.FindAsync(Id);
            var modelDTO = mapper.Map<PurchaseOrder, PurchaseOrderDto>(model);
            return modelDTO;
        }

        public async Task<PurchaseOrderDto> SaveUpdateAsync(PurchaseOrderDto modelDTO)
        {
            _dbContext = new ApplicationContext(Constants.DbPath);
            var existing = _dbContext.PurchaseOrder.Where(p => p.EntryDate == modelDTO.EntryDate && p.OrderID == modelDTO.OrderID &&
                p.Shift == modelDTO.Shift).First();
            if (existing != null)
                _dbContext.PurchaseOrder.Remove(existing);

            var model = mapper.Map<PurchaseOrderDto, PurchaseOrder>(modelDTO);
            await _dbContext.PurchaseOrder.AddAsync(model);

            await _dbContext.SaveChangesAsync();
            modelDTO = mapper.Map<PurchaseOrder, PurchaseOrderDto>(model);
            return modelDTO;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> SaveUpdateAsync(IEnumerable<PurchaseOrderDto> modelDTO, IEnumerable<Shift> shifts)
        {
           
            _dbContext = new ApplicationContext(Constants.DbPath);
            var modelList = await _dbContext.PurchaseOrder.ToListAsync();
            // Save qty related data
            foreach (var po in modelDTO)
            {
                var pointsRecord = new List<PurchaseOrderDto>();              
                {
                    pointsRecord.Add(GetPoObject(po, po.PassQty, po.DefectQty, po.RejectQty, "" , string.Empty));
                }

                if (modelList.Count > 0)
                {
                    var modellist2 = modelList.Where(x => x.FactoryID == po.FactoryID && x.SoNo == po.SoNo && x.PoNo == po.PoNo && x.Color == po.Color).ToList();

                    if (modellist2.Count() > 1) {
                        _dbContext.PurchaseOrder.RemoveRange(modellist2);
                        var model = mapper.Map<IEnumerable<PurchaseOrderDto>, IEnumerable<PurchaseOrder>>(pointsRecord);
                        await _dbContext.PurchaseOrder.AddRangeAsync(model);
                    }
                    if (modellist2.Count > 0)
                    {
                        modellist2[0].PassQty = po.PassQty;
                        modellist2[0].DefectQty = po.DefectQty;
                        modellist2[0].RejectQty = po.RejectQty;
                        modellist2[0].WFXColorCode = po.WFXColorCode;
                        modellist2[0].WFXColorName = po.WFXColorName;
                        modellist2[0].SizeList = po.SizeList;
                        _dbContext.PurchaseOrder.UpdateRange(modellist2);
                    }
                    else
                    {
                        var model = mapper.Map<IEnumerable<PurchaseOrderDto>, IEnumerable<PurchaseOrder>>(pointsRecord);
                        await _dbContext.PurchaseOrder.AddRangeAsync(model);
                    }
                }
                else
                {
                    var model = mapper.Map<IEnumerable<PurchaseOrderDto>, IEnumerable<PurchaseOrder>>(pointsRecord);
                    await _dbContext.PurchaseOrder.AddRangeAsync(model);
                }
            }
            var model1 = mapper.Map<IEnumerable<PurchaseOrderDto>, IEnumerable<PurchaseOrder>>(modelDTO);
            int rows = await _dbContext.SaveChangesAsync();
            modelDTO = mapper.Map<IEnumerable<PurchaseOrder>, IEnumerable<PurchaseOrderDto>>(model1);
            return modelDTO;
        }

        private PurchaseOrderDto GetPoObject(PurchaseOrderDto po, int passQty, int defectQty, int rejectQty, string shift, string markPoints)
        {
            return new PurchaseOrderDto
            {
                Id = po.Id,
                UserID = po.UserID,
                FactoryID = po.FactoryID,
                OrderID = po.OrderID,
                SoNo = po.SoNo,
                PoNo = po.PoNo,
                PoQty = po.PoQty,
                Part = po.Part,
                Color = po.Color,
                Customer = po.Customer,
                EntryDate = po.EntryDate,
                ExFactory = po.ExFactory,
                Fabric = po.Fabric,
                Fit = po.Fit,
                Hexcode = po.Hexcode,
                IsSizeRun = po.IsSizeRun,
                Module = po.Module,
                OrderRemark = po.OrderRemark,
                OrderStatus = po.OrderStatus,
                PlanStDt = po.PlanStDt,
                PrimaryPart = po.PrimaryPart,
                Product = po.Product,
                Season = po.Season,
                SizeList = po.SizeList,
                Style = po.Style,
                Shift = shift,
                MarkPoints = markPoints,
                DefectQty = defectQty == -1 ? 0 : defectQty,
                PassQty = passQty == -1 ? 0 : passQty,
                RejectQty = rejectQty == -1 ? 0 : rejectQty,
                WFXColorCode = po.WFXColorCode,
                WFXColorName = po.WFXColorName
                //,
                //ProcessCode = po.ProcessCode,
                //ProcessName = po.ProcessName,
                //FulfillmentType = po.FulfillmentType
            };
        }
    }
}
