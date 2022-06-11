export class UserDashboardModel {
  runningOrders: number;
  completedOrders: number;
  delayedOrders: number;

  compliance: number;
  picesChecked: number;
  rejectionRate: number;
  currentDHU: number;
  orderStatus: number;

  lowestLlineEff: number;
  averagetLlineEff: number;
  highestLlineEff: number;

  qualityBottlenecks: number;

  StartDate: string;
  EndDate: string;
  Module: string;
  ProcessCode: string;
  //modulelist: {
  //  id: string;
  //  value: string;
  //}


  orderlist: {
   /* module: string;*/
    soNo: string;
    poNo: string;
    orderRemark: string;
    //style: string;
    //fit: string;
    //product: string;
    //season: string;
    //customer: string;
    //soQty: number;
    //factoryID: number;
    //noOfPO: number;
    orderStatus: number;
    //completedQty: number;
    //completedPer: number;
  }
}
