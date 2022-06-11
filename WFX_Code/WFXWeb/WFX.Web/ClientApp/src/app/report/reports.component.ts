import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { ExcelExportService } from '../services/excelexport.service';
import { OrderService } from '../services/Order.service';
import { OrderModel } from '../../models/OrderModel';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';

import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { QCService } from '../services/qc.service';

@Component({
  selector: 'app-report',
  templateUrl: './reports.component.html'
})

export class ReportsComponent implements OnInit {
  Url: string;
  UserName: string;
  auth: string;
  public ordermodel: OrderModel;
  startdate: any;
  enddate: any;

  so: string;
  po: string;
  module: string;
  line: string;
  style: string;
  process: string;
  public data_production: object[];
  public data_target: object[];
  public data_endline: object[];
  public data_hourly: object[];
  public data_quality: object[];
  public data_defect: object[];

  constructor(
    private router: Router,
    private http: HttpClient,
    private orderservice: OrderService,
    private excelexportservice: ExcelExportService,
    private _Activatedroute: ActivatedRoute,
    private qcservice: QCService,
  ) {

    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
  }
  OnInit() {

  }

  ngOnInit() {
    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);
    //this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    //this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    this.ordermodel = new OrderModel();
    this.orderservice.FillLine().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.linelist = res.data;
      })

    this.orderservice.FillModule().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.modulelist = res.data;
      })

    this.orderservice.FillPO().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.polist = res.data;
      })

    this.orderservice.FillSO().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.solist = res.data;
      })
    this.qcservice.FillStyle({}).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.ordermodel.stylelist = res.data;
      })
    this.orderservice.FillProcess().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.processlist = res.data;
      })
  }

  StartDate(val: any) {
    //alert("blur1 " + val);
    this.startdate = val;
    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
  }

  EndDate(val: any) {
    //alert("blur1 " + val);
    this.enddate = val;
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
  }

  POChangeHandler(event: string) {
    this.po = event;
    if (this.po != '-1')
      this.ordermodel.poNo = this.po;
    else
      this.ordermodel.poNo = '';
  }

  SOChangeHandler(event: string) {
    this.so = event;
    if (this.so != '-1')
      this.ordermodel.soNo = this.so;
    else
      this.ordermodel.soNo = '';
  }

  ModuleChangeHandler(event: string) {
    this.module = event;
    if (this.module != '-1')
      this.ordermodel.module = this.module;
    else
      this.ordermodel.module = '';
  }

  ProcessChangeHandler(event: string) {
    this.process = event;
    if (this.process != '-1')
      this.ordermodel.process = this.process;
    else
      this.ordermodel.process = '';
  }

  LineChangeHandler(event: string) {
    this.line = event;
    if (this.line != '-1')
      this.ordermodel.line = this.line;
    else
      this.ordermodel.line = '';
  }

  ProductionSummary() {

    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Production Summary');

    worksheet.columns = [
      { header: 'StartDate', key: 'startDate' },
      { header: 'EndDate', key: 'endDate' },
      { header: 'Module', key: 'module' },
      { header: 'Process', key: 'process' },
      { header: 'Customer', key: 'customer' },
      { header: 'Line', key: 'line' },
      { header: 'Style', key: 'style' },
      { header: 'Product', key: 'product' },
      { header: 'Production Order', key: 'so' },
      { header: 'Work Order', key: 'po' },
      { header: 'SMV', key: 'smv' },
      { header: 'Color', key: 'color' },
      { header: 'PiecesChecked', key: 'piecesChecked' },
      { header: 'PlannedOutput', key: 'plannedOutput' },
      { header: 'ActualOutput', key: 'actualOutput' },
      { header: 'PlannedSAH', key: 'plannedSAH' },
      { header: 'ActualSAH', key: 'actualSAH' },
      { header: 'PlannedEff', key: 'plannedEff' },
      { header: 'ActualEff', key: 'actualEff' },
      { header: 'PlannedManHours', key: 'plannedManHours' },
      { header: 'ActualManHours', key: 'actualManHours' }
    ];

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.ProductionSummary(this.ordermodel).subscribe(
      res => {
        this.data_production = res.data;
        this.data_production.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'ProductionSummary.xlsx');
        })
      })
    //alert("Excel Export Done");
  }

  TargetVsActual() {

    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Target vs actual summary');

    worksheet.columns = [
      { header: 'StartDate', key: 'startDate' },
      { header: 'EndDate', key: 'endDate' },
      { header: 'Module', key: 'module' },
      { header: 'Process', key: 'processName' },
      { header: 'Customer', key: 'customer' },
      { header: 'Line', key: 'line' },
      { header: 'Style', key: 'style' },
      { header: 'Product', key: 'product' },
      { header: 'Production Order', key: 'so' },
      { header: 'Work Order', key: 'po' },
      { header: 'SMV', key: 'smv' },
      { header: 'PiecesChecked', key: 'piecesChecked' },

      { header: 'PlannedOutput', key: 'plannedOutput' },
      { header: 'ActualOutput', key: 'actualOutput' },
      { header: 'OutPutVariation', key: 'outPutVariation' },

      { header: 'PlannedSAH', key: 'plannedSAH' },
      { header: 'ActualSAH', key: 'actualSAH' },
      { header: 'SAHVariation', key: 'sahVariation' },

      { header: 'PlannedEff', key: 'plannedEff' },
      { header: 'ActualEff', key: 'actualEff' },
      { header: 'EffVariation', key: 'effVariation' },

      { header: 'PlannedManHours', key: 'plannedManHours' },
      { header: 'ActualManHours', key: 'actualManHours' },
      { header: 'HrsVariation', key: 'hrsVariation' }
    ];

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.TargetVsActual(this.ordermodel).subscribe(
      res => {
        this.data_target = res.data;
        this.data_target.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'TargetVsActualSummary.xlsx');
        })
      })
    //alert("Excel Export Done");
  }

  EndlineQCSummary() {

    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Endline QC summary');

    worksheet.columns = [
      { header: 'StartDate', key: 'startDate' },
      { header: 'EndDate', key: 'endDate' },
      { header: 'Module', key: 'module' },
      { header: 'Process', key: 'processName' },
      { header: 'Line', key: 'line' },
      { header: 'QC', key: 'qc' },
      { header: 'ShiftTimings', key: 'shiftTimings' },
      { header: 'Production Order', key: 'so' },
      { header: 'Work Order', key: 'po' },
      { header: 'Style', key: 'style' },

      { header: 'PiecesChecked', key: 'piecesChecked' },
      { header: 'DefectPieces', key: 'defectPieces' },
      { header: 'ReworkedPieces', key: 'reworkedPieces' },
      { header: 'RejectPieces', key: 'rejectPieces' },
      { header: 'ActualOutput', key: 'actualOutput' },
      { header: 'ReworkRate', key: 'reworkRate' },
      { header: 'RejectRate', key: 'rejectRate' }
    ];

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.EndlineQCSummary(this.ordermodel).subscribe(
      res => {
        this.data_endline = res.data;
        this.data_endline.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'EndlineQCSummary.xlsx');
        })
      });
  }

  HourlyProduction() {
    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Hourly Production');

    worksheet.columns = [
      { header: 'StartDate', key: 'startDate' },
      { header: 'EndDate', key: 'endDate' },
      { header: 'Module', key: 'module' },
      { header: 'Process', key: 'processName' },
      { header: 'Line', key: 'line' },
      { header: 'QC', key: 'qc' },
      { header: 'StartHour', key: 'startHour' },
      { header: 'EndHour', key: 'endHour' },
      { header: 'Production Order', key: 'so' },
      { header: 'Work Order', key: 'po' },
      { header: 'Style', key: 'style' },

      { header: 'PiecesChecked', key: 'piecesChecked' },
      { header: 'DefectPieces', key: 'defectPieces' },
      { header: 'ReworkedPieces', key: 'reworkedPieces' },
      { header: 'RejectPieces', key: 'rejectPieces' },
      { header: 'ActualOutput', key: 'actualOutput' },
      { header: 'ReworkRate', key: 'reworkRate' },
      { header: 'RejectRate', key: 'rejectRate' }
    ];

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.HourlyProduction(this.ordermodel).subscribe(
      res => {
        this.data_hourly = res.data;
        this.data_hourly.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'HourlyProduction.xlsx');
        })
      });
  }

  QualityPerfromance() {
    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('QualityPerformance');

    worksheet.columns = [
      { header: 'StartDate', key: 'startDate' },
      { header: 'EndDate', key: 'endDate' },
      { header: 'Module', key: 'module' },
      { header: 'Process', key: 'processName' },
      { header: 'Line', key: 'line' },
      { header: 'QC', key: 'qc' },
      { header: 'Production Order', key: 'so' },
      { header: 'Work Order', key: 'po' },
      { header: 'Style', key: 'style' },

      { header: 'PiecesChecked', key: 'piecesChecked' },
      { header: 'DefectPieces', key: 'defectPieces' },
      { header: 'ReworkedPieces', key: 'reworkedPieces' },
      { header: 'RejectPieces', key: 'rejectPieces' },
      { header: 'ActualOutput', key: 'actualOutput' },
      { header: 'ReworkRate', key: 'reworkRate' },
      { header: 'RejectRate', key: 'rejectRate' },
      { header: 'DHU', key: 'dhu' }
    ];

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.QualityPerfromance(this.ordermodel).subscribe(
      res => {
        this.data_quality = res.data;
        this.data_quality.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'QualityPerformance.xlsx');
        })
      });
  }

  DefectAnalysis() {
    this.ordermodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.ordermodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('DefectAnalysis');

    var col = [
      { header: 'StartDate', key: 'StartDate' },
      { header: 'EndDate', key: 'EndDate' },
      { header: 'Module', key: 'Module' },
      { header: 'Process', key: 'ProcessName' },
      { header: 'Customer', key: 'Customer' },
      { header: 'Line', key: 'Line' },
      { header: 'Style', key: 'Style' },
      { header: 'Product', key: 'Product' },
      { header: 'Production Order', key: 'SO' },
      { header: 'Work Order', key: 'PO' },
      { header: 'Color', key: 'Color' },

      { header: 'PiecesChecked', key: 'PiecesChecked' },
      { header: 'DefectPieces', key: 'DefectPieces' },
      { header: 'DefectPieces', key: 'DefectPieces' },

      { header: 'ReworkedPieces', key: 'ReworkedPieces' },
      { header: 'RejectPieces', key: 'RejectPieces' },
      { header: 'RejectRate', key: 'RejectRate' },

      { header: 'ActualOutput', key: 'ActualOutput' },
      { header: 'DHU', key: 'DHU' },
      { header: 'DefectsFound', key: 'DefectsFound' }

    ];

    col.push({ header: 'Operarion1', key: 'OP1' });
    col.push({ header: 'Operarion2', key: 'OP2' });
    col.push({ header: 'Operarion3', key: 'OP3' });

    /* worksheet.columns = col;*/
    //for (int i = 1; i < 4 ; i++)
    //{
    //  columns.push({ header: 'OP1', key: 'OP1' },)
    //}

    /*worksheet.getCell('A1').font = { bold: true };*/
    const rows = worksheet.getRow(1);
    rows.font = { size: 12, bold: true }

    this.excelexportservice.DefectAnalysis(this.ordermodel).subscribe(
      res => {
        this.data_defect = res.data;
        if (!worksheet.columns) {
          if (this.data_defect.length > 0)
            worksheet.columns = Object.keys(this.data_defect[0]).map((k) => ({ header: k, key: k }))
        }
        this.data_defect.forEach(x => {
          worksheet.addRow(x);
        });

        //Generate Excel File with given name
        workbook.xlsx.writeBuffer().then((data) => {
          let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
          fs.saveAs(blob, 'DefectAnalysis.xlsx');
        })
      });
  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  StyleChangeHandler(event: string) {
    this.style = event;
    if (this.style != '-1')
      this.ordermodel.style = this.style;
    else
      this.ordermodel.style = '';
  }
}

