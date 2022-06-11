import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { OrderService } from '../services/order.service';
import { POViewModel } from '../../models/POViewModel';
import { ActivatedRoute } from '@angular/router';
import { QCService } from '../services/qc.service';
import { QCModel } from '../../models/QCModel';
import * as moment from 'moment';
import { IPointEventArgs } from '@syncfusion/ej2-angular-charts';


declare function getlist(): any;

@Component({
  selector: 'app-order',
  templateUrl: './poview.component.html'
})

export class POViewComponent implements OnInit {
  Url: string;
  UserName: string;
  auth: string;
  public poviewmodel: POViewModel;
  public qcmodel: QCModel;
  sono: string;
  pono: string;
  posearchtext: string;
  module: string;
  line: string;
  orderstatus: number;
  public defectlist: object[];
  public operationlist: object[];
  public operationlistforchart: any;
  public imagelist: object[];
  public shiftimagelist: object[];
  public dhulist: any;
  public rejlist: any;
  btnOverallSelected: boolean;
  Page: string;

  //get via chart clicked
  opheading: string;
  opdefects: string;
  public opdefectlist: object[];

  startdate: Date;
  enddate: Date;
  processCode: string;
  public planedlist: any;
  public actuallist: any;

  public prodvarianceyAxis: object;
  public dhuyAxis: object;

  public rejyAxis: object;
  public opyAxis: object;
  public data: object[];
  public data1: object[];

  public title: string;


  delay: boolean;
  img_ob: number;

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
  PassWidth: any = '0%';
  DefectWidth: any = '0%';
  RejectWidth: any = '0%';
  Totalcheckedpcs = 0;
  RejectWidthPer: any;
  DefectWidthPer: any;
  PassWidthPer: any;
  processList: any = [];
  calledFrom: string;
  imgUrl: string;
  completedPcsProcessWise: any;
  rejectedPcsProcessWise: any;
  plannedSAHProcessWise: any;
  ActulSAHlist: any;
  processDetails: any;
  constructor(
    private router: Router,
    private http: HttpClient,
    private orderservice: OrderService,
    private qcservice: QCService,
    private _Activatedroute: ActivatedRoute,
  ) {

    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    /*alert("poview " + this.UserName);*/

  }
  OnInit() {
    //alert("poview 2 " + this.UserName);
  }


  //public PointClick = function(args: IPointEventArgs): void {
  //    alert('SelectedSeriesIndex :' + args.seriesIndex);
  //  };

  public pointClick(args: IPointEventArgs): void {
    //alert("Hello");
    //alert(args.point.x);
    //alert(args.point.y);
    this.opheading = args.point.x.toString();
    this.opdefects = args.point.y.toString() + " Defects";
    //this.opdefectlist = [{ "DefectName": "DEF 1 Oil Stain", "DefectCount": 1 }, { "DefectName": "DEF 2 Run Stitch", "DefectCount": 2 }, { "DefectName": "DQ04", "DefectCount": 2 }]
    this.qcmodel.OperationName = this.opheading;
    this.qcmodel.PONo = this.pono;
    this.qcservice.POOperationDefect(this.qcmodel).subscribe(
      res => {
        this.opdefectlist = res.opdefectlist;
      })
    //document.getElementById("lbl").innerText = "X : " + args.point.x + "\nY : " + args.point.y;
  };

  ngOnInit() {
    this.delay = false;
    this.enddate = new Date();
    this.startdate = new Date();
    this.startdate.setDate(this.startdate.getDate() - 7);
    this.poviewmodel = new POViewModel();

    this._Activatedroute.queryParams
      .subscribe(params => {
        this.calledFrom = params.calledFrom;
        this.processCode = params.ProcessCode;
        this.Page = params.Page;


        this._Activatedroute.paramMap.subscribe(params => {

          if (this.Page === 'WorkOrder') {
            this.pono = params.get('pono');
            this.poviewmodel.poNo =  this.pono ;
          }
          this.sono = params.get('sono');
        });
        /*this.pono = "PO301";*/
        /*alert(this.pono);*/
        if (this.calledFrom === 'FPPO') {
           this.poviewmodel.poNo = this.pono;
          this.btnOverallSelected = false;
        }
        if (this.calledFrom === 'ProductionOrder') {
          this.btnOverallSelected = true;
          this.poviewmodel.poNo='';
          this.poviewmodel.processCode ='';
        }
        if (this.processCode) {
          this.poviewmodel.processCode = this.processCode;
        }
        this.poviewmodel.soNo = this.sono;
        let arrSO:any={};
        arrSO['SONo']=this.poviewmodel.soNo;
        this.orderservice.FactoryProcessList(arrSO).subscribe(res => {

          this.processList = res.data;
          for (let i of this.processList) {

            if (i.id == 'Sewing') {
              i.img = '../../assets/images/sewing.png';
            }
            else if (i.id == 'Finishing') {
              i.img = '../../assets/images/finishing.png';
            }
            else if (i.id == 'Packing') {
              i.img = '../../assets/images/packing.png';
            }
            else if (i.id == 'Cutting') {
              i.img = '../../assets/images/cutting.png';
            }

            else if (i.id == 'Washing') {

              i.img = '../../assets/images/washing.png';

            }
          }

        })

        this.orderservice.POView(this.poviewmodel).subscribe(
          res => {
            this.poviewmodel = res.data;
            if(this.calledFrom==='ProductionOrder'){
              this.poviewmodel.poNo='';
              this.poviewmodel.processCode='';
            }
            this.imagelist = res.imagelist;
            this.processDetails = res.processListt;
              this.processDetails.unshift({
                processCode: 'Overall'
              })
            for (let i of this.processDetails) {
              if (i.passPcs > 0) {
                i.PassWidth = i.passPcs + 20;
                i.PassWidth = i.PassWidth.toString() + 'px';
              } else {
                i.PassWidth = 0;
              }
              if (i.defectPcs > 0) {
                i.DefectWidth = i.defectPcs + 20;
                i.DefectWidth = i.DefectWidth.toString() + 'px'
              } else {
                i.DefectWidth = 0;
              }
              if (i.rejectPcs > 0) {
                i.RejectWidth = i.rejectPcs + 20;
                i.RejectWidth = i.RejectWidth.toString() + 'px'
              } else {
                i.RejectWidth = 0;
              }

            }

            if (this.poviewmodel.processCode && this.calledFrom === 'FPPO') {
              this.processCode = this.poviewmodel.processCode;
            }
            if (this.poviewmodel.expectedDelay > 0) {
              this.delay = true;
            }
            this.sono = res.data["soNo"];
            this.orderstatus = res.data["orderStatus"];
            this.loadSearch();
            if (this.calledFrom === 'ProductionOrder') {
              this.poviewmodel.poNo = '';
            }
            if(this.calledFrom==='FPPO'){
            this.orderservice.POColorSizeList(this.poviewmodel).subscribe(
              res => {
                this.poviewmodel._lstcolor = res._lstcolor;
                console.log(res._lstcolor)
                for(let i of res._lstcolor){
               let sumOfPlannedQty: number=0;
               let sumOfyetToPlanQty: number=0;
               i.sizelist.forEach((element, index)=> {
                sumOfPlannedQty=sumOfPlannedQty+element.plannedQty;
                sumOfyetToPlanQty=sumOfyetToPlanQty+element.yetToPlanQty;
               });
               if(sumOfPlannedQty===0 && sumOfyetToPlanQty===0){
                i.sizelist.forEach((element, index)=> {
                element.plannedQty='-';
                element.yetToPlanQty='-';
                 });


              }

                }
              })
            }
            this.orderservice.POPartsList(this.poviewmodel).subscribe(
              res => {
                this.poviewmodel.partlist = res.data;
              })

            this.orderservice.GetPOLineList(this.pono).subscribe(
             res => {
               if( res.data &&  res.data.length>0){
                res.data.unshift({ id: '-1', text: 'All' });
               }
                this.poviewmodel.linelist = res.data;
              })

            this.orderservice.GetPOModuleList(this.pono).subscribe(
              res => {
                if( res.data &&  res.data.length>0){
                res.data.unshift({ id: '-1', text: 'All' });
                }
                this.poviewmodel.modulelist = res.data;
              })
            this.CheckedPcsprogressBar();
          });
      });

    this.getData();
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

  loadSearch() {
    this.poviewmodel.posearchtext = this.posearchtext;
    this.poviewmodel.soNo = this.sono;
    if(this.calledFrom==='FPPO'&& this.Page==='WorkOrder'){
      this.poviewmodel.poNo='';
      this.poviewmodel.processCode='';
    }
    this.orderservice.POListOfSO(this.poviewmodel).subscribe(
      res => {
        this.poviewmodel.polist = res.data;
      })
  }

  getData() {
    this.qcmodel = new QCModel();
    this.qcmodel.PONo = this.pono;
    this.qcmodel.Line = this.line;
    this.qcmodel.Module = this.module;
    this.qcmodel.StartDate = moment(this.startdate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    this.qcmodel.EndDate = moment(this.enddate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    if(this.calledFrom==='FPPO'){
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
        if(this.rejlist && this.rejlist.length>0)
          this.qcmodel.totalRejected =Math.ceil(this.qcmodel.totalRejected/this.rejlist.length);
        else
          this.qcmodel.totalRejected =0;
      })
  }
}

  ModuleChangeHandler(event: any) {
    this.module = event;
    if (this.module == '-1')
      this.module = null;
    if (this.module != "") { this.getData(); }
  }

  LineChangeHandler(event: any) {
    this.line = event;
    if (this.line == '-1')
      this.line = null;
    if (this.line != "") { this.getData(); }
  }
  onChangeEvent(event: any) {
    this.posearchtext = event.target.value;
    this.loadSearch();
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

  CheckOrderStatusForEdit() {
    if (this.orderstatus == 1) {
      this.router.navigate(['/User/EditOrder/' + this.pono])
    }
    else {
      alert("You can not edit this order beacause its already completed")
    }
  }

  CheckOrderStatusForImage(image_ob: number) {
    if (this.orderstatus == 1) {
      this.img_ob = image_ob;//  0 for image tab, 1 for ob
      this.router.navigate(['/User/IMGEditor/' + this.sono + '/' + this.img_ob])
      //if (this.img_ob == 0)
      //{
      //  console.log('Image Section call');
      //  this.router.navigate(['/User/IMGEditor/' + this.pono + '/' + this.img_ob])
      //}
      //else {
      //  console.log('OB Section call');
      //  this.router.navigate(['/User/IMGEditor/' + this.pono + '/' + this.img_ob])
      //}
    }
    else {
      alert("You can not edit image this order beacause its already completed")
    }
  }

  CheckOrderStatusForPlan() {
    if (this.orderstatus == 1) {
      this.router.navigate(['/User/PlanEditor/' + this.pono])
    }
    else {
      alert("You can not edit plan this order beacause its already completed")
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
  CheckedPcsprogressBar() {
    if (!this.poviewmodel.completedQty) {
      this.poviewmodel.completedQty = 0;
    }
    if (!this.poviewmodel.defectedQty) {
      this.poviewmodel.defectedQty = 0;
    }
    if (!this.poviewmodel.rejectedQty) {
      this.poviewmodel.rejectedQty = 0;
    }
    this.Totalcheckedpcs = this.poviewmodel.completedQty + this.poviewmodel.defectedQty + this.poviewmodel.rejectedQty;
    if (this.Totalcheckedpcs > 0) {
      this.PassWidth = this.poviewmodel.completedQty * 100 / this.Totalcheckedpcs;
      this.DefectWidth = this.poviewmodel.defectedQty * 100 / this.Totalcheckedpcs;
      this.RejectWidth = this.poviewmodel.rejectedQty * 100 / this.Totalcheckedpcs;
      if (this.PassWidth < 15 && this.poviewmodel.completedQty > 0) {
        this.PassWidth = 15;
      }
      if (this.DefectWidth < 15 && this.poviewmodel.defectedQty > 0) {

        this.DefectWidth = 15

      }
      if (this.RejectWidth < 15 && this.poviewmodel.rejectedQty > 0) {
        this.RejectWidth = 15;
      }
      if (this.poviewmodel.rejectedQty < 1) {
        this.RejectWidth = 0;

      }

      if (this.poviewmodel.defectedQty < 1) {
        this.DefectWidth = 0;

      }
      if (this.poviewmodel.completedQty < 1) {
        this.PassWidth = 0;

      }
      if ((this.RejectWidth + this.PassWidth + this.DefectWidth) > 100) {
        const more = (this.RejectWidth + this.PassWidth + this.DefectWidth) - 100;
        if (this.RejectWidth > 15) {
          this.RejectWidth = this.RejectWidth - more;
          if (this.RejectWidth < 15) {
            this.RejectWidth = 15;
          }
        }
        if (this.PassWidth > 15) {
          this.PassWidth = this.PassWidth - more;
          if (this.PassWidth < 15) {
            this.PassWidth = 15;
          }
        }
        if (this.DefectWidth > 15) {
          this.DefectWidth = this.DefectWidth - more;
          if (this.DefectWidth < 15) {
            this.DefectWidth = 15;
          }
        }
      }
      this.DefectWidthPer = this.DefectWidth + '%'
      this.RejectWidthPer = this.RejectWidth + '%'
      this.PassWidthPer = this.PassWidth + '%'
    }
  }
  onButtonClick(e: any, item) {
    this.btnOverallSelected = false;
    this.poviewmodel.processCode = item.id
    this.poviewmodel.poNo = '';
    this.orderservice.POView(this.poviewmodel).subscribe(
      res => {
        this.poviewmodel = res.data;
        if (this.poviewmodel.poNo) {

          this.router.navigate(['/User/POView/' + this.poviewmodel.soNo], { queryParams: { calledFrom: 'FPPO', ProcessCode: item.id } });
        }

      });
  }
  CheckOrderStatusForProcessTemplate() {
    if (this.orderstatus == 1) {
    this.router.navigate(['/User/ProcessTemplate/' + this.sono])
  }else{
    alert("You can not edit process template for this order beacause its already completed")
  }
}
  trackByButton(index: number, item: any) {
    return item;
  }
  trackByFPPO(index: number, item: any) {
    return item;
  }
  onOverallButtonClick(e: any) {
    this.btnOverallSelected = true;
    this.router.navigate(['/User/POView/' + this.poviewmodel.soNo], { queryParams: { calledFrom: 'ProductionOrder' } });

  }
  FPPOClick(item:any){
   let url=window.location.origin + '/User/POView/'+ item +'/'+ this.poviewmodel.soNo+'?calledFrom=FPPO&Page=WorkOrder'
     window.open(url,'_self')

  }
}

