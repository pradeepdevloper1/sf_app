import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { CulsterService } from '../services/culster.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin',
  templateUrl: './clusterlist.component.html'
})

export class ClusterListComponent implements OnInit {

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
    private clusterservice: CulsterService
  )

  {
    this.columnDefs = [
      { headerName: 'ClusterID', field: "clusterID", hide: true, },
      { headerName: 'Cluster', field: "clusterName", minWidth: 100 },
      { headerName: 'Organisation', field: "organisationName", minWidth: 100 },
      { headerName: 'Factory Count', field: "factoryCount", minWidth: 100 },
      { headerName: '', field: "clusterID", minWidth: 100, cellRenderer: params => "<a href ='/Admin/EditCulster/" + params.value + "' ><img src='../../assets/images/icon-edit-master.svg'></a>" },

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
      .get(this.Url + "Cluster/GetCulsterListView", {
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

