import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { OrderService } from '../services/order.service';
import { POViewModel } from '../../models/POViewModel';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OBModel } from 'src/models/OBModel';

declare function importExcel(): any;
declare function saveob(pono: string): any;

@Component({
  selector: 'app-order',
  templateUrl: './imgeditor.component.html'
})

export class IMGEditorComponent implements OnInit {
  selecttedFile = null;
  Url: string;
  UserName: string;
  auth: string;
  public poviewmodel: POViewModel;
  sono: string;
  pono: string;
  userID: number;
  posearchtext: string;
  poimageid: number;
  poimageName: string;
  imagecount: number;
  imgno: number;
  title: any = 'obeditorpo';
  showSaveButton: boolean= false;
  OBModel: OBModel;
  updatedRows = [];

  public gridApi;
  public gridColumnApi;
  public columnDefs;
  public defaultColDef;
  public rowData: any;
  public img_ob;
  constructor(
    private translate: TranslateService,
    private router: Router,
    private http: HttpClient,
    private orderservice: OrderService,
    private _Activatedroute: ActivatedRoute,
    private msg: MatSnackBar,
  ) {
    let lang = sessionStorage.getItem('lang');
    let gridData = translate.store.translations[lang].lang
    this.columnDefs = [
      { headerName: gridData['sNo'], field: "sNo" },
      { headerName: gridData['operationCode'], field: "operationCode", editable: true },
      { headerName: gridData['operationName'], field: "operationName", editable: true},
      { headerName: gridData['smv'], field: "smv",  editable: true },
      { headerName: gridData['obLocation'], field: "obLocation",  editable: true},
    ];
    this.defaultColDef = {
      resizable: true,
      minWidth: 80,
      flex: 1,
      filter: true,
    };

    this.loadScripts();
    this.Url = environment.apiUrl;
    this.auth = sessionStorage.getItem('auth');
    this.UserName = sessionStorage.getItem('userFirstName') + " " + sessionStorage.getItem('userLastName').replace("-", "");
    this.userID = parseInt(sessionStorage.getItem('userID'));
  }

  ngOnInit() {
    this.imagecount = 0;
    this._Activatedroute.paramMap.subscribe(params => {
      this.sono = params.get('sono');
      this.poviewmodel = new POViewModel();
      this.poviewmodel.soNo = this.sono;
      this.img_ob = parseInt(params.get('img_ob'));
      this.poviewmodel.picesChecked = this.img_ob;
      if (this.img_ob == 0) {
        this.title = 'imageeditorpo';
      }
      this.getData();
      this.loadSearch();
    });
  }

  getData() {
    this.orderservice.GetPOImages(this.sono).subscribe(
      res => {
        this.poviewmodel.imagelist = res.data;
        this.imagecount = (res.data && res.data.length) || 0;
        if (this.imagecount == 0 || this.imagecount == undefined) {
          this.imgno = 0;
        }
        else {
          this.imgno = (res.data[this.imagecount - 1].poImageID + 1);
        }

      })
    this.orderservice.POOB(this.poviewmodel).subscribe(
      res => {
        this.rowData = res.data;
      })
  }

  loadSearch() {
    this.poviewmodel.posearchtext = this.posearchtext;
    this.poviewmodel.soNo = this.sono;
    this.orderservice.POListOfSO(this.poviewmodel).subscribe(
      res => {
        this.poviewmodel.polist = res.data;
        this.poviewmodel.soNo = res.sono;
      })
  }

  onChangeEvent(event: any) {
    this.posearchtext = event.target.value;
    this.loadSearch();
  }

  SampleFile() {
    window.location.href = 'sample/obsample.xlsx';
  }

  UploadImage(event) {
    if (this.imagecount == 0 || this.imagecount == undefined) {
      this.imgno = 1;
    }
    else {
      this.imgno = this.imgno + 1;
    }
    if (event.target.files[0].type.includes('image')) {
      this.selecttedFile = <File>event.target.files[0];
      const endpoint = "Upload/Image/UploadImage/" + this.imgno + "?SONo=" + this.sono;
      const formData: FormData = new FormData();
      formData.append('file', this.selecttedFile, this.selecttedFile.name);


      this.http.post<any>(endpoint, formData)
        .subscribe(res => {
          if (res.status = 200) {
            sessionStorage.setItem("imgname", res.message);
            sessionStorage.setItem("imgpath", res.path);

            this.poviewmodel.soNo = this.sono;
            this.poviewmodel.imagePath = res.path;
            this.poviewmodel.imageName = res.filename;
            this.poviewmodel.imgno = this.imgno;
            this.orderservice.PostPOImages(this.poviewmodel).subscribe(
              res => {
                this.poviewmodel.imagelist = res.data;
                this.imagecount = res.data.length;
                this.imgno = res.data[this.imagecount - 1].poImageID;
              });
          }
        });
    }
    else {
      alert("Invalid Format ! Please choose Image")
    }
  }

  ChangeImage(event, poimageid, previousimageName: string) {
    this.selecttedFile = <File>event.target.files[0];
    if(this.poimageid==undefined ){
      window.location.reload();
    }
    const endpoint = "Upload/Image/ChangeImage/" + this.poimageid + "/?SONo=" + this.sono+ "&PreviousImageName=" + this.poimageName;
    const formData: FormData = new FormData();
    formData.append('newfile', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status = 200) {
          sessionStorage.setItem("imgname", res.message);
          sessionStorage.setItem("imgpath", res.path);
          this.poviewmodel.poImageID = this.poimageid;
          this.poviewmodel.soNo = this.sono;
          this.poviewmodel.imagePath = res.path;
          this.poviewmodel.imageName = res.filename;
          this.orderservice.PutPOImages(this.poviewmodel).subscribe(
            res => {
              this.poviewmodel.imagelist = res.data;
              this.imagecount = res.data.length;
              this.imgno = res.data[this.imagecount - 1].poImageID;
            });
        }
      });
  }

  deleteImage(poimageid, imageName: string) {
    this.poviewmodel.poImageID = poimageid;
    this.orderservice.DeletePOImage(this.poviewmodel).subscribe(
      res => {
        if (res.status == 200) {
          this.poviewmodel.imagelist = res.data;
          this.imagecount = res.data.length;
          if (this.imagecount > 0) {
            this.imgno = res.data[this.imagecount - 1].poImageID;
          }
          this.loadSearch();
          this.router.navigate(['/User/IMGEditor/' + this.sono + '/' + this.img_ob]);
        }
      });
    const endpoint = "Upload/Image/DeleteImage?SONo=" + this.sono + "&FileName=" + imageName;
    this.http.post<any>(endpoint,{}).subscribe(res => {
      console.log(res);
    });

  }

  OBFileInput(event) {
    // if (event.target.files[0].type.includes('spreadsheetml.sheet')) {
      this.selecttedFile = <File>event.target.files[0];
      this.OBUpload();
    // }
    // else {
    //   alert("Invalid Format ! Please choose xlsx File")
    // }
    this.getData();
  }


  OBUpload() {
    const endpoint = 'Upload/Excel/UploadOB/' + this.userID.toString();
    const formData: FormData = new FormData();
    formData.append('file', this.selecttedFile, this.selecttedFile.name);
    this.http.post<any>(endpoint, formData)
      .subscribe(res => {
        if (res.status == 200) {
          // sessionStorage.setItem("pono", this.pono);
          sessionStorage.setItem("sono",this.sono);
          sessionStorage.setItem("obexcelfilename", res.message);
          sessionStorage.setItem("obexcelfilepath", res.path);
          importExcel();
          // this.poviewmodel.poNo = this.pono;
          this.poviewmodel.soNo = this.sono;
          this.getData();
          this.loadSearch();
          if (res.status == 200) {
            this.router.navigate(['/User/IMGEditor/' + this.sono + '/' + this.img_ob]);
          }
        }
      },
        error => {
          console.log(error);
        }
      );

  }
  clickImage(poimageid,poimageName) {
    this.poimageid = poimageid;
    this.poimageName=poimageName;
  }
  // Method to dynamically load JavaScript
  loadScripts() {
    const dynamicScriptsbody = [
      'https://unpkg.com/@ag-grid-enterprise/all-modules@25.1.0/dist/ag-grid-enterprise.min.js',
      'assets/js/w_ob.js',
      'assets/js/custom.js',
      'assets/js/jquery.fancybox.js',
    ];

    for (let i = 0; i < dynamicScriptsbody.length; i++) {
      const node = document.createElement('script');
      node.src = dynamicScriptsbody[i];
      node.type = 'text/javascript';
      node.async = false;
      document.getElementsByTagName('body')[0].appendChild(node);
    }
  }
  changetitle(event: any) {

    var target = event.target || event.srcElement || event.currentTarget;
    var idAttr = target.attributes.id.nodeValue;

    if (idAttr == 'firsttab') {
      this.title = 'imageeditorpo';

    } else {
      this.title = 'obeditorpo';
    }
  }
  cellValueChanged(e:any){
    this.showSaveButton=true;
    let data = e.data;
    this.OBModel = data;
    const removeIndex = this.updatedRows.findIndex(item => item.OBID == this.OBModel.OBID);
    if (removeIndex != -1) {
      this.updatedRows.splice(removeIndex, 1);
    }
    this.updatedRows.push(this.OBModel);
  }
  async savechanges(){
    const arr=[];
for(let i of this.rowData){
  arr.push(i.operationCode)
}

if(arr.length !== new Set(arr).size){
  this.msg.open('Duplicate Operation codes are not allowed', 'Error', { duration: 2000 });
  return false;
}
console.log(this.rowData)
console.log(this.updatedRows)
var list = [];
for(let i of this.rowData){
  var OBList = {
    OBID: parseInt(i.obid),
    PONo: i.poNo,
    SONo: i.soNo,
    SNo: i.sNo,
    OperationCode: i.operationCode,
    OperationName: i.operationName,
    Section:i.section,
    SMV: parseInt(i.smv),
    UserID:i.userID,
    EntryDate: i.entryDate,
    OBLocation:i.obLocation,
    FactoryID: parseInt(i.factoryID)
 }
    list.push(OBList);
};
console.log(list)
this.orderservice.PutOB(list).subscribe(res => {
  this.msg.open(res.message, 'info', { duration: 2000 });
  this.getData();
})
  }

}

