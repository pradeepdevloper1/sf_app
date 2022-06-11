import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})

export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  title = 'Create';

  data = false;
  UserForm: any;
  massage: string;

  model: any = {};
  errorMessage: string;
  Url: string;
  constructor(private router: Router, private _fb: FormBuilder, private LoginService: LoginService) {
    this.Url = environment.apiUrl;
  }

  ngOnInit() {
    this.loginForm = this._fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });

    if (localStorage.getItem('module')!=null || localStorage.getItem('line')!=null|| localStorage.getItem('shift')!=null) {
     this.router.navigate(['/Dashboard']);
    }
  }

  login() {
    this.LoginService.Login(this.loginForm.value).subscribe(
      res => {
        if (res.status == 200) {      
          localStorage.setItem('apiurl', this.Url);
          localStorage.setItem('userID', res.data["userID"]);
          localStorage.setItem('factoryID', res.data["factoryID"]);
          localStorage.setItem('userName', res.data["userName"]);
          localStorage.setItem('userFirstName', res.data["userFirstName"]);
          localStorage.setItem('userLastName', res.data["userLastName"]);
          localStorage.setItem('auth', res.auth);
          this.router.navigate(['/Dashboard']);
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });
  };
};


