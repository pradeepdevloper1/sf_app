import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { OrderModel } from '../../models/OrderModel';

@Component({
  selector: 'app-order',
  templateUrl: './cell-custom.component.html'
})

export class CellCustomComponent implements OnInit {
  public ordermodel: OrderModel;

  constructor(
    private router: Router,
    private OrderService: OrderService
  ) { }
  data: any;
  params: any;
  disabled: boolean=false;
  agInit(params) {
    this.params = params;
    this.data = params.value / 5;
  }

  ngOnInit() {
    if(this.params.data.orderStatus ===2){
      this.disabled=true;
    }
  }

  SOView() {
    //console.log(this.params);
    //alert(this.params.data.poNo);
    /*<!--< a[routerLink]="['/SOView', 1]" > <i class="fa fa-eye " > </i> </a > -->*/
    this.router.navigate(['/User/POView/' + this.params.data.poNo]);
  }

  POComplete() {
    var c = confirm("Are you sure the Order has been Completed?");
    if (c == true) {
    var pono = this.params.data.poNo;
    this.ordermodel = new OrderModel();
    this.ordermodel.poNo = pono;
    this.OrderService.CompletePO(this.ordermodel).subscribe(
      res => {
        if (res.status == 200) {
          /*console.log(res.status);*/
          window.location.reload()
        }
      })}
  }


  PODelete() {
    var source=this.params.data.source;
    if(source=="ERPApp"){
      alert("PO integrated with ERP, deletion not allowed.");
      return;
    }
    var c = confirm("Are you sure you want Delete Order?");
    if (c == true) {
      var pono = this.params.data.poNo;
      /* alert(pono);*/
       this.ordermodel = new OrderModel();
       this.ordermodel.poNo = pono;
       this.OrderService.DeletePO(this.ordermodel).subscribe(
         res => {
           if (res.status == 200) {
             console.log(res.status);
             window.location.reload();

           }
         })
    }

  }
  editScreen() {
    this.router.navigate(['/User/EditOrder/' + this.params.data.poNo])
  }
}

