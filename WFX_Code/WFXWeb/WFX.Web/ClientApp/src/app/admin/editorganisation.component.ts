import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrganisationService } from '../services/organisation.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { ActivatedRoute } from '@angular/router';
import { OrganisationModel } from '../../models/OrganisationModel';
import { HttpClient, HttpHeaders } from '@angular/common/http';
@Component({
  selector: 'app-admin',
  templateUrl: './editorganisation.component.html',
})
export class EditOrganisationComponent implements OnInit {
  objForm: FormGroup;
  title = 'Create';
  data = false;
  UserForm: any;
  massage: string;
  selecttedFile = null;
  model: any = {};
  errorMessage: string;
  Url: string;

  organisationid: number;
  organisationname: string;
  organisationlogopath: string;

  public organisationmodel: OrganisationModel;
  constructor(private router: Router,
    private _fb: FormBuilder,
    private _Activatedroute: ActivatedRoute,
    private http: HttpClient,
    private organisationService: OrganisationService)
  {
    this.Url = environment.apiUrl;
    this._Activatedroute.paramMap.subscribe(params => {
      this.organisationid = parseInt(params.get('id'));
    });

  }

  ngOnInit() {
    /*  alert(this.organisationid);*/
    this.organisationname = "";
    this.organisationmodel = new OrganisationModel();
    this.organisationService.OrganisationView(this.organisationid).subscribe(
      res => {
        this.organisationmodel = res;
        console.log(res);
        //alert(this.organisationmodel.organisationName);
        //alert(this.organisationmodel.organisationLogoPath);

        this.organisationname = this.organisationmodel.organisationName;
        this.organisationlogopath = this.organisationmodel.organisationLogoPath;
      })
   
    

  }

  OrganisationFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ORGUpload();

  }

  UpdateOrganisation() {
 
    if (this.organisationname == "") {
      alert("Please enter organisationname");
      return;
    }
    this.organisationmodel = new OrganisationModel();
    this.organisationmodel.organisationID = this.organisationid;
    this.organisationmodel.organisationName = this.organisationname;
    this.organisationmodel.organisationLogoPath = this.organisationlogopath;
    
    var filepath = sessionStorage.getItem("organisationlogopath");
    if (this.organisationlogopath != filepath)
      this.organisationmodel.organisationLogoPath = this.organisationlogopath;
    else { this.organisationmodel.organisationLogoPath = filepath; }
    this.organisationService.UpdateOrganisation(this.organisationmodel).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
          alert("Record save");
          this.router.navigate(['/Admin/AdminDashboard'])
        }
        else if (res.status == 201) {
          alert("Already Exits");
        }
        else {
          this.errorMessage = res.message;
        }
      },
      error => {
        this.errorMessage = error.message;
      });
  };
  ORGUpload() {
    const endpoint = 'Upload/Image/UploadOrganisationLogo';
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        console.log(res)
        if (res.status = 200) {
          sessionStorage.setItem("organisationfilename", res.message);
          sessionStorage.setItem("organisationlogopath", res.path);
        }
      });
  }

};


