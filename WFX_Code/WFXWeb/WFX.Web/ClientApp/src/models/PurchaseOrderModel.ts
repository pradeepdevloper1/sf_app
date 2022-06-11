export class PurchaseOrderModel {
  module: string;
  soNo: string;
  poNo: string;
  style: string;
  fit: string;
  product: string;
  season: string;
  customer: string;
  soQty: number;
  factoryID: number;
  noOfPO: number;
  poQty: number;
  completedQty: number;
  completedPer: number;
  plannedQty: number;
  plannedPer: number;
  orderRemark: string;
  efficiencyPlanedPer: number;
  efficiencyActualPer: number;
  efficiencyRequiredPer: number;  
  efficiencyLossGain: number;
  orderID: number;
  currentDHU: number;
  rejectionRate: number; 
  source: string;
  plannedSAH: number;
  actualSAH: number;
  picesChecked: number;

  exFactory: Date;
  recvDate: Date;
  planStDt: Date;
  planEndDate: Date;
  actualStartDate: Date;

  expectedDelay: number;
  expectedDelDate: Date;
  orderStatusText: string;
 
  plannedManHrs: number;
  actualManHrs: number;

  imagePath: string;
  imageName: string;
  imgno : number;

  posearchtext: string;
  poImageID: number;
  //ExFactory: date;
  //EntryDate: date;
  imagelist: {
    imageName: string;
    imagePath: string;
  }

  polist: {
    id: number;
    text: string;
  }

  linelist: {
    id: string;
    text: string;
  }



  _lstcolor: {
    colorname: string;
    febric: string;
    size1Qty: number;
    size2Qty: number;
    size3Qty: number;
    size4Qty: number;
    size5Qty: number;
    size6Qty: number;
    totalQty: number;

    size1Name: string;
    size2Name: string;
    size3Name: string;
    size4Name: string;
    size5Name: string;
    size6Name: string;
  }


}
