import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { LineModel } from '../../models/LineModel';
import {LineService } from '../services/line.service';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonFunctions } from '../commonlibrary/commonfunctions';
import { TranslationService } from '../services/translation.service';
import { GridSelectComponent } from '../commonlibrary/gridselect.component';
@Component({
  selector: 'app-admin',
  templateUrl: './linelist.component.html'
})

export class LineListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  errorMessage: string;
  factoryid: number;
  public linemodel: LineModel;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public gridOptions;
  processTypesList:any;
  frameworkComponents = {
    GridSelectComponent: GridSelectComponent
  }

  constructor(
    private router: Router,
    private lineService: LineService,
    private http: HttpClient,
  private _Activatedroute: ActivatedRoute,
  private msg: MatSnackBar,
  private cf: CommonFunctions,
    private ts: TranslationService
  )
    {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
    this._Activatedroute.paramMap.subscribe(params => {
      this.factoryid = parseInt(params.get('id'));
    });
    this.ts.GetData("ProcessTypes").subscribe(
      res => {
        this.processTypesList = res.obj;
    this.columnDefs = [
      { headerName: 'LineID', field: "lineID", hide: true, },
      { headerName: 'ModuleID', field: "ModuleID", hide: true, },
      { headerName: 'Line', field: "lineName", minWidth: 100,editable: true },
      { headerName: 'Process Type', field: "processType", minWidth: 100,editable: true,
      cellEditor: 'GridSelectComponent',
      cellEditorParams: {
        values: this.processTypesList,
        ddlValueText: 'translatedString',
        ddlValueCode: 'objectKey'
      },
      valueFormatter: params => {
        return this.cf.lookupValue(params, this.processTypesList, params.value);
      }, },
      { headerName: 'InternalLine', field: "internalLineName", minWidth: 100 ,editable: true},
      { headerName: 'NoOfMachine', field: "noOfMachine", minWidth: 100 ,editable: true},
      { headerName: 'Capacity', field: "lineCapacity", minWidth: 100,editable: true },
      { headerName: 'LiloadingPoint', field: "lineloadingPoint", minWidth: 100,editable: true },
      { headerName: 'TabletID', field: "tabletID", minWidth: 100, editable: true },
      { headerName: 'DeviceSerialNo', field: "deviceSerialNo", minWidth: 100, editable: true },
      { headerName: 'ModuleName', field: "moduleName", minWidth: 100, editable: true },
    ];
  });
  }

  ngOnInit() {
    /*alert(this.factoryid);*/
  }
 onCellValueChanged(event) {
    //console.log(event) to test it
    event.data.modified = true;
  }
 SaveLine() {
  const arr=[];
  for(let i of this.rowData){
    arr.push(i.tabletID+i.moduleName)
  }
if(this.rowData.length>1){
  if(arr.length !== new Set(arr).size){
    this.msg.open('Modules with same Tablet IDs are not allowed', 'Error', { duration: 2000 });
    return false;
  }
}
    var list = [];
    this.gridApi.forEachNode((rowNode, index) => {
      /*console.log(rowNode.data);*/
      var line = {
        LineID: parseInt(rowNode.data.lineID),
        LineName: rowNode.data.lineName,
        InternalLineName: rowNode.data.internalLineName,
        NoOfMachine: parseInt(rowNode.data.noOfMachine),
        LineCapacity: parseInt(rowNode.data.lineCapacity),
        LineloadingPoint: rowNode.data.lineloadingPoint,
        TabletID: rowNode.data.tabletID,
        ModuleName: rowNode.data.moduleName,
        ProcessType: rowNode.data.processType,
        DeviceSerialNo: rowNode.data.deviceSerialNo,
     }
        list.push(line);
    });
    //var re = JSON.stringify(list);
    //alert(re);

    this.linemodel = new LineModel();
    this.lineService.UpdateLine(list).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
          alert("Record update");
          this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });

  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',

      'assets/js/jquery.fancybox.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();

    this.http
      .get(this.Url + "Line/GetLine/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        console.log(data);
        this.rowData = data;
      });

  }
}

