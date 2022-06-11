import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { ActivatedRoute } from '@angular/router';
import { ShiftModel } from '../../models/ShiftModel';
import { ShiftService } from '../services/shift.service';
@Component({
  selector: 'app-admin',
  templateUrl: './shiftlist.component.html'
})

export class ShiftListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  factoryid: number;
  errorMessage: string;
  public shiftmodel: ShiftModel;
  flag: boolean;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public gridOptions;

  constructor(
    private router: Router,
    private http: HttpClient,
    private shiftService: ShiftService,
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
      { headerName: 'ShiftID', field: "shiftID", hide: true, },
      { headerName: 'ModuleID', field: "moduleID", hide: true, },
      { headerName: 'Shift ', field: "shiftName", minWidth: 100, editable: true },
      { headerName: 'Start Time', field: "shiftStartTime", minWidth: 100, editable: true},
      { headerName: 'End Time', field: "shiftEndTime", minWidth: 100, editable: true },
    ]; 
  }

  onCellValueChanged(event) {
    //console.log(event) to test it
    event.data.modified = true;
  }

  ngOnInit() {
    /*alert(this.factoryid);*/
  }

  SaveShift() {
    var list = [];
    this.flag=true;
    
    this.gridApi.forEachNode((rowNode, index) => {
      if(!this.shiftStartTime(rowNode.data.shiftStartTime)|| !this.shiftStartTime(rowNode.data.shiftEndTime))
      {
        this.flag=false;
        return false ;
      };
      var shift = {
        ShiftID: parseInt(rowNode.data.shiftID),
        ModuleID: rowNode.data.moduleID,
        ShiftName: rowNode.data.shiftName,
        ShiftStartTime:rowNode.data.shiftStartTime,
        ShiftEndTime: rowNode.data.shiftEndTime,
      }
      list.push(shift);
    });
    //var re = JSON.stringify(list);
    //alert(re);
    // return;
    if(this.flag){
    this.shiftmodel = new ShiftModel();
    this.shiftService.UpdateShift(list).subscribe(
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
      .get(this.Url + "Shift/GetShiftList/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

  }

  shiftStartTime(time):any {
    const r1 = new RegExp('([01][0-9]|2[0-3]):[0-5][0-9]');
    if (!r1.test(time)) {
      alert("Time Format should be HH:mm");
      return false;
    }
    return true;

  }
}

