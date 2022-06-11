import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { OrganisationService } from '../services/organisation.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-admin',
  templateUrl: './organisationlist.component.html'
})

export class OrganisationListComponent implements OnInit {

  UserName: string;
  Url: string;
  header: any;
  auth: string;
  data: any;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject('BASE_URL') baseUrl: string,
    private organisationservice: OrganisationService
  )

  {
    this.columnDefs = [
      { headerName: 'OrganisationID', field: "organisationID", hide: true, },
      { headerName: 'Organisation', field: "organisationName", minWidth: 100 },
      { headerName: 'Cluster Count', field: "clusterCount", minWidth: 100 },
      { headerName: 'Factory Count', field: "factoryCount", minWidth: 100 },
      { headerName: '', field: "organisationID", minWidth: 100, cellRenderer: params => "<a href ='/Admin/EditOrganisation/" + params.value + "' ><img src='../../assets/images/icon-edit-master.svg'></a>" },

    ];
    this.defaultColDef = { resizable: true, filter: 'agTextColumnFilter'};

    this.loadScripts();
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
  }

  ngOnInit() {
    
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();
        this.http
      .get(this.Url + "Organisation/GetOrganisationListView", {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

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

