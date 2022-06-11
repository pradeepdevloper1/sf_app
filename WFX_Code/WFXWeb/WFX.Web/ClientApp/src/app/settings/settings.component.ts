import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';


@Component({
  selector: 'app-setting',
  templateUrl: './settings.component.html'
})

export class SettingsComponent implements OnInit {
  constructor(private router: Router) { }

  ngOnInit(): void {

  }
  uploadOBClicked() {
    this.router.navigate(['/User/UploadOB'])
  }
}
