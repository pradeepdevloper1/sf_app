import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { UserModel } from '../../models/UserModel';
import { UserService } from '../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { GridSelectComponent } from '../commonlibrary/gridselect.component';
import { UserDashboardService } from '../services/userdashboard.service';
import { CommonFunctions } from '../commonlibrary/commonfunctions';


@Component({
  selector: 'app-admin',
  templateUrl: './userlist.component.html'
})

export class UserListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  factoryid: number;
  errorMessage: string;
  public usermodel: UserModel;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public gridOptions;
  frameworkComponents = {
    GridSelectComponent: GridSelectComponent,

  }
  moduleList: any;
  userRoleList: any;
  constructor(
    private router: Router,
    private http: HttpClient,
    private UserDashboardService: UserDashboardService,
    private userService: UserService,
    private _Activatedroute: ActivatedRoute,
    private cf: CommonFunctions
  ) {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
    this._Activatedroute.paramMap.subscribe(params => {
      this.factoryid = parseInt(params.get('id'));
    });


  }

  onCellValueChanged(event) {
    event.data.modified = true;

  }
  ngOnInit() {


    this.createColDef();
    /* alert(this.factoryid);*/
  }

  SaveUser() {
    var list = [];

    this.gridApi.forEachNode((rowNode, index) => {
      let tbl_UserModules = [];
      let tbl_FactoryUserRoles = [];
      let arrModuleID = [];
      let arrUserRoleID = [];
      if (rowNode.data.ModuleID && rowNode.data.ModuleID !== null) {
        arrModuleID = rowNode.data.ModuleID;
        for (let i of arrModuleID) {
          let moduleData = {
            FactoryID: this.factoryid,
            ModuleID: i
          }
          tbl_UserModules.push(moduleData);
        }
      }
      if (rowNode.data.userRoleID && rowNode.data.userRoleID !== null) {

        arrUserRoleID = rowNode.data.userRoleID;
        for (let i of arrUserRoleID) {
          let userRoleData = {
            FactoryID: this.factoryid,
            UserRoleID: i
          }
          tbl_FactoryUserRoles.push(userRoleData);
        }
      }
      var defects = {
        UserID: parseInt(rowNode.data.UserID),
        FactoryID: rowNode.data.factoryID,
        UserFirstName: rowNode.data.userFirstName,
        UserLastName: rowNode.data.userLastName,
        UserName: rowNode.data.userName,
        Password: rowNode.data.password,
        tbl_FactoryUserRoles: tbl_FactoryUserRoles,
        tbl_UserModules: tbl_UserModules,
        UserType: rowNode.data.userType,
        UserEmail: rowNode.data.userEmail,
      }
      list.push(defects);
    });
    //var re = JSON.stringify(list);
    //alert(re);
    this.usermodel = new UserModel();
    this.userService.UpdateUser(list).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
          alert("Record save");
          this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
        }
        else {
          alert(res.message);
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
      .get(this.Url + "User/GetUserList/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((res) => {
        this.rowData = res;
      })
  }

  createColDef() {
    this.userService.FillUserListModule(this.factoryid).subscribe(
      res => {
        this.moduleList = res.data;
        this.userService.FillUserRole().subscribe(
          res => {
            this.userRoleList = res.data;
            this.columnDefs = [
              { headerName: 'UserID', field: "UserID", hide: true, },
              { headerName: 'FactoryID', field: "factoryID", hide: true, },
              { headerName: 'First Name', field: "userFirstName", minWidth: 100, editable: true },
              { headerName: 'Last Name', field: "userLastName", minWidth: 100, editable: true },
              { headerName: 'User Name', field: "userName", minWidth: 100, editable: true },
              { headerName: 'Password', field: "password", minWidth: 100, editable: true, },
              {
                headerName: 'UserRole', field: "userRoleID", minWidth: 100, editable: true, cellEditor: 'GridSelectComponent',
                cellEditorParams: {
                  multiple: true,
                  values: this.userRoleList
                },
                valueFormatter: params => {
                  return this.cf.lookupValue(params, this.userRoleList, params.value);
                },
              },
              {
                headerName: 'Module', field: "ModuleID", minWidth: 100, editable: true, cellEditor: 'GridSelectComponent',
                cellEditorParams: {
                  multiple: true,
                  values: this.moduleList
                },
                valueFormatter: params => {
                  return this.cf.lookupValue(params, this.moduleList, params.value);
                },
              },
              { headerName: 'Type', field: "userType", minWidth: 100, editable: true },
              { headerName: 'Email', field: "userEmail", minWidth: 100, editable: true },

            ];
          })
      });
  }
}

