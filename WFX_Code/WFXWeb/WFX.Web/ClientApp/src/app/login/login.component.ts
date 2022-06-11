import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
/*import { debug } from 'util';*/
import { TranslateService } from '@ngx-translate/core';

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
  languagelist: object[];

  constructor(
    private router: Router,
    private _fb: FormBuilder,
    private LoginService: LoginService,
    private translate: TranslateService
  ) {
    translate.setDefaultLang('en');
    this.Url = environment.apiUrl;
  }

  useLanguage(language: string): void {
    //alert(language);
    this.translate.use(language);
    sessionStorage.setItem('lang',language);

  }

  ngOnInit() {
    this.languagelist = [{ id: "en", text: "EN" }, { id: "vie", text: "VIE" }]; //, { id: "de", text: "DE" }
    this.loginForm = this._fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
    })
    sessionStorage.setItem('lang','en');

  }

  login() {
    /*    debugger;*/
    this.LoginService.Login(this.loginForm.value).subscribe(
      res => {
        /*console.log(res);*/
        /*        debugger;*/
        if (res.status == 200) {

          sessionStorage.setItem('apiurl', this.Url);

          sessionStorage.setItem('userID', res.data["userID"]);
          sessionStorage.setItem('factoryID', res.data["factoryID"]);
          sessionStorage.setItem('userName', res.data["userName"]);
          sessionStorage.setItem('userFirstName', res.data["userFirstName"]);
          sessionStorage.setItem('userLastName', res.data["userLastName"]);
          sessionStorage.setItem('auth', res.auth);
          sessionStorage.setItem('LinkedwithERP', res.data["linkedwithERP"]);

          if (res.data["userRoleID"] == 1) {
            this.router.navigate(['/Admin/AdminDashboard']);
          }
          else if (res.data["userRoleID"] == 2)
            this.router.navigate(['/User/UserDashboard']);
          else
            this.router.navigate(['.']);
          /*          debugger;*/
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });
  };
// logout() {
//     debugger;
//     sessionStorage.clear();
//     this.router.navigate(['']);
//   };
  

};


