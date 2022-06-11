export class QCModel {
  orderQty: number;
  plannedQty: number;
  plannedPer: number;
  completedQty: number;
  completedPer: number;

  efficiencyPlanedPer: number;
  efficiencyActualPer: number;
  efficiencyRequiredPer: number;

  plannedSAM: number;
  actualSAM: number;
  actualSAMOfOneGarment: number;

  plannedManHrs: number;
  actualManHrs: number;

  dhuCurrent: number;
  totalDefectsFound: number;
  dhuAvg: number;
  dhuMax: number;

  rejCurrent: number;
  totalRejected: number;
  totalInspected: number;
  rejAvg: number;
  rejMax: number;
  rejMaxPer: number;


  plannedSAH: number;
  actualSAH: number;

  currRejected: number;

  StartDate: string;
  EndDate: string;
  Module: string;
  PONo: string;
  Line: string;
  OperationName: string;
  Product: string;
  Fit: string;
  Customer: string;
  Season: string;
  Style: string;
  SONo: string;
  ProcessCode: string;
}
