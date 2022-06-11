import {
  Component,
  QueryList,
  ViewChildren,
  ViewContainerRef
} from "@angular/core";
import { ICellEditorAngularComp } from "@ag-grid-community/angular";
import * as moment from "moment";

@Component({
  selector: "gridDatePicker",
  template: `
    <mat-form-field appearance="fill">
      <mat-label>Choose a date</mat-label>
      <input [(ngModel)]="inputDate" matInput [matDatepicker]="picker" />
      <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
      <mat-datepicker #picker></mat-datepicker>
    </mat-form-field>
  `,
  styles: [
    `
      .container {
        width: 350px;
      }
    `
  ]
})
export class GridDatePickerComponent implements ICellEditorAngularComp {
  private params: any;

  public inputDate: Date;

  @ViewChildren("input", { read: ViewContainerRef })
  public inputs: QueryList<any>;
  // private focusedInput: number = 0;

  agInit(params: any): void {
    this.params = params;

    // simple implementation - we assume a full name consists of a first and last name only
    this.inputDate = this.params.value;
  }

  // dont use afterGuiAttached for post gui events - hook into ngAfterViewInit instead for this
  ngAfterViewInit() {
    this.focusOnInputNextTick(this.inputs.first);
  }

  private focusOnInputNextTick(input: ViewContainerRef) {
    window.setTimeout(() => {
      input.element.nativeElement.focus();
    }, 0);
  }

  getValue() {
    const date = new Date(this.inputDate);
    // This will return an ISO string matching your local time.
    const selectedDate= new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes() - date.getTimezoneOffset()).toISOString();
    return selectedDate;
  }

  isPopup(): boolean {
    return true;
  }
}


