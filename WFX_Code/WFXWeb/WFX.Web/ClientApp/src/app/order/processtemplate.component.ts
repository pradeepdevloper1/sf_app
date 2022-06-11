import { Component, OnInit } from '@angular/core';

import { TranslateService } from '@ngx-translate/core';
import { OrderService } from '../services/order.service';
import { OrderProcessModel } from 'src/models/OrderProcessModel';
import { ActivatedRoute } from '@angular/router';
import { GridSlideToggleComponent } from '../commonlibrary/gridslidetoggle.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { POViewModel } from 'src/models/POViewModel';



@Component({
  selector: 'app-linebooking',
  templateUrl: './processtemplate.component.html',

})

export class ProcessTemplateComponent implements OnInit {

  columnDefs: any;
  posearchtext: string;
  OrderProcessModel: OrderProcessModel;
  public defaultColDef;
  gridApi: any;
  gridColumnApi: any;
  sono: string;
  rowData: [] = [];
  public poviewmodel: POViewModel;


  frameworkComponents = {
    GridSlideToggleComponent: GridSlideToggleComponent,

  }

  constructor(
    private orderservice: OrderService,
    private translate: TranslateService,
    private _Activatedroute: ActivatedRoute,
    private msg: MatSnackBar,

  ) {
  }

  ngOnInit() {

    let lang = sessionStorage.getItem('lang');
    let gridData = this.translate.store.translations[lang].lang;
    this._Activatedroute.paramMap.subscribe(params => {
      this.sono = params.get('sono');
    }
    );
    this.columnDefs = [
      {
        headerName: 'SNo',
        width: 100,
        cellRenderer: (params) => {
          return params.rowIndex + 1;
        }
      },

      {
        headerName: gridData['processName'],
        field: 'processName',
        minWidth: 200,
      },
      {
        headerName: gridData['processCode'],
        field: 'processCode',
        minWidth: 200
      },
      {
        headerName: gridData['afterProcessName'],
        field: 'afterProcessName',
        minWidth: 300
      },
      {
        headerName: gridData['processEnabled'],
        field: 'processEnabled',
        cellRenderer: 'GridSlideToggleComponent'
      },
    ]
    this.OrderProcessModel = new OrderProcessModel();
    this.OrderProcessModel.SONo = this.sono;
    this.orderservice.GetProcessTemplate(this.OrderProcessModel).subscribe(
      res => {
        this.rowData = res.result;
        console.log
        this.OrderProcessModel = res.result;
        this.loadSearch();
      }
    )
    // this.orderservice.FactoryProcessList(this.poviewmodel).subscribe(
    //   res => {
    //     console.log(res.data)
    //   })
  }

  onChangeEvent(event: any) {
    this.posearchtext = event.target.value;
    this.loadSearch();
  }
  loadSearch() {
    this.poviewmodel = new POViewModel();
    this.poviewmodel.posearchtext = this.posearchtext;
    this.poviewmodel.soNo = this.sono;
    this.orderservice.POListOfSO(this.poviewmodel).subscribe(
      res => {
        this.poviewmodel.polist = res.data;
      })
  }
  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
  }
  saveChanges() {
    this.orderservice.PutProcessTemplate(this.OrderProcessModel).subscribe(
      res => {
        if (res.status === 200) {
          this.msg.open(res.message, 'info', { duration: 2000 });

        }
      }
    )
  }
}
