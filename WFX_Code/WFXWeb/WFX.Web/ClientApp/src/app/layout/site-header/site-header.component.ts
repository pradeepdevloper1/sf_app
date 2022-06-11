import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.css']
})

export class SiteHeaderComponent implements OnInit {

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


  onChangeEvent(event: any) {
    console.log(event.target.value);
    this.globalsearchtext = event.target.value;
  }

  GlobalSearch() {
    //alert(this.globalsearchtext);
    this.router.navigate(['/User/OrderList', this.globalsearchtext]);
  }

  logout() {
    sessionStorage.clear();
    this.router.navigate(['']);
  };

}
