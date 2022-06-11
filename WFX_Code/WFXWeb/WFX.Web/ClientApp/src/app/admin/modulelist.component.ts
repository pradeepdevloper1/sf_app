import { Component, Inject, OnInit } from '@angular/core';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { ActivatedRoute } from '@angular/router';
import { ModuleModel } from '../../models/ModuleModel';
import { ModuleService } from '../services/module.service';

declare function saveproduct(): any;


@Component({
  selector: 'app-admin',
  templateUrl: './modulelist.component.html'
})

export class ModuleListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  factoryid: number;
  errorMessage: string;
  public modulemodel: ModuleModel;
  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;

  constructor(
    private router: Router,
    private http: HttpClient,
    private moduleService: ModuleService,
    private _Activatedroute: ActivatedRoute
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

    this.columnDefs = [
      { headerName: 'ModuleID', field: "moduleID", hide: true, },
      { headerName: 'DepartmentID', field: "departmentID", hide: true, },
      { headerName: 'Module Name ', field: "moduleName", minWidth: 100, editable: true },
    ];
    this.defaultColDef = { resizable: true, filter: 'agTextColumnFilter' };
  }
 
  SaveModule() {
    var list = [];
    this.gridApi.forEachNode((rowNode, index) => {
      /*console.log(rowNode.data);*/
      var module = {
        ModuleID: parseInt(rowNode.data.moduleID),
        DepartmentID: rowNode.data.departmentID,
        ModuleName: rowNode.data.moduleName,

      }
      list.push(module);
    });
    //var re = JSON.stringify(list);
    //alert(re);

    this.modulemodel = new ModuleModel();
    this.moduleService.UpdateModule(list).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
          alert("Record save");
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


  ngOnInit() {
    /*alert(this.factoryid);*/
  }
  onCellValueChanged(event) {
    //console.log(event) to test it
    event.data.modified = true;
  }
  
 
  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_product.js',
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
      .get(this.Url + "Module/GetModuleList/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

  }
}

