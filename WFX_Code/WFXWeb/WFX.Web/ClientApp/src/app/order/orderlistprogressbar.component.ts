import { Component, Inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-order',
  templateUrl: './orderlistprogressbar.component.html'
})

export class OrderListProgressBarComponent implements OnInit {
  constructor(

  ) { }
  data: any;
  params: any;
  pono: any;
  WidthPer: any;

  agInit(params) {
    this.params = params;
    this.data = this.params.data.completedPer;
    this.WidthPer=this.params.data.completedPer+'%'
  }

  ngOnInit() {

  }

}

