import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';

import { ActivatedRoute } from '@angular/router';
declare function importExcel(): any;
declare function saveproduct(): any;

@Component({
  selector: 'app-admin',
  templateUrl: './uploadproduct.component.html'
})

export class UploadProductComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;

  constructor(
    private router: Router,
    private http: HttpClient,
    private _Activatedroute: ActivatedRoute
  )
    {
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));

    this.columnDefs = [
      { headerName: 'ProductID', field: "ProductID", hide: true, },
      { headerName: 'Product', field: "productName", minWidth: 100 },
    ];
  }

  
  ngOnInit() {
   
  }

  ProductFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ProductUpload();
  }

  SaveProduct() {
    var res = saveproduct();
    alert(res.message);
    if (res.status == 200) {
      this.router.navigate(['/Admin/FactoryWizard'])
     
    }
  }

  ProductUpload() {
    const endpoint = 'Upload/AdminExcel/UploadProduct/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("productexcelfilename", res.message);
          sessionStorage.setItem("productexcelfilepath", res.path);
          importExcel();
          /*alert(res.message);*/
          if (res.status == 200) {

          }
        }
      });
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
      .get(this.Url + "Product/GetProduct", {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

    //window.addEventListener("resize", function () {
    //  setTimeout(function () {
    //    params.api.sizeColumnsToFit();
    //  });
    //});
  }
}

