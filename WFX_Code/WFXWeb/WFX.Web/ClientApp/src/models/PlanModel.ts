export class PlanModel {
  Line: string;
  Module: string;
  PONo: string;
  ProcessCode: string;
  pono: string;
  sono: string;

  IsTargetStartDate: number;
  IsTargetEndDate: number;
  TargetStartDate: string;
  TargetEndDate: string;

  linelist: {
    id: string;
    text: string;
  }
  modulelist: {
    id: string;
    text: string;
  }
  polist: {
    id: string;
    text: string;
  }
  processlist: {
    id: string;
    text: string;
  }
}
