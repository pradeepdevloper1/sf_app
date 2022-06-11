import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { ActivatedRoute } from '@angular/router';
import { ProductModel } from '../../models/ProductModel';
import { ProductService } from '../services/product.service';
declare function saveproduct(): any;

@Component({
  selector: 'app-admin',
  templateUrl: './productlist.component.html'
})

export class ProductListComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  data: any;
  header: any;
  factoryid: number;
  errorMessage: string;
  public productmodel: ProductModel;

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public gridOptions;

  constructor(
    private router: Router,
    private http: HttpClient,
    private productService: ProductService,
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
      { headerName: 'ProductID', field: "productID", hide: true, },
      { headerName: 'factoryID', field: "factoryID", hide: true, },
  
      { headerName: 'Product', field: "productName", minWidth: 100, editable: true },
    ];

  }
 
  onCellValueChanged(event) {
    //console.log(event) to test it
    event.data.modified = true;
  }
  
  ngOnInit() {
    /*alert(this.factoryid);*/
  }

  SaveProduct() {
    var list = [];
    this.gridApi.forEachNode((rowNode, index) => {
      /*console.log(rowNode.data);*/
      var orderItem = {
        ProductID: parseInt(rowNode.data.productID),
        FactoryID: rowNode.data.factoryID,
        productName: rowNode.data.productName,      }
        list.push(orderItem);
    });
    //var re = JSON.stringify(list);
    //alert(re);

    this.productmodel = new ProductModel();
    this.productService.UpdateProduct(list).subscribe(
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
      .get(this.Url + "Product/GetProduct/" + this.factoryid.toString(), {
        headers: new HttpHeaders().set('Authorization', "Bearer " + this.auth)
      })
      .subscribe((data) => {
        this.rowData = data;
      });

  }
}

