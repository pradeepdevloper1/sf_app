//using XF.APP.Abstraction;
//using XF.APP.DTO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace XF.APP.BAL
//{
//    public class ClientViewModel : BaseViewModel,IClientViewModel
//    {
//        private readonly IClientService dataservice;

//        public int count = 0;
//        public ClientViewModel()
//        {
//            try
//            {
//                this.dataservice = ServiceLocator.Resolve<IClientService>();
//                //PerformSampleDbOperation();
//            }
//            catch (System.Exception)
//            {
//            } 
//        }
//        public async void PerformSampleDbOperation()
//        {
//            var allRecords = await this.dataservice.GetAllAsync();
//            var insertedRecords = await this.dataservice.SaveUpdateAsync(new ClientMasterDto()
//            {
//                ClientName = "Tushar"
//            });
//            allRecords = await this.dataservice.GetAllAsync();
//            var firstRecord = await this.dataservice.GetByIdAsync(allRecords.ToList().First().ClientMasterID);
//            firstRecord.ClientName = firstRecord.ClientName + " Updated";
//            var deleteFirstRecord = await this.dataservice.DeleteAsync(firstRecord.ClientMasterID);
//            allRecords = await this.dataservice.GetAllAsync();
//            string a = string.Empty;
//        } 
//    }
//}
