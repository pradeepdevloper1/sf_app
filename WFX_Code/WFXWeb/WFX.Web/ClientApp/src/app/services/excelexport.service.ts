import { Injectable } from '@angular/core';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { QCModel } from '../../models/QCModel';

@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {

  Url: string;
  header: any;
  auth: string;
  public qcmodel: QCModel;
 
  public data_production: object[];
  public data_target: object[];
  public data_endline: object[];
  public data_hourly: object[];
  public data_quality: object[];
  public data_defect: object[];

  constructor(private http: HttpClient) {
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  generateExcel() {

    //Excel Title, Header, Data
    const title = 'Car Sell Report';
    /*const header = ["Year", "Month", "Make", "Model", "Quantity", "Pct"]*/

    const header = ["Year", "Make"]

    //const data = [ 
    //  [2007, 1, "Volkswagen ", "Volkswagen Passat", 1267, 10],
    //  [2007, 1, "Toyota ", "Toyota Rav4", 819, 6.5]
    //];

    /*let arr = [1, 3, 'Apple', 'Orange', 'Banana', true, false];*/
    //let data = [
    //  [1, 'JAN'],
    //  [1, 'FEB'],
    //  [1, 'MAR']
    //];

    var apidata = [
      { Month: 1, Make: "Volkswagen" },
      { Month: 1, Make: "Toyota" }
    ];

    const data = Object.keys(apidata);
    console.log("Data");
    console.log(data);
    //const data = [
    //  { Year: 2007, Month: 1, Make: "Volkswagen", Model: "Volkswagen Passat", Quantity: 1267, Pct:10},
    //  { Year: 2007, Month: 1, Make: "Toyota", Model: "Toyota Rav4", Quantity: 819, Pct: 6.5 }
    //];



    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Car Data');


    //Add Row and formatting
    let titleRow = worksheet.addRow([title]);
    titleRow.font = { name: 'Comic Sans MS', family: 4, size: 16, underline: 'double', bold: true }
    worksheet.addRow([]);
    //let subTitleRow = worksheet.addRow(['Date : ' + this.datePipe.transform(new Date(), 'medium')])


    //Add Image
    //let logo = workbook.addImage({
    //  base64: logoFile.logoBase64,
    //  extension: 'png',
    //});

    //worksheet.addImage(logo, 'E1:F3');
    //worksheet.mergeCells('A1:D2');


    //Blank Row 
    worksheet.addRow([]);

    //Add Header Row
    let headerRow = worksheet.addRow(header);

    // Cell Style : Fill and Border
    headerRow.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF00' },
        bgColor: { argb: 'FF0000FF' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    })
    // worksheet.addRows(data);


    // Add Data and Conditional Formatting
    data.forEach(d => {
      let row = worksheet.addRow(d);
      let qty = row.getCell(5);
      let color = 'FF99FF99';
      if (+qty.value < 500) {
        color = 'FF9999'
      }

      qty.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: color }
      }
    }

    );

    worksheet.getColumn(3).width = 30;
    worksheet.getColumn(4).width = 30;
    worksheet.addRow([]);


    //Footer Row
    let footerRow = worksheet.addRow(['This is system generated excel sheet.']);
    footerRow.getCell(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFCCFFE5' }
    };
    footerRow.getCell(1).border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }

    //Merge Cells
    worksheet.mergeCells(`A${footerRow.number}:F${footerRow.number}`);

    //Generate Excel File with given name
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, 'CarData.xlsx');
    })

  }

  ProductionSummary(model: any) {
    var res = this.http.post<any>(this.Url + "Report/ProductionSummary", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  TargetVsActual(model: any) {
    var res = this.http.post<any>(this.Url + "Report/TargetVsActual", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  EndlineQCSummary(model: any) {
    var res = this.http.post<any>(this.Url + "Report/EndlineQCSummary", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  HourlyProduction(model: any) {
    var res = this.http.post<any>(this.Url + "Report/HourlyProduction", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  QualityPerfromance(model: any) {
    var res = this.http.post<any>(this.Url + "Report/QualityPerfromance", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }

  DefectAnalysis(model: any) {
    var res = this.http.post<any>(this.Url + "Report/DefectAnalysis", model, {
      headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
    })
    return res;
  }
}
