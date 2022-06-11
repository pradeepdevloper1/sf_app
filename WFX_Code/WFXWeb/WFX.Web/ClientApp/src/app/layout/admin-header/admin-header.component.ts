import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'admin-header',
  templateUrl: './admin-header.component.html',
  styleUrls: ['./admin-header.component.css']
})
export class AdminHeaderComponent implements OnInit {

 UserName: string;
 globalsearchtext: string;
    constructor(
   private router: Router,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
  }

  ngOnInit(): void {
  }


}
