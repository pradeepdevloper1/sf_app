export class HourlyProductionModel {


  target: number;
  outputGoodPCS: number;
  reWorkDefectPCS: number;
  rejectPCS: number;
  variation: number;

  picesChecked: number;
  picesCheckedForDisplay: number;
  defectRate: number;
  hourlytarget: number;
  module: string;
  line: string;
  soNo: string;
  poNo: string;
  style: string;
  fit: string;
  product: string;
  season: string;
  customer: string;
  factoryID: number;
  recvDate: Date;
  exFactoryDate: Date;

  timeLeft: string;
  hourlyAchieve: number;

  effAchieve: number;
  effTarget: number;
  rejectRate: number;
  
  hourlyProductionList: {
    hour: string;
    target: number;
    output: number;
    variation: number;
    defectRate: number;
  }

  defectList: {
    defectName: string;
    defectCount: number;
  }

  rejectionDefectList: {
    defectName: string;
    defectCount: number;
  }

  shiftname: string;
}
