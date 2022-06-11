export class POViewModel {
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
  defectedQty: number;
  rejectedQty: number;
  completedPer: number;
  plannedQty: number;
  plannedPer: number;

  efficiencyPlanedPer: number;
  efficiencyActualPer: number;
  efficiencyRequiredPer: number;
  efficiencyLossGain: number;

  currentDHU: number;
  rejectionRate: number;

  plannedSAH: number;
  actualSAH: number;
  picesChecked: number;

  exFactoryDate: Date;
  recvDate: Date;
  planStartDate: Date;
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
  orderStatus : number;
  posearchtext: string;
  poImageID: number;
  processCode: string;
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

  modulelist: {
    id: string;
    text: string;
  }

  _lstcolor: {
    colorname: string;
    sizelist: {
      sizename: string;
      sizeqty: number;
      sizeqtylist: {
        orderqty: number;
        plannedqty: number;
        yettoplanqty: number;
        completedqty: number;
        complper: number;
      }
    }
  }

  partlist: {
    part: string;
    poNo: string;
  }
}
