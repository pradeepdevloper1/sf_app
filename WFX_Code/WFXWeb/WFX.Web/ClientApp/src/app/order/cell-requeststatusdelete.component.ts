import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { RequestStatusModel } from '../../models/RequestStatusModel';

@Component({
  selector: 'app-order',
  templateUrl: './cell-requeststatusdelete.component.html',
})
export class CellRequeststatusdeleteComponent implements OnInit {
  public requeststatusmodel: RequestStatusModel;
  constructor(
    private router: Router,
    private OrderService: OrderService) { }
  data: any;
  params: any;

  agInit(params) {
    this.params = params;
    this.data = this.params.data.requestid;
  }

  ngOnInit(): void {
  }

  RequestStatusDelete() {
    var qcRequestID = this.params.data.qcRequestID;
    this.requeststatusmodel = new RequestStatusModel();
    this.requeststatusmodel.QCRequestID = qcRequestID;
    this.OrderService.DeleteRequestStatus(this.requeststatusmodel).subscribe(
      res => {
        if (res.status == 200) {
          window.location.reload();
        }
      }) 
   }
}
