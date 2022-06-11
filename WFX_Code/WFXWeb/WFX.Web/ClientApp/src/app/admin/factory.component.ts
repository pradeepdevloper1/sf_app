import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FactoryService } from '../services/factory.service';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { FactoryModel } from '../../models/FactoryModel';
/*import { debug } from 'util';*/
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-admin',
  templateUrl: './factory.component.html',
})
export class FactoryComponent implements OnInit {

  objForm: FormGroup;
  title = 'Create';
  data = false;
  UserForm: any;
  massage: string;
  model: any = {};
  submitted: boolean = false;
  public factorymodel: FactoryModel;
  errorMessage: string;
  Url: string;

  organisationid: number;
  clusterid: number;
  factorytype: string;
  linkedwithERP:string;
  country: string;
  timezone: string;
  countryid: number;
  factoryname: string;
  factoryaddress: string;
  noofline: number;
  smartlines: number;
  linkedwitherplist: any;

  constructor(private router: Router,
    private _fb: FormBuilder,
    private _Activatedroute: ActivatedRoute,
   private FactoryService: FactoryService)
    {
    this.Url = environment.apiUrl;
    this.loadScripts();
    this._Activatedroute.paramMap.subscribe(params => {
      this.clusterid = parseInt(params.get('id'));
    });
  }

  ngOnInit() {
    this.organisationid = 0;
    this.countryid = 0;
    this.clusterid = 0;
    this.factorytype = "";
    this.linkedwithERP = "";
    this.factoryname = "";
    this.factoryaddress = "";
    this.timezone = "";
    this.noofline = 0;
    this.smartlines = 0;
    this.factorymodel = new FactoryModel();
    this.factorymodel.clusterID = this.clusterid;
    this.FactoryService.GetOrganisationID(this.clusterid).subscribe(
      res => {
        this.organisationid = res;
     
      })
    this.factorymodel.organisationID = this.organisationid;

    this.FactoryService.FillOrganisation().subscribe(
      res => {
        this.factorymodel.organisationlist = res.data
        console.log(res.data);
      })

    this.FactoryService.FillCountry().subscribe(
      res => {
        this.factorymodel.countrylist = res.data
        console.log(res.data);
      })

    this.FactoryService.FillFactoryType().subscribe(
      res => {
        this.factorymodel.factorytypelist = res.data;
        this.linkedwitherplist = [{id:"YES",text:"YES"},{id:"NO",text:"NO"}];
        this.factorymodel.linkedwitherplist = this.linkedwitherplist;
        console.log(res.data);
      })
  }

  organisationmodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != "") {
      this.organisationid = event;
     
      this.ClusterList();
    }
    else {
      this.organisationid = 0;
    }
  }

  ClusterList() {
    //alert("ClusterList");
    this.FactoryService.FillCulster(this.organisationid).subscribe(
      res => {
        this.factorymodel.clusterlist = res.data
        console.log(res.data);
      })
  }

  clustermodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != 0) {
      this.clusterid = event;
     /* alert(this.clusterid)*/
    }
    else {
      this.clusterid = 0;
    }
  }

  factorytypemodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != "") {
      this.factorytype = event;
    /*  alert(this.factorytype);*/

    }
    else {
      this.factorytype = "";
    }
  }
  linkedwitherpmodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != 0) {
      this.linkedwithERP = event;
    }
    else {
      this.linkedwithERP = "";
    }
  }
  countrymodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != 0) {
      this.countryid = event;

      this.TimeZoneList();
    }
    else {
      this.countryid = 0;
    }
  }

  TimeZoneList() {
  
    this.FactoryService.FillTimeZone(this.countryid).subscribe(
      res => {
        this.factorymodel.timezonelist = res.data
        console.log(res.data);
      })
  }

  timezonemodelChangeHandler(event: any) {
    console.log('value changed: ' + event);
    if (event != "") {
      this.timezone = event;
      /*alert(this.timezone);*/

    }
    else {
      this.timezone = "";
    }
  }
  keyPressNumbers(event) {
    var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    }
    return true;
  }
  SaveFactory() {
    if (this.organisationid == 0) {
      alert("Please select organisation");
      return;
    }
    if (this.clusterid == 0) {
      alert("Please select cluster");
      return;
    }

    if (this.factorytype == "") {
      alert("Please select factoryType");
      return;
    }
    if (this.linkedwithERP == "") {
      alert("Please select Linked With ERP");
      return;
    }
    if (this.factoryname == "") {
      alert("Please enter factory name");
      return;
    }
    if (this.factoryaddress == "") {
      alert("Please enter factory address");
      return;
    }
    if (this.noofline == 0) {
      alert("Please enter no of line");
      return;
    }
    if (this.smartlines == 0) {
      alert("Please enter smart lines");
      return;
    }
    if (this.countryid ==0) {
      alert("Please select country");
      return;
    }
    if (this.timezone == "") {
      alert("Please enter time zone");
      return;
    }

    this.factorymodel = new FactoryModel();
    this.factorymodel.clusterID = Number(this.clusterid);
    this.factorymodel.factoryName = this.factoryname;
    this.factorymodel.factoryAddress = this.factoryaddress;
    this.factorymodel.factoryType = this.factorytype;
    this.factorymodel.linkedwithERP=this.linkedwithERP;
    this.factorymodel.factoryHead = "-";
    this.factorymodel.factoryEmail = "-";
    this.factorymodel.factoryContactNumber =0;
    this.factorymodel.factoryCountry = Number(this.countryid);
    this.factorymodel.factoryTimeZone = this.timezone;
    this.factorymodel.noOfShifts = 0;
    this.factorymodel.decimalValue = 0;
    this.factorymodel.ptmPrice = 0;
    this.factorymodel.noOfUsers = 0;
    this.factorymodel.factoryOffOn = "-";
    this.factorymodel.measuringUnit = "-";
    this.factorymodel.dataScale = 0;
    this.factorymodel.noOfLine = Number(this.noofline);
    this.factorymodel.smartLines = Number(this.smartlines);
    this.FactoryService.Save(this.factorymodel).subscribe(
      res => {
        /*        debugger;*/
        if (res.status == 200) {
          alert("Record save");
          this.router.navigate(['/Admin/FactoryList'])
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

  // Method to dynamically load JavaScript
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
 
};


