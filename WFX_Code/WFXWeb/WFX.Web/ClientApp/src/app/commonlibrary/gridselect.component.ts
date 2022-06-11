import {
  Component, Renderer2, ViewChild, ViewContainerRef,
} from "@angular/core";
import { ICellEditorAngularComp } from "@ag-grid-community/angular";


@Component({
  selector: "gridSelect",
  template: `
      <div class="custom-input" [style.width]=columnWidth>
        <mat-form-field>
          <mat-select #select [multiple]="multiple" [(ngModel)]="selectedValues">
            <mat-option *ngFor="let item of optionList" [value]="item[this.ddlValueCode]">
              {{item[this.ddlValueText]}}
            </mat-option>
          </mat-select>
        </mat-form-field>
      </div>
  `,
})
export class GridSelectComponent implements ICellEditorAngularComp {
  private params: any;
  optionList: any;
  columnWidth = '';
  @ViewChild('Select', { read: ViewContainerRef, static: true }) public input: any;
  constructor(private rd: Renderer2) { }
  public selectedValues: any = [];
  multiple = false;
  ddlValueText = 'text';
  ddlValueCode = 'id';

  agInit(params: any): void {
    this.params = params;
    this.columnWidth = this.params.column.actualWidth - 2 + 'px';
    if (params && params.values.length) {
      this.optionList = this.params.values;
    }
    if (params && typeof params.multiple === 'boolean') {
      this.multiple = params.multiple;
    }
    if (params && typeof params.ddlValueText === 'string') {
      this.ddlValueText = params.ddlValueText;
    }
    if (params && typeof params.ddlValueCode === 'string') {
      this.ddlValueCode = params.ddlValueCode;
    }
    if (this.multiple) {
      if (params.value && !Array.isArray(params.value)) {
        this.selectedValues = [params.value];
      } else {
        this.selectedValues = params.value;
      }
    } else {
      this.selectedValues = params.value;
    }
    // if (selected && ((Array.isArray(selected)) || (selected !== ''))) {
    //   if(this.multiple) {
    //     for (let i of selected) {
    //       this.selectedValues.push(this.optionList.find((element: any) => element[this.ddlValueText] === i)[this.ddlValueCode]);
    //     }
    //   } else {
    //     this.selectedValues = this.optionList.find((element: any) => element[this.ddlValueText] === selected)[this.ddlValueCode];
    //   }
    // }
  }

  ngAfterViewInit(): void {
  }

  getValue() {
    return this.selectedValues;
    // if((!Array.isArray(this.selectedValues) && this.selectedValues) || (Array.isArray(this.selectedValues) && this.selectedValues.length)) {
    //   if(this.multiple) {
    //     let arrSelectedIds: any = [];
    //     for (let i of this.selectedValues) {
    //       arrSelectedIds.push(this.optionList.find((element: any) => element[this.ddlValueText] === i)[this.ddlValueCode])
    //     }
    //     return arrSelectedIds;
    //   } else {
    //     return this.optionList.find((element: any) => element[this.ddlValueText] === this.selectedValues)[this.ddlValueCode];
    //   }
    // }
  }

  isPopup(): boolean {
    return true;
  }
}
