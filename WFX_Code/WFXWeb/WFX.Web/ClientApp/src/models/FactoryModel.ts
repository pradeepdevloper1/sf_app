export class FactoryModel {
 organisationID: number;
  factoryID: number;
  clusterID: number;
  factoryName: string;
  factoryAddress: string;
  factoryType: string;
  linkedwithERP: string;
  factoryHead: string;

  factoryEmail: string;
  factoryContactNumber: number;
  factoryCountry: number;
  factoryTimeZone: string;
  noOfShifts: number;
  decimalValue: number;
  ptmPrice: number;
  noOfUsers: number;

  factoryOffOn: string;
  measuringUnit: string;
  dataScale: number;
  noOfLine: number;
  smartLines: number;

  organisationName: string;
  clusterName: string;



  organisationlist: {
    id: number;
    text: string;
  }
  clusterlist: {
    id: number;
    text: string;
  }
  factorytypelist: {
    id: string;
    text: string;
  }
  linkedwitherplist: {
    id: string;
    text: string;
  }
  countrylist: {
    id: number;
    text: string;
  }
  timezonelist: {
    id: string;
    text: string;
  }
}
