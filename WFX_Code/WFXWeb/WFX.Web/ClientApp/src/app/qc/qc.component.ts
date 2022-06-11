import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { QCService } from '../services/qc.service';
import { QCModel } from '../../models/QCModel';
import { QCSearchModel } from '../../models/QCSearchModel';
import { ActivatedRoute } from '@angular/router';
import { ViewEncapsulation } from '@angular/core';

import { IPointEventArgs } from '@syncfusion/ej2-angular-charts';

import * as moment from 'moment';

@Component({
  selector: 'app-qc',
  templateUrl: './qc.component.html',
  encapsulation: ViewEncapsulation.None
})

export class QCComponent implements OnInit {
  Url: string;
  UserName: string;
  auth: string;
  public qcmodel: QCModel;
  public qcsearchmodel: QCSearchModel;
  po: string = '';
  style: string = '';
  fit: string = '';
  season: string = '';
  customer: string = '';
  so: string = '';
  module: string = '';
  line: string = '';
  product: string = '';
  ProcessCode: string = '';
  public datalabel: Object;

  public defectlist: object[];
  public operationlist: object[];
  public operationlistforchart: any;
  public shiftimagelist: object[];
  public dhulist: any;
  public rejlist: any;

  public prodvarianceyAxis: object;
  public dhuyAxis: object;

  public rejyAxis: object;
  public opyAxis: object;
  public data: object[];
  public data1: object[];

  startdate: Date;
  enddate: Date;

  public planedlist: any;
  public actuallist: any;


  public title: string;


  maxprodvarianceYaxis = 50;
  maxdhuYaxis = 50;
  maxrejYaxis = 50;
  maxopYaxis = 50;
  rejintervalvalue = 10;
  dhuintervalvalue = 10;
  opintervalvalue = 10;
  maxplannedQty = 0;
  maxactualQty = 0;
  prodvarianceintervalvalue = 10;
  legendSettings = {
    visible: true
  }
  marker = {
    visible: true,
    dataLabel: {
      visible: true
    }
  };
  tooltip = {
    enable: true
  }
  xAxis = {
    valueType: 'DateTime',
    labelFormat: 'd'
  };
  opXAxis = {
    valueType: 'Category',
  };
  //get via chart clicked
  opheading: string;
  opdefects: string;
  public opdefectlist: object[];

  constructor(
    private router: Router,
    private http: HttpClient,
    private qcservice: QCService,
    private _Activatedroute: ActivatedRoute
  ) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
  }

  public pointClick(args: IPointEventArgs): void {
    //alert("Hello");
    //alert(args.point.x);
    //alert(args.point.y);
    this.opheading = args.point.x.toString();
    this.opdefects = args.point.y.toString() + " Defects";
    //this.opdefectlist = [{ "DefectName": "DEF 1 Oil Stain", "DefectCount": 1 }, { "DefectName": "DEF 2 Run Stitch", "DefectCount": 2 }, { "DefectName": "DQ04", "DefectCount": 2 }]
    this.qcmodel.OperationName = this.opheading;
    this.qcmodel.PONo = this.po;
    this.qcservice.POOperationDefect(this.qcmodel).subscribe(
      res => {
        this.opdefectlist = res.opdefectlist;
      })
    //document.getElementById("lbl").innerText = "X : " + args.point.x + "\nY : " + args.point.y;
  };

  getData() {
    this.qcmodel = new QCModel();
    this.qcmodel.PONo = this.po;
    this.qcmodel.Line = this.line;
    this.qcmodel.Module = this.module;
    this.qcmodel.Fit = this.fit;
    this.qcmodel.Product = this.product;
    this.qcmodel.Customer = this.customer;
    this.qcmodel.Season = this.season;
    this.qcmodel.Style = this.style;
    this.qcmodel.SONo = this.so;
    this.qcmodel.ProcessCode = this.ProcessCode;
    //this.qcmodel.PONo = "PO301";
    this.qcmodel.StartDate = moment(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.qcmodel.EndDate = moment(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.loadDropDownData(this.qcmodel);
    this.qcservice.Performance(this.qcmodel).subscribe(
      res => {
        this.qcmodel = res.data;
        this.planedlist = res.planedlist;
        this.actuallist = res.actuallist;
        this.operationlist = res.operationlist;
        this.operationlistforchart = res.operationlistforchart;
        this.defectlist = res.defectlist;
        this.shiftimagelist = res.shiftimagelist;
        this.dhulist = res.dhulist;
        this.rejlist = res.rejlist;
        this.planedlist.map((e: any) => {
          e.planDate = new Date(e.planDate);
        })
        this.actuallist.map((e: any) => {
          e.qcDate = new Date(e.qcDate);
        })
        this.dhulist.map((e: any) => {
          e.qcDate = new Date(e.qcDate);
        })
        this.rejlist.map((e: any) => {
          e.qcDate = new Date(e.qcDate);
        })

        if (this.planedlist && this.planedlist.length > 0) {
          this.maxplannedQty = Math.max.apply(Math, this.planedlist.map(function (o) { return o.planQty; }))
          this.maxprodvarianceYaxis = this.maxplannedQty;
          this.prodvarianceintervalvalue = 10 * this.maxprodvarianceYaxis / 100;
        }
        if (this.actuallist && this.actuallist.length > 0) {
          this.maxactualQty = Math.max.apply(Math, this.actuallist.map(function (o) { return o.qcQty; }))
          // this.prodvarianceintervalvalue = 10 * this.maxprodvarianceYaxis / 100;
        }
        if (this.maxactualQty > this.maxplannedQty) {
          this.maxprodvarianceYaxis = this.maxactualQty;
          this.prodvarianceintervalvalue = 10 * this.maxprodvarianceYaxis / 100;
        }
        if (this.dhulist && this.dhulist.length > 0) {
          this.maxdhuYaxis = Math.max.apply(Math, this.dhulist.map(function (o) { return o.dayDefectsPer; }))
          this.dhuintervalvalue = 10 * this.maxdhuYaxis / 100;
        }
        if (this.rejlist && this.rejlist.length > 0) {
          this.maxrejYaxis = Math.max.apply(Math, this.rejlist.map(function (o) { return o.rejectPer; }))
          this.rejintervalvalue = 10 * this.maxrejYaxis / 100;
        }
        if (this.operationlistforchart && this.operationlistforchart.length > 0) {
          this.maxopYaxis = Math.max.apply(Math, this.operationlistforchart.map(function (o) { return o.defectCount; }))
          this.opintervalvalue = 10 * this.maxopYaxis / 100;
        }
        this.ProductionVarianceChart();
        this.dhugraph();
        this.rejgraph();
        this.opgraph();
        if (this.dhulist && this.dhulist.length > 0)
          this.qcmodel.dhuMax = Math.max.apply(Math, this.dhulist.map(function (o) { return o.dayDefectsPer; }))
        else
          this.qcmodel.dhuMax = 0
        if (this.rejlist && this.rejlist.length > 0)
          this.qcmodel.totalRejected = Math.ceil(this.qcmodel.totalRejected / this.rejlist.length);
        else
          this.qcmodel.totalRejected = 0;
      })
  }

  loadDropDownData(data:any){
    if(data.ProcessCode == null || data.ProcessCode == undefined || data.ProcessCode == "")
      this.fillProcess(data);
    if(data.Product == null || data.Product == undefined || data.Product == "")
      this.fillProduct(data); 
    if(data.Fit == null || data.Fit == undefined || data.Fit == "")
      this.fillProductFit(data);
    if(data.Style == null || data.Style == undefined || data.Style == "")
      this.fillStyle(data);
    if(data.Season == null || data.Season == undefined || data.Season == "")
      this.fillSeason(data);
    if(data.Customer == null || data.Customer == undefined || data.Customer == "")
      this.fillCustomer(data);
    if(data.SONo == null || data.SONo == undefined || data.SONo == "")
      this.fillSO(data);
    if(data.Module == null || data.Module == undefined || data.Module == "")
      this.fillModule(data);
    if(data.Line == null || data.Line == undefined || data.Line == "")
      this.fillLine(data);
    if(data.PONo == null || data.PONo == undefined || data.PONo == "")
      this.fillPO(data);
  }
  ngOnInit() {

    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);
    this.getData();

    this.qcmodel = new QCModel();
    this.qcmodel.orderQty = 0;
    this.qcmodel.plannedQty = 0;
    this.qcmodel.plannedPer = 0;
    this.qcmodel.completedQty = 0;
    this.qcmodel.completedPer = 0;

    this.qcmodel.efficiencyPlanedPer = 0;
    this.qcmodel.efficiencyActualPer = 0;
    this.qcmodel.efficiencyRequiredPer = 0;

    this.qcmodel.plannedSAM = 0;
    this.qcmodel.actualSAM = 0;
    this.qcmodel.actualSAMOfOneGarment = 0;

    this.qcmodel.plannedManHrs = 0;
    this.qcmodel.actualManHrs = 0;

    this.qcmodel.dhuCurrent = 0;
    this.qcmodel.totalDefectsFound = 0;
    this.qcmodel.dhuAvg = 0;
    this.qcmodel.dhuMax = 0;

    this.qcmodel.rejCurrent = 0;
    this.qcmodel.totalRejected = 0;
    this.qcmodel.totalInspected = 0;
    this.qcmodel.rejAvg = 0;
    this.qcmodel.rejMax = 0;

    this.qcsearchmodel = new QCSearchModel();
  }

  fillProcess(data:any){
    this.qcservice.FillProcess(data).subscribe(
    res => {
      res.data.unshift({ id: '-1', text: 'All' });
      this.qcsearchmodel.processlist = res.data;
    })
  }

  fillProduct(data:any){
    this.qcservice.FillProduct(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.productlist = res.data;
        //this.product = data.Product;
      })
  }

  fillProductFit(data:any){
    this.qcservice.FillProductFit(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.productfitlist = res.data;
        
      })
  }

  fillStyle(data:any){
    this.qcservice.FillStyle(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.stylelist = res.data;
      })
  }
  
  fillSeason(data:any){
    this.qcservice.FillSeason(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.seasonlist = res.data;
      })
  }


  fillCustomer(data:any){
    this.qcservice.FillCustomer(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.customerlist = res.data;
      })
  }

  fillSO(data:any){
    this.qcservice.FillSO(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.solist = res.data;
      })
  }
  fillModule(data:any){
    this.qcservice.FillModule(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.modulelist = res.data;
      })
  }
  fillLine(data:any){
    this.qcservice.FillLine(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.linelist = res.data;
      })
  }
  fillPO(data:any){
    this.qcservice.FillPO(data).subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.qcsearchmodel.polist = res.data;
      })
  }

  ProductionVarianceChart() {
    this.prodvarianceyAxis = this.axis(this.maxprodvarianceYaxis, Math.ceil(this.prodvarianceintervalvalue));
  }

  StartDate(val: any) {
    //alert("blur1 " + val);
    this.startdate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.getData();
  }

  EndDate(val: any) {
    //alert("blur1 " + val);
    this.enddate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.getData();
  }

  POChangeHandler(event: any) {
    this.po = event;
    if (this.po == '-1')
      this.po = undefined;
    if (this.po != "") { 
      this.getData();
    }
  }

  StyleChangeHandler(event: any) {
    this.style = event;
    if (this.style == '-1')
      this.style = undefined;
    if (this.style != "") { 
      this.getData(); 
    }
  }

  FitChangeHandler(event: any) {
    this.fit = event;
    if (this.fit == '-1')
      this.fit = undefined;
    if (this.fit != "") { 
      this.getData(); 
    }
  }

  SeasonChangeHandler(event: any) {
    this.season = event;
    if (this.season == '-1')
      this.season = undefined;
    if (this.season != "") { 
      this.getData(); 
    }
  }

  CustomerChangeHandler(event: any) {
    this.customer = event;
    if (this.customer == '-1')
      this.customer = undefined;
    if (this.customer != "") { 
      this.getData(); 
    }
  }

  SOChangeHandler(event: any) {
    this.so = event;
    if (this.so == '-1')
      this.so = undefined;
    if (this.so != "") { 
      this.getData(); 
    }
  }

  ModuleChangeHandler(event: any) {
    this.module = event;
    if (this.module == '-1')
      this.module = undefined;
    if (this.module != "") { 
      this.getData(); 
    }
  }

  LineChangeHandler(event: any) {
    this.line = event;
    if (this.line == '-1')
      this.line = undefined;
    if (this.line != "") { 
      this.getData(); 
    }
  }
  ProductChangeHandler(event: any) {
    this.product = event;
    if (this.product == '-1')
      this.product = undefined;
    if (this.product != "") { 
      this.getData(); 
    }
  }

  ProcessChangeHandler(event: any) {
    this.ProcessCode = event;
    if (this.ProcessCode == '-1')
      this.ProcessCode = undefined;
    if (this.ProcessCode != "") { 
      this.getData(); 
    }
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

  dhugraph() {
    this.dhuyAxis = this.axis(this.maxdhuYaxis, Math.ceil(this.dhuintervalvalue));
  }
  rejgraph() {
    this.rejyAxis = this.axis(this.maxrejYaxis, Math.ceil(this.rejintervalvalue));
  }
  opgraph() {
    this.opyAxis = this.opaxis(this.maxopYaxis, Math.ceil(this.opintervalvalue));
  }

  axis(maxYaxis, interval) {
    if (maxYaxis <= 10) {
      interval = 1;
    }
    let axis = {
      minimum: 0,
      maximum: maxYaxis + interval,
      interval: interval
    };
    return axis;
  }
  opaxis(maxYaxis, interval) {
    if (maxYaxis <= 10) {
      interval = 1;
    }
    let axis = {
      labelFormat: '{value}',
      minimum: 0,
      maximum: maxYaxis + interval,
      interval: interval
    };
    return axis;
  }
}

