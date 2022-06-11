export class SOViewModel {
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
  posearchtext: string;
  completedQty: number;
  defectedQty: number;
  rejectedQty: number;

  efficiencyPlanedPer: number;
  efficiencyActualPer: number;
  efficiencyRequiredPer: number;

  currentDHU: number;
  rejectionRate: number;

  plannedSAH: number;
  actualSAH: number;

delayedPO: number;
completedPO: number;
holdPO: number;
onTimePO: number;

  polist: {
    id: number;
    value: string;
  }
}
