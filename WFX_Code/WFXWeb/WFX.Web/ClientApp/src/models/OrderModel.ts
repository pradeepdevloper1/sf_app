export class OrderModel {
  module: string;
  line: string;
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
  orderStatus: number;
  completedQty: number;
  completedPer: number;

  StartDate: string;
  EndDate: string;

  process: string;

  orderstatuslist: {
    id: number;
    value: string;
  }

  solist: {
    id: number;
    text: string;
  }

  modulelist: {
    id: number;
    text: string;
  }

  processlist: {
    id: string;
    text: string;
  }

  linelist: {
    id: number;
    text: string;
  }

  polist: {
    id: number;
    text: string;
  }
  stylelist: {
    id: number;
    text: string;
  }
}
