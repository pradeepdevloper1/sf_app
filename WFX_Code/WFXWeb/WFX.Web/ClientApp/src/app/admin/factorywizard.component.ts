import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { FactoryService } from '../services/factory.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { FactoryViewModel } from '../../models/FactoryViewModel';
import { AdminExcelModel } from '../../models/AdminExcelModel';
declare function importProductExcel(): any;
declare function importProductFitExcel(): any;
declare function importCustomerExcel(): any;
declare function importLineExcel(): any;
declare function importShiftExcel(): any;
declare function importQCCodeExcel(): any;
declare function importUserExcel(): any;
declare function importModuleExcel(): any;
declare function importProcessDefinitionExcel():any;
@Component({
  selector: 'app-admin',
  templateUrl: './factorywizard.component.html',
})
export class FactoryWizardComponent implements OnInit {
  selecttedFile = null;
  objForm: FormGroup;
  title = 'Create';
  data = false;
  UserForm: any;
  massage: string;
  model: any = {};
  errorMessage: string;

  Url: string;
  UserName: string;
  auth: string;
  userID: number;
  header: any;
  factoryid: number;
  public factorymodel: FactoryViewModel;
  public adminexcelmodel: AdminExcelModel;
  constructor(private router: Router,
    private http: HttpClient,
    private _fb: FormBuilder,
    private _Activatedroute: ActivatedRoute,
    private FactoryService: FactoryService
  ) {
    this.Url = environment.apiUrl;
    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
    this._Activatedroute.paramMap.subscribe(params => {
      this.factoryid = parseInt(params.get('id'));
    });
  }

  ngOnInit() {
    this.factorymodel = new FactoryViewModel();
    this.adminexcelmodel = new AdminExcelModel();
    this.factorymodel.factoryID = this.factoryid
    this.FactoryService.FactoryView(this.factorymodel).subscribe(
      res => {
        this.factorymodel = res.data;
        console.log(res.data);
      })
     this.FactoryService.GetExcelUpload(this.factoryid).subscribe(
          res => {
         this.adminexcelmodel = res;

          })
  }

  //Product Upload
  ProductFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ProductUpload();
    //this.router.navigate(['/FactoryWizard/' + this.factoryid]);
  }

  //Product Fit Upload
  ProductFitFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ProductFitUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  //Customer Fit Upload
  CustomerFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.CustomerUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  //Line  Upload
  LineFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.LineUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  //Shift Fit Upload
  ShiftFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ShiftUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  //Defects Fit Upload
  QCCodeFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.QCCodeUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }


  //Module  Upload
  ModuleFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ModuleUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  //User  Upload
  UserFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.UserUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  ProcessDefinitionInput(event) {
    this.selecttedFile = <File>event.target.files[0];
    this.ProcessDefinitionUpload();
    this.router.navigate(['/Admin/FactoryWizard/' + this.factoryid]);
  }

  ProductUpload() {
    const endpoint = 'Upload/AdminExcel/UploadProduct/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("productexcelfilename", res.message);
          sessionStorage.setItem("productexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          var res = importProductExcel();
          console.log(res);
          this.FactoryService.GetExcelUpload(this.factoryid).subscribe(
            res => {
              console.log("res after excel");
              console.log(res);
              this.adminexcelmodel = res;
            })
          //if (res.data == 200) {
          //  this.FactoryService.GetExcelUpload(this.factoryid).subscribe(
          //    res => {
          //      console.log("res after excel");
          //      console.log(res);
          //      this.adminexcelmodel = res;
          //    })
          //}
        }
      });
  }
  ProcessDefinitionUpload(){
    const endpoint = 'Upload/AdminExcel/UploadProcessDefinition/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("processdefinitionexcelfilename", res.message);
          sessionStorage.setItem("processdefinitionexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importProcessDefinitionExcel();
        }
      });  }

  ProductFitUpload() {
    const endpoint = 'Upload/AdminExcel/UploadProductFit/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("productfitexcelfilename", res.message);
          sessionStorage.setItem("productfitexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importProductFitExcel();
        }
      });
  }

  CustomerUpload() {
    const endpoint = 'Upload/AdminExcel/UploadCustomer/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("customerexcelfilename", res.message);
          sessionStorage.setItem("customerexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importCustomerExcel();
        }
      });
  }

  LineUpload() {
    const endpoint = 'Upload/AdminExcel/UploadLine/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("lineexcelfilename", res.message);
          sessionStorage.setItem("lineexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importLineExcel();
        }
      });
  }

  ShiftUpload() {
    const endpoint = 'Upload/AdminExcel/UploadShift/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("shiftexcelfilename", res.message);
          sessionStorage.setItem("shiftexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importShiftExcel();
        }
      });
  }

  QCCodeUpload() {
    const endpoint = 'Upload/AdminExcel/UploadQCCode/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("qccodeexcelfilename", res.message);
          sessionStorage.setItem("qccodeexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importQCCodeExcel();
        }
      });
  }
  ModuleUpload() {

    const endpoint = 'Upload/AdminExcel/ModuleUpload/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("moduleexcelfilename", res.message);
          sessionStorage.setItem("moduleexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importModuleExcel();
        }
      });
  }

  UserUpload() {
    const endpoint = 'Upload/AdminExcel/UploadUser/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("userexcelfilename", res.message);
          sessionStorage.setItem("userexcelfilepath", res.path);
          sessionStorage.setItem("factoryid", this.factoryid.toString());
          importUserExcel();
        }
      });
  }

  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
      'assets/js/w_adminexcel.js',

    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }


};


