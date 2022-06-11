import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { OrderModel } from '../../models/OrderModel';

@Component({
  selector: 'app-order',
  templateUrl: './cell-soview.component.html'
})

export class CellSOViewComponent implements OnInit {
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
    this.data = this.params.data.soNo;
  }

  ngOnInit() {
  }

  SOView() {
    // console.log(this.params.data.soNo);
    this.router.navigate(['/User/POView/' +  this.params.data.soNo ] ,{ queryParams: { calledFrom: 'ProductionOrder' } });
  }
}

