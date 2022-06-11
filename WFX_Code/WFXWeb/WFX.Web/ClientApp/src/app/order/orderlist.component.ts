import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../services/order.service';
import { OrderModel } from '../../models/OrderModel';
import { RequestStatusModel } from '../../models/RequestStatusModel';

import { environment } from '../../environments/environment';
import { CellCustomComponent } from '../order/cell-custom.component';
import { CellPOViewComponent } from '../order/cell-poview.component';
import { CellSOViewComponent } from '../order/cell-soview.component';
import { CellRequeststatusdeleteComponent } from '../order/cell-requeststatusdelete.component';

import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { OrderListProgressBarComponent } from './orderlistprogressbar.component';
@Component({
  selector: 'app-order',
  templateUrl: './orderlist.component.html'
})

export class OrderListComponent implements OnInit {
  syncbtnvisiblity: boolean = false;
  syncspanvisiblity: boolean = true;
  orderstatusvisiblity:boolean=true;
  syncbtndisable: boolean = false;

  UserName: string;
  Url: string;
  header: any;
  auth: string;
  data: any;
  linkedwithERP: string;
  orderstatus: number;
  pono: string;
  disabled: boolean = true;
  public ordermodel: OrderModel;
  public requeststatusmodel:RequestStatusModel;

  public OLgridApi;
  public OLgridColumnApi;
  public RSgridApi;
  public RSgridColumnApi;
  public columnDefs;
  public requeststatus_columnDefs;
  public defaultColDef;
  public rowData: any;
  public requeststatus_rowData: any;
  selectedTab='orderlistTab';
tooltipShowDelay =0;


  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject('BASE_URL') baseUrl: string,
    private OrderService: OrderService,
    private _Activatedroute: ActivatedRoute,
    private translate: TranslateService
  ) {

    let lang = sessionStorage.getItem('lang');
    this.linkedwithERP = sessionStorage.getItem("LinkedwithERP");
    if (this.linkedwithERP == "YES") {
      this.syncbtnvisiblity = true;

      let gridData = translate.store.translations[lang].lang;
      this.columnDefs = [
        {
          headerName: gridData["process"],
          field: 'processName',
          minWidth: 120,
          headerTooltip:  gridData["process"]
        },

        {
          headerName: gridData["productionorder"],
          field: 'soNo',
          minWidth: 160,       
          "cellRendererFramework": CellSOViewComponent,
          headerTooltip:  gridData["productionorder"]
        },
        {
          headerName: gridData["workorder"],
          field: 'poNo',
          minWidth: 140,       
          "cellRendererFramework": CellPOViewComponent,
          headerTooltip:  gridData["workorder"]
        },
        { headerName: gridData["style"], field: "style", headerTooltip:  gridData["style"],minWidth:120  },
        { headerName: gridData["customer"], field: "customer", minWidth: 120 ,  headerTooltip:  gridData["customer"]},
        { headerName: gridData["quantity"], field: "poQty", minWidth: 120 , headerTooltip:  gridData["quantity"] },
        { headerName: gridData["completedQty"], field: "completedQty", headerTooltip:  gridData["completedQty"] ,minWidth: 120 },
        {
          headerName: gridData["completion%"], field: "completedPer",
          "cellRendererFramework": OrderListProgressBarComponent,
          minWidth: 150,filter: false, headerTooltip:  gridData["completion%"],tooltipValueGetter:  (params) => {
            return params.value+'%';
          }
        },

        {
          headerName: gridData['received'], field: "entryDate",
          minWidth: 120,headerTooltip:  gridData["received"],
          cellRenderer: (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          }
        },
        {
          headerName: gridData["exFactory"], field: "exFactory",
          minWidth: 120, headerTooltip:  gridData["exFactory"],
          cellRenderer: (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          }
        },
        {
          headerName: gridData["lastsyncedat"], field: "lastSyncedAt",
          minWidth: 120, headerTooltip:  gridData["lastsyncedat"],
          cellRenderer: (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          }
        },
        { field: "fulfillmentType", headerName: gridData["fulfillment"], headerTooltip: gridData["fulfillment"] ,minWidth: 120},

        {
          headerName: gridData["action"],
          field: 'poqty',
          minWidth: 180,
          "cellRendererFramework": CellCustomComponent,
          filter: false,
          headerTooltip:  gridData["action"]
        }
      ];
      this.requeststatus_columnDefs = [
        {
          headerName: '',
          width: 60,
          headerCheckboxSelection: true,
          checkboxSelection: true,
        },
        { field: "tranNum", headerName: gridData["requestid"] ,headerTooltip:  gridData["requestid"],minWidth: 120},
        { field: "poNo", headerName: gridData["workorder"], headerTooltip: gridData["workorder"],minWidth: 120},
        { field: "lineNumber", headerName: gridData["lineno"] ,headerTooltip:  gridData["lineno"],minWidth: 120},
        { field: "styleRef", headerName: gridData["styleref"] ,headerTooltip:  gridData["styleref"],minWidth: 120},
        { field: "quantity", headerName: gridData["quantity"] ,headerTooltip: gridData["quantity"],minWidth: 120},
        {
          field: "syncedAt", headerName: gridData["syncedat"],headerTooltip: gridData["syncedat"],minWidth: 120, cellRenderer: (params) => {
            return moment.utc(params.value).format('hh:mm A DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return  moment.utc(params.value).format('hh:mm A DD/MM/YY')
          }
        },
        { field: "grNstatus", headerName: gridData["grnstatus"] , headerTooltip: gridData["grnstatus"],minWidth: 120},
        // {
        //   field: "requestID", headerName: gridData["action"], minWidth: 100, headerTooltip: gridData["action"],
        //   "cellRendererFramework": CellRequeststatusdeleteComponent,
        //   filter: false
        // }
      ];
    }
    if (this.linkedwithERP == "NO") {
      let gridData = translate.store.translations[lang].lang;
      this.columnDefs = [
        {
          headerName: gridData["process"],
          field: 'processName',
          minWidth: 120,
          headerTooltip:  gridData["process"]
        },
        {
          headerName: gridData["productionorder"],
          field: 'soNo',
          minWidth: 160,       
          "cellRendererFramework": CellSOViewComponent,
          headerTooltip: gridData["productionorder"]
        },
        {
          headerName: gridData["workorder"],
          field: 'poNo',
          minWidth: 140,       
          "cellRendererFramework": CellPOViewComponent,
          headerTooltip:  gridData["workorder"]
        },

        { headerName: gridData["style"], field: "style" , headerTooltip: gridData["style"],minWidth:120 },
        { headerName: gridData["customer"], field: "customer", headerTooltip: gridData["customer"],minWidth: 120 },
        { headerName: gridData["quantity"], field: "poQty",  headerTooltip: gridData["quantity"],minWidth: 120},
        { headerName: gridData["completedQty"], field: "completedQty", headerTooltip: gridData["completedQty"],minWidth: 120},
        { headerName: gridData["completion%"], field: "completedPer",headerTooltip: gridData["completion%"],minWidth: 120,
        "cellRendererFramework": OrderListProgressBarComponent,tooltipValueGetter:  (params) => {
          return params.value+'%';
        }
      },
        {  headerName: gridData['received'], field: "entryDate",headerTooltip: gridData["received"],
          cellRenderer: (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },minWidth: 120,
        },
        {
          headerName: gridData["exFactory"], field: "exFactory",headerTooltip: gridData["exFactory"],minWidth: 120,
          cellRenderer: (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          },tooltipValueGetter:  (params) => {
            return moment.utc(params.value).format('DD/MM/YY')
          }
        },
        {
          headerName: gridData["action"],headerTooltip: gridData["action"],
          field: 'poqty',
          minWidth: 120,
          "cellRendererFramework": CellCustomComponent,
          filter: false
        }
      ];

    }
    this.defaultColDef =
    { resizable: true,
       filter: 'agTextColumnFilter',
       tooltipValueGetter: (params) => {
    return params.value;

      } };
    this.loadScripts();
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.orderstatus = 1;
    this._Activatedroute.paramMap.subscribe(params => {
      this.pono = params.get('pono');
    });
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    }
  }

  POView(pono: any) {
    this.router.navigate(['/User/POView/' + pono]);
  }

  SOView(sono: any) {
    this.router.navigate(['/User/SOView/' + sono]);
  }

  ngOnInit() {
    this.ordermodel = new OrderModel();
    this.OrderService.GetOrderStatusList().subscribe(
      res => {
        res.data.unshift({ id: -1, text: 'All' });
        this.ordermodel.orderstatuslist = res.data;
      })
    this.OrderList();
    this.RequestStatusList();
  }

  onOLGridReady(params) {
    this.OLgridApi = params.api;
    this.OLgridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();
  }
  onRSGridReady(params) {
    this.RSgridApi = params.api;
    this.RSgridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();
  }

  OrderList() {
    if (isNaN(this.orderstatus)) {
      this.orderstatus = 1;
    }
    this.ordermodel.orderStatus = this.orderstatus;
    this.ordermodel.poNo = this.pono;
    this.OrderService.OrderList(this.ordermodel).subscribe(
      res => {
        this.rowData = res.data;
      });
  }

  RequestStatusList() {
    if (isNaN(this.orderstatus)) {
      this.orderstatus = 1;
    }
    this.ordermodel.orderStatus = this.orderstatus;
    this.ordermodel.poNo = this.pono;
    this.OrderService.RequestStatusList().subscribe(
      res => {
        this.requeststatus_rowData = res.data;
      })
  }

  statusChangeHandler(event: any) {
    this.orderstatus = parseInt(event);
    this.OrderList();
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

  syncData() {
    let RSSelectedRows = this.RSgridApi.getSelectedRows();
    if(RSSelectedRows.length>0){
      this.syncbtndisable = true;
    }
    this.requeststatusmodel = RSSelectedRows;
    if (RSSelectedRows.length==0) {
      alert("Please select RequestID");
    }
    if (RSSelectedRows.length>0) {
      this.OrderService.SyncData(this.requeststatusmodel).subscribe(
        res => {
          this.requeststatusmodel = res.data;
          alert(res.message);
          if(res.status==200){
            window.location.reload();
          }
          else{
            this.syncbtndisable = false
            this.orderstatusvisiblity=false;
          }
        });
    }
  }
  onTabClick(event){
    var target = event.target || event.srcElement || event.currentTarget;
    var idAttr = target.id;
    if (idAttr == 'orderlistTab') {
      this.selectedTab = 'orderlistTab';
      this.syncspanvisiblity=true;
      this.orderstatusvisiblity=true;
    }
    if (idAttr == 'requeststatusTab'){
      this.selectedTab = 'requeststatusTab';
      this.syncspanvisiblity=false;
      this.orderstatusvisiblity=false;

    }
  }
 get disableClick(){
return this.syncbtndisable
  }
}



