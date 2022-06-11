import { DateTime } from "@syncfusion/ej2-charts";

export class FactoryViewModel {
  factoryID: number;
  factoryName: string;
  factoryAddress: string;
  factoryType: string;
  factoryCountry: string;
  factoryTimeZone: string;
  noOfShifts: number;
  noOfUsers: number;
  organisationName: string;
  clusterName: string;
  factoryStatus: string;
  checkProduct: number;

  product_Count: number;
  product_Status: string;
  product_CreateDate: DateTime;
  product_UpdateDate: DateTime;

  fit_Count: number;
  fit_Status: string;
  fit_CreateDate: DateTime;
  fit_UpdateDate: DateTime;

  customer_Count: number;
  customer_Status: string;
  customer_CreateDate: DateTime;
  customer_UpdateDate: DateTime;

  line_Count: number;
  line_Status: string;
  line_CreateDate: DateTime;
  line_UpdateDate: DateTime;


  shift_Count: number;
  shift_Status: string;
  shift_CreateDate: DateTime;
  shift_UpdateDate: DateTime;

  qCCode_Count: number;
  qCCode_Status: string;
  qCCode_CreateDate: DateTime;
  qCCode_UpdateDate: DateTime;

  user_Count: number;
  user_Status: string;
  user_CreateDate: DateTime;
  user_UpdateDate: DateTime;

  holidays_Count: number;
  holidays_Status: string;
  holidays_CreateDate: DateTime;
  holidays_UpdateDate: DateTime;

  processDefinition_Count: number;
  processDefinition_Status: string;
  processDefinition_CreatedOn: string;
  processDefinition_LastChangedOn: string;

}
