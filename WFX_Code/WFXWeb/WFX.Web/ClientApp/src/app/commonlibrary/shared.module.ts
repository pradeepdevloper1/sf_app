

import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule , MatRippleModule, MatOptionModule} from '@angular/material/core';
import { A11yModule } from "@angular/cdk/a11y";
import { GridDatePickerComponent } from './griddatepicker.component';
import { GridNumericFieldComponent } from './gridnumericfield.component';
import { GridSelectComponent } from './gridselect.component';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BrowserModule } from '@angular/platform-browser';
import {CommonFunctions} from './commonfunctions';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { GridSlideToggleComponent } from './gridslidetoggle.component';



// Mat components




@NgModule({
  declarations: [GridDatePickerComponent,GridNumericFieldComponent, GridSelectComponent, GridSlideToggleComponent],
  imports: [
    FormsModule, ReactiveFormsModule, MatDialogModule, MatSnackBarModule,MatInputModule, MatFormFieldModule,
    MatNativeDateModule, MatDatepickerModule, MatRippleModule, A11yModule, MatOptionModule, MatSelectModule,
    MatCheckboxModule, CommonModule,  BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),MatSlideToggleModule
  ],
  providers: [CommonFunctions,
    { provide: MAT_DIALOG_DATA, useValue: {} },
    { provide: MatDialogRef, useValue: {} }],

  bootstrap: [],
  exports: [FormsModule, ReactiveFormsModule]
})
export class SharedModule { }
