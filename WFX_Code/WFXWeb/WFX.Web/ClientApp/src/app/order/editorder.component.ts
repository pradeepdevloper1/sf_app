import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { OrderService } from '../services/order.service';
import { PurchaseOrderModel } from '../../models/PurchaseOrderModel';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-order',
  templateUrl: './editorder.component.html'
})

export class EditOrderComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  public poviewmodel: PurchaseOrderModel;
  sono: string;
  pono: string;
  userID: number;
  posearchtext: string;
  poimageid: number;
  imagecount: number;
  rows: any;
  newRow: any;
  style: string;
  fit: string;
  product: string;
  salesorderno: string;
  season: string;
  customer: string;
  orderremark: string;
showSaveButton: boolean =false;
  exdate: Date;
  plandate: Date;
  prvpono: string;

  public tableapi;
  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  rowData: any[];
  errorMessage: string;
  shownoneditable:any;
  size: string;

  constructor(
    private router: Router,
    private http: HttpClient,
    private orderservice: OrderService,
    private _Activatedroute: ActivatedRoute,
    private msg: MatSnackBar,

  ) {

    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));

    this.columnDefs = [];

  }

  ngOnInit() {
    this.imagecount = 0;
    this._Activatedroute.paramMap.subscribe(params => {
      this.pono = params.get('pono');
      this.prvpono = params.get('pono');
      this.poviewmodel = new PurchaseOrderModel();
      this.poviewmodel.poNo = this.pono;
      this.loadSearch();
    });
  }

  onAddRow() {
    //alert("Hi");
    //this.gridApi.updateRowData({
    //  add: [{ Color: 'a', Fabric: 'b', XS: '0', S: '0', M: '0', L: '0', XL: '0', XXXL: '0', TotalQty: '0' }]
    //});
    this.gridApi.updateRowData({
      add: [{  }]
    });
  }

  onAddCol() {
    //alert("Hi");
    let newColumn = {
      headerName: "S1",
      field: "S1",
      editable: true
    }
    this.columnDefs.push(newColumn);
    this.gridApi.setColumnDefs(this.columnDefs);
  }

  generateColumns(data: any[]) {
    let columnDefinitions = [];
    let noneditable = this.poviewmodel.source;
    let newColumn = {
      headerName: "",
      field: "HexcodeValue",
      cellRenderer: params => "<input type='color' value='" + params.value +"'>",
      editable: true
    }
    let newColumn1 = {
      headerName: "Hexcode",
      field: "Hexcode",
      editable: true
    }
    columnDefinitions.push(newColumn,newColumn1);

    data.map(object => {
      Object
        .keys(object)
        .map(key => {
          let mappedColumn = {
            headerName: key,
            field: key,
          editable:function(params){
            if(
              noneditable == "ERPApp")
              {
              return false;
            }
            return true; }
          }
          if(mappedColumn.field !=='TotalQty'){
          columnDefinitions.push(mappedColumn);

          }

          //if (key.toString() == "Hexcode") {
          //  let newColumn = {
          //    headerName: key,
          //    field: key,
          //    cellRenderer: params => "<input type='color' ",
          //    editable: true
          //  }
          //  columnDefinitions.push(newColumn);
          //}
          //else {
          //  let mappedColumn = {
          //    headerName: key,
          //    field: key,
          //    editable: true
          //  }
          //  columnDefinitions.push(mappedColumn);
          //}
        })
    })
    columnDefinitions.push({ headerName: 'TotalQty',
    field: 'TotalQty',

    editable:function(params){
      if(
        noneditable == "ERPApp"
        ){
        return false;
      }
      return true; } },
    )

    //Remove duplicate columns
    columnDefinitions = columnDefinitions.filter((column, index, self) =>
      index === self.findIndex((colAtIndex) => (
        colAtIndex.field === column.field
      ))
    )
    return columnDefinitions;
  }


  loadSearch() {
    this.poviewmodel.posearchtext = this.posearchtext;
    this.poviewmodel.soNo = this.sono;
    this.orderservice.PurchaseOrder(this.poviewmodel).subscribe(
      res => {
        console.log("POView Res");
        this.poviewmodel = res.data;
        this.poviewmodel.recvDate = res.data.entryDate;
        this.exdate = this.poviewmodel.exFactory;
        this.plandate = this.poviewmodel.planStDt;
        this.orderservice.POListOfSO(this.poviewmodel).subscribe(
          res => {
            this.poviewmodel.polist = res.data;
            this.shownoneditable =this.poviewmodel.source;
            this.sono = res.sono;
          })
      });

    this.orderservice.PurchaseOrderColorSizeList(this.poviewmodel).subscribe(
      res => {
        this.rowData = res._lst;
        if (this.rowData) {
          this.columnDefs = this.generateColumns(this.rowData);
        }
      })
  }
  onChangeEvent(event: any) {
    this.posearchtext = event.target.value;
    this.loadSearch();
  }

  onCellValueChanged(event) {
    this.showSaveButton=true;
    event.data.modified = true;
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    params.api.sizeColumnsToFit();

  }

  ExDate(val: any) {
    this.showSaveButton=true;
    this.exdate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.onChngeDate()
  }

  onChngeDate(){
    const dateObj = new Date();
    var CurrDate = dateObj;
  }

  PlanDate(val: any) {
    this.showSaveButton=true;
    this.plandate = new Date(moment(val, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    this.onChngeDate()
  }

 async UpdateOrder() {
    var list = [];
    const dateObj = new Date();
    var CurrDate =moment.utc(dateObj.getDate(), 'DD/MM/YYYY');
      if(moment.utc(this.plandate, 'DD/MM/YYYY') > moment.utc(this.exdate, 'DD/MM/YYYY') ){
        this.msg.open("Ex-Factory date cannot be before plan start date", 'Error', { duration: 2000 });
        return false;
    }
    if(moment.utc(this.plandate, 'DD/MM/YYYY')  < CurrDate ){
      this.msg.open("Planned date can't be before than the current date.", 'Error', { duration: 2000 });
      return false;
    }
    if (moment.utc(this.exdate, 'DD/MM/YYYY')  < CurrDate ){
      this.msg.open("Ex-Factory date can't be before than the current date.", 'Error', { duration: 2000 });
      return false;
    }
    var FitData={
      FitType: this.poviewmodel.fit,
    }
    var ProductData={
      ProductName: this.poviewmodel.product,
    }
    var CustomerData={
      CustomerName: this.poviewmodel.customer,
    }
    var OrderData={
      PONo:this.poviewmodel.poNo,
      SONo: this.poviewmodel.soNo,
      OrderID: this.poviewmodel.orderID
    }
if(this.pono !== this.poviewmodel.poNo){
    const p =await this.orderservice.PostIsPOExist(OrderData).toPromise();
    if (p.status == 200) {
      this.msg.open('Order Number already exists in the factory', 'Error', { duration: 2000 });
      return false;
    }
  }

    const t =await this.orderservice.PostIsFitExist(FitData).toPromise();
    if (t.status == 401) {
      this.msg.open('Fit doesn’t exist in the factory', 'Error', { duration: 2000 });
      return false;
    }

    const y =await this.orderservice.PostIsProductExist(ProductData).toPromise();
      if (y.status == 401) {
        this.msg.open('Product doesn’t exist in the factory', 'Error', { duration: 2000 });
        return false;
      }

    const g =await this.orderservice.PostIsCustomerExist(CustomerData).toPromise();
    if (g.status == 401) {
      this.msg.open('Customer doesn’t exist in the factory', 'Error', { duration: 2000 });
      return false;
    }
    this.gridApi.forEachNode((rowNode, index) => {
      var obj = rowNode.data;

      this.size = "";
      var total = 0;
      for (var key of Object.keys(obj)) {
        console.log(key + " -> " + obj[key])
        var str = key.toString();
        if (str == "Color") { continue; }
        else if (str == "Hexcode") { continue; }
        else if (str == "HexcodeValue") { continue; }
        else if (str == "Fabric") { continue; }
        else if (str == "TotalQty") { continue; }
        else if (str == "modified") { continue; }

        else {
          total = total + parseInt(obj[key]);
          if (this.size == "") { this.size =  key + "-" + obj[key]; }
          else { this.size = this.size + "," + key + "-" + obj[key]; }
        }
      }
      //alert(this.size);

      var orderItem = {
        Part: this.prvpono,
        // Part use for prvpono in case inf pono change
        PONo: this.poviewmodel.poNo,
        Style: this.poviewmodel.style,
        Fit: this.poviewmodel.fit,
        Product: this.poviewmodel.product,
        SONo: this.poviewmodel.soNo,
        Season: this.poviewmodel.season,
        Customer: this.poviewmodel.customer,
        OrderRemark: this.poviewmodel.orderRemark,
        //ExFactory: this.poviewmodel.exFactory,
        //PlanStDt: this.poviewmodel.planStDt,
        ExFactory: this.exdate,
        EntryDate: this.poviewmodel.recvDate,

        PlanStDt: this.plandate,
        Color: rowNode.data.Color,
        Hexcode: rowNode.data.Hexcode,
        Fabric: rowNode.data.Fabric,
        Source:'',
        //SizeList: rowNode.data.size1Name + "-" + parseInt(rowNode.data.size1Qty).toString() + "," + rowNode.data.size2Name + "-" + parseInt(rowNode.data.size2Qty).toString() + "," + rowNode.data.size3Name + "-" + parseInt(rowNode.data.size3Qty).toString() + "," + rowNode.data.size4Name + "-" + parseInt(rowNode.data.size4Qty).toString() + "," + rowNode.data.size5Name + "-" + parseInt(rowNode.data.size5Qty).toString() + "," + rowNode.data.size6Name + "-" + parseInt(rowNode.data.size6Qty).toString(),
        //SizeList: "XS-" + rowNode.data.XS.toString() + ",S-" + rowNode.data.S.toString() + ",M-" + rowNode.data.M.toString() + ",L-" + rowNode.data.L.toString() + ",XL-" + rowNode.data.XL.toString() + ",3XL-0",
        SizeList: this.size,
        POQty: total,

        //parseInt(rowNode.data.TotalQty),
      }
      list.push(orderItem);
    });
    //var re = JSON.stringify(list);
    //alert(re);

    this.orderservice.Update(list).subscribe(
      res => {
        /*        debugger;*/
       if (res.status == 200) {
          alert("Record save");
          this.router.navigate(['/User/POView/' + this.poviewmodel.poNo])
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });
  };

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_ob.js',
      'assets/js/custom.js',
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

}

