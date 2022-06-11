import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

import { UserDashboardService } from '../services/userdashboard.service';
import { UserDashboardModel } from '../../models/UserDashboardModel';
import { Select2OptionData } from 'ng-Select2';
import * as moment from 'moment';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-userdashboard',
  templateUrl: './userdashboard.component.html'
})

export class UserDashboardComponent implements OnInit {
  public userdashboardmodel: UserDashboardModel;
  UserName: string;
  module: string = '';
  value: string;
  ProcessCode: string = '';
  globalsearchtext: string;
  startdate: Date;
  enddate: Date;
  modulelist: {
    id: string;
    text: string;
  }
  processlist: {
    id: string;
    text: string;
  }
  avgline: any;
  topdefectlist: object[];
  efflst: any;
  maxefflst: any;
  minefflst: any;
  AveragetLlineEff: any;
  //chart variables
  public xAxis: object;
  public yAxis: object;
  public data: object[];
  public data1: object[];
  public data2: object[];

  public legendSettings: Object;
  public tooltip: Object;
  public title: string;
  public marker: Object;

  public data3: object[];
  public datalabel: Object;
  public legendSettings2: Object;
  interval = 0;
  maximum = 0;
  Yminefflst = 0;
  Ymaxefflst = 0;

  constructor(
    private router: Router,
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private UserDashboardService: UserDashboardService,
    private orderservice: OrderService,
  ) {
    this.loadScripts();
    //  this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
  }

  ngOnInit() {

    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);

    this.GetUserDashboard();
    this.UserDashboardService.FillModule().subscribe(
      res => {
        res.data.unshift({ id: '-1', text: 'All' });
        this.modulelist = res.data;
      })
    this.orderservice.FillProcess().subscribe(
      res => {
        this.processlist = res.data;
      })

  }

  moduleChangeHandler(event: string) {
    this.module = event;
    if (this.module == '-1')
      this.module = '';
    this.GetUserDashboard();
  }
  ProcessChangeHandler(event: string) {
    this.ProcessCode = event;
    this.GetUserDashboard();
  }

  GetUserDashboard() {
    let maxYEff = 0;
    let maxinterval = 0;
    this.maximum = 100;
    this.interval = 20;
    this.userdashboardmodel = new UserDashboardModel();

    this.userdashboardmodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.userdashboardmodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.userdashboardmodel.Module = this.module;
    this.userdashboardmodel.ProcessCode = this.ProcessCode;
    this.UserDashboardService.GetUserDashboard(this.userdashboardmodel).subscribe(
      res => {
        this.userdashboardmodel = res.data;
        this.topdefectlist = res.topdefectlist;
        this.efflst = res.efflst;
        this.AveragetLlineEff = this.userdashboardmodel.averagetLlineEff;

        this.minefflst = res.minefflst;
        this.maxefflst = res.maxefflst;

        this.avgline = this.maxefflst[0].qcDay;

        if (this.maxefflst && this.maxefflst.length > 0) {
          this.Ymaxefflst = Math.max.apply(Math, this.maxefflst.map(function (o) { return o.qcEff; }))
          maxYEff = this.Ymaxefflst;
          maxinterval = 10 * maxYEff / 100;
        }
        if (this.minefflst && this.minefflst.length > 0) {
          this.Yminefflst = Math.max.apply(Math, this.minefflst.map(function (o) { return o.qcEff; }))
        }
        if (this.Yminefflst > this.Ymaxefflst) {
          maxYEff = this.Yminefflst;
          maxinterval = 10 * maxYEff / 100;
        }
        if (this.AveragetLlineEff > maxYEff) {
          maxYEff = this.AveragetLlineEff;
          maxinterval = 10 * maxYEff / 100;
        }
        if (maxYEff < 100) {
          this.maximum = 100;
          this.interval = 20;
        }
        else {
          this.maximum = Math.ceil(maxYEff + maxinterval);
          this.interval = Math.ceil(maxinterval);
        }



        this.GetChart1();
        this.userdashboardmodel.StartDate = moment.utc(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
        this.userdashboardmodel.EndDate = moment.utc(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
        this.userdashboardmodel.Module = this.module;
        this.userdashboardmodel.ProcessCode = this.ProcessCode;
        this.UserDashboardService.OrderList(this.userdashboardmodel).subscribe(
          res => {
            this.userdashboardmodel.orderlist = res.data;
          })
      })

  }

  StartDate(val: any) {
    this.startdate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.GetUserDashboard();
  }

  EndDate(val: any) {
    this.enddate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.GetUserDashboard();
  }


  GetChart1() {
    //Chart Section
    this.marker = {
      visible: true,
      dataLabel: {
        visible: true
      }
    };

    this.tooltip = {
      enable: true
    }

    this.xAxis = {
      valueType: 'Category'
    };

    this.yAxis = {
      labelFormat: '{value}',
      minimum: 0, maximum: this.maximum,
      interval: this.interval
    };

    this.legendSettings = {
      visible: true
    }

    this.datalabel = { visible: true, name: 'text', position: 'Outside' };

    this.legendSettings2 = {
      visible: true
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

}

