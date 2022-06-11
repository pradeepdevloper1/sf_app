import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CulsterService } from '../services/culster.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { CulsterModel } from '../../models/CulsterModel';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
/*import { debug } from 'util';*/
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-admin',
  templateUrl: './cluster.component.html',
})
export class CulsterComponent implements OnInit {

  objForm: FormGroup;
  title = 'Create';
  data = false;
  UserForm: any;
  massage: string;
  public culstermodel: CulsterModel;
  model: any = {};
  errorMessage: string;
  Url: string;
  submitted: boolean = false;
  organisationid: number;
  clustername: string;

  constructor(private router: Router,
    private _fb: FormBuilder,
    private _Activatedroute: ActivatedRoute,
    private CulsterService: CulsterService) {
    this.Url = environment.apiUrl;
    this.loadScripts();
    this._Activatedroute.paramMap.subscribe(params => {
      this.organisationid = parseInt(params.get('id'));
    });
    /*alert(this.organisationid);*/
  }
  ngOnInit() {
    this.culstermodel = new CulsterModel();
    this.organisationid = 0;
    this.clustername = "";
    this.CulsterService.FillOrganisation().subscribe(
      res => {
        this.culstermodel.organisationlist = res.data
        console.log(res.data);
      })
  }

  organisationmodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != 0) {
      this.organisationid = event;
      this.clustername == "";

    }
    else {
      this.organisationid = 0;
    }
  }

  SaveCulster() {

    if (this.organisationid == 0) {
      alert("Please select organisation");
      return;
    }

    if (this.clustername == "") {
      alert("Please enter cluster name");
      return;
    }

    this.culstermodel = new CulsterModel();

    this.culstermodel.organisationID = Number(this.organisationid);
    this.culstermodel.clusterName = this.clustername;

    //var re = JSON.stringify(this.culstermodel);
    //alert(re);
    /*return;*/
    this.CulsterService.SaveCulster(this.culstermodel).subscribe(
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
  }
  loadScripts() {
    const dynamicScriptsbody = [
      'assets/js/custom.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
}


