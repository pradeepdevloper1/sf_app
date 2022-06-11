import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrganisationService } from '../services/organisation.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
/*import { debug } from 'util';*/
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OrganisationModel } from '../../models/OrganisationModel';
@Component({
  selector: 'app-admin',
  templateUrl: './organisation.component.html',
})
export class OrganisationComponent implements OnInit {
  selecttedFile = null;
  submitted: boolean = false;
  objForm: FormGroup;
  title = 'Create';
  data = false;
  UserForm: any;
  massage: string;

  model: any = {};
  errorMessage: string;
  Url: string;
  organisationname: string;
  public organisationmodel: OrganisationModel;

  constructor(private router: Router,
    private _fb: FormBuilder,
    private http: HttpClient,
    private OrganisationService: OrganisationService) {
    this.organisationmodel = new OrganisationModel();
    this.Url = environment.apiUrl;
  }

  ngOnInit() {
    this.organisationmodel = new OrganisationModel();
    this.organisationname = "";
    sessionStorage.setItem("organisationlogopath", '')
    //this.objForm = this._fb.group({
    //  OrganisationName: ['', [Validators.required]],
    //})
    //this.objForm
  }

  //ORG Logo Upload
OrganisationFileInput(event) {
    console.log(event);
    this.selecttedFile = <File>event.target.files[0];
    this.ORGUpload();
   
  }

  SaveOrganisation() {
    /*    debugger;*/
    if (this.organisationname == "")
    {
      alert("Please enter organisationname");
      return;
    }
 
    this.organisationmodel = new OrganisationModel();
    this.organisationmodel.organisationName = this.organisationname;
   
    var filepath = sessionStorage.getItem("organisationlogopath");

    if (filepath == '')
      this.organisationmodel.organisationLogoPath = "-";
    else { this.organisationmodel.organisationLogoPath = filepath;}

    this.OrganisationService.Organisation(this.organisationmodel).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
            alert("Record save");
          this.router.navigate(['/Admin/AdminDashboard'])
        }
        else if (res.status == 201)
        {
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


