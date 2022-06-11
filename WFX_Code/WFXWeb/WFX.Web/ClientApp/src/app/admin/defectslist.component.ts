import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { DefectsModel } from '../../models/DefectsModel';
import { DefectsService } from '../services/defects.service';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-admin',
  templateUrl: './defectslist.component.html'
})

export class DefectsListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  factoryid: number;
  errorMessage: string;
  public defectsmodel: DefectsModel;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public gridOptions;

  constructor(
    private router: Router,
    private http: HttpClient,
    private defectsService: DefectsService,
    private _Activatedroute: ActivatedRoute
  ) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
    this._Activatedroute.paramMap.subscribe(params => {
      this.factoryid = parseInt(params.get('id'));
    });


    this.columnDefs = [
      { headerName: 'DefectID', field: "defectID", hide: true, },
      { headerName: 'DepartmentID', field: "departmentID", hide: true, },
      { headerName: 'Code ', field: "defectCode", minWidth: 100, editable: true },
      { headerName: 'Name', field: "defectName", minWidth: 100, editable: true },
      { headerName: 'Type', field: "defectType", minWidth: 100, editable: true },
      { headerName: 'Level', field: "defectLevel", minWidth: 100, editable: true },
    ];
    this.defaultColDef = { resizable: true, filter: 'agTextColumnFilter' };
  }


  onCellValueChanged(event) {
    //console.log(event) to test it
    event.data.modified = true;
  }


  ngOnInit() {
    /*alert(this.factoryid);*/
  }

  SaveDefects() {
    var list = [];
    this.gridApi.forEachNode((rowNode, index) => {
      /*console.log(rowNode.data);*/
      var defects = {
        DefectID: parseInt(rowNode.data.defectID),
        DepartmentID: rowNode.data.departmentID,
        DefectCode: rowNode.data.defectCode,
        DefectName: rowNode.data.defectName,
        DefectType: rowNode.data.defectType,
        DefectLevel: rowNode.data.defectLevel,
      }
      list.push(defects);
    });
    //var re = JSON.stringify(list);
    //alert(re);

    this.defectsmodel = new DefectsModel();
    this.defectsService.UpdateDefects(list).subscribe(
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
      .get(this.Url + "Defects/GetDefectsList/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

  }
}

