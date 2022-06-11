import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { OrderModel } from '../../models/OrderModel';

@Component({
  selector: 'app-order',
  templateUrl: './cell-poview.component.html'
})

export class CellPOViewComponent implements OnInit {
  public ordermodel: OrderModel;
  constructor(
    private router: Router,
    private OrderService: OrderService
  ) { }
  data: any;
  params: any;
  pono: any;

  agInit(params) {
    // console.log("cell-poview call - " + params);
    this.params = params;
    this.data = this.params.data.poNo;
  }

  ngOnInit() {
  }

  POView() {
    // console.log(this.params.data.poNo);
    //this.params.data.poNo=`$"{TEST1PO1//1018}`;
    this.router.navigate(['/User/POView/'  +this.params.data.poNo +'/'+  this.params.data.soNo ],{ queryParams: { calledFrom: 'FPPO',Page: 'WorkOrder' } });
  }
}

