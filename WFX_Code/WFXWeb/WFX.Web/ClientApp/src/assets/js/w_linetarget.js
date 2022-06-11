// we expect the following columns to be present
// var TotalPlannedTarget = 0;
var columns = {
  'A': 'Module',
  'B': 'Process',
  'C': 'Section',
  'D': 'Line',
  'E': 'Style',
  'F': 'SONo',
  'G': 'PONo',
  'H': 'Part',
  'I': 'Color',
  'J': 'SMV',
  'K': 'Operators',
  'L': 'Helpers',
  'M': 'ShiftName',
  'N': 'ShiftHours',
  'O': 'Date',
  'P': 'PlannedEffeciency',
  'Q': 'PlannedTarget',

  'R': 'S',
  'S': 'S',
  'T': 'S',
  'U': 'S',
  'V': 'S',
  'W': 'S',
  'X': 'S',
  'Y': 'S',
  'Z': 'S',
  'AA': 'S',
  'AB': 'S',
  'AC': 'S',
  'AD': 'S',
  'AE': 'S',
  'AF': 'S',
  'AG': 'S'
};

var sizecolumns = {

};

var duplicatesize = [];

var gridOptions = {
  columnDefs: [
    { field: "Module", minWidth: 100 },
    { field: "Process", minWidth: 100 },
    { field: "Section", minWidth: 100 },
    { field: "Line", minWidth: 100 },
    { field: "Style", minWidth: 100 },
    { field: "SONo", minWidth: 100 },
    { field: "PONo", minWidth: 100 },
    { field: "Part", minWidth: 100 },
    { field: "Color", minWidth: 100 },
    { field: "SMV", minWidth: 100 },
    { field: "Operators", minWidth: 100 },
    { field: "Helpers", minWidth: 100 },
    { field: "ShiftName", minWidth: 100 },
    { field: "ShiftHours", minWidth: 100 },
    { field: "Date", minWidth: 100 },
    { field: "PlannedEffeciency", headerName: "Planned Efficiency", minWidth: 100 },
    { field: "PlannedTarget", headerName: "Planned Target", minWidth: 100 },


  ],

  defaultColDef: {
    resizable: true,
    minWidth: 80,
    flex: 1,
    filter: true,
    editable: true,
  },
  pagination: true,
  paginationPageSize: 10,
  rowData: []
};
// XMLHttpRequest in promise format
function makeRequest(method, url, success, error) {
  /*  alert("Hello in makeRequest");*/

  var httpRequest = new XMLHttpRequest();
  httpRequest.open("GET", url, true);
  httpRequest.responseType = "arraybuffer";

  httpRequest.open(method, url);
  httpRequest.onload = function () {
    /* alert(" makeRequest success");*/
    success(httpRequest.response);
  };
  httpRequest.onerror = function () {
    /*alert(" makeRequest fail");*/
    error(httpRequest.response);
  };
  httpRequest.send();
}

// read the raw data and convert it to a XLSX workbook
function convertDataToWorkbook(data) {
  /* alert("Hello in convertDataToWorkbook");*/
  /* convert data to binary string */
  var data = new Uint8Array(data);
  var arr = new Array();

  for (var i = 0; i !== data.length; ++i) {
    arr[i] = String.fromCharCode(data[i]);
  }

  //alert("Hello in convertDataToWorkbook 2");
  //alert(arr);
  var bstr = arr.join("");

  return XLSX.read(bstr, { type: "binary" });

}

// pull out the values we're after, converting it into an array of rowData

function populateGrid(workbook) {
  /*  alert("Hello in populateGrid");*/
  // our data is in the first sheet
  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];

  var rowIndex = 1;
  var count = 0;
  var columnDefs = gridOptions.api.getColumnDefs();
  Object.keys(columns).forEach(function (column) {
    if (worksheet[column + rowIndex]) {
      var sizename = worksheet[column + rowIndex].w;
      count++;
      if (count > 17) {
        duplicatesize.push(sizename);
        columnDefs.push(
          {
            headerName: sizename,
            field: sizename,
          }
        );
        columns[column] = sizename;
        sizecolumns[sizename] = sizename;
      }
    }
  });
  gridOptions.api.setColumnDefs(columnDefs);
  /*alert("column count = " + count);*/

  // start at the 2nd row - the first row are the headers
  rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var row = {};
    Object.keys(columns).forEach(function (column) {
      if (worksheet[column + rowIndex]) {
        row[columns[column]] = worksheet[column + rowIndex].w;
      }
    });

    rowData.push(row);

    rowIndex++;
  }

  //var myJSON = JSON.stringify(rowData);
  //alert(myJSON);

  // finally, set the imported rowData into the grid
  gridOptions.api.setRowData(rowData);
}

function importExcel() {
  //alert("Hello in Import excel");
  //var filenamestr = "uploads/excel/" + sessionStorage.getItem("orderexcelfilepath");
  var filepath = sessionStorage.getItem("linetargetfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      populateGrid(workbook);
      geterrorcount();
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}

function getlist() {
  /*alert("getlist call ");*/
  var list = [];
  var res = "";
  geterrorcount();

  var errorcount = parseInt($("#errocount").text());
  /*  alert("errorcount " + errorcount);*/
  if (parseInt(errorcount) == 0) {
    gridOptions.api.forEachNode((rowNode, index) => {

      var sizestring = "";
      var totalqty = 0;
      var index = 0;
      var sizename = "";
      var colsizename = "";
      var sizeqty = 0;
      Object.keys(sizecolumns).forEach(function (column) {
        index++;
        colsizename = sizecolumns[column];
        if (index % 2 == 0) {
          sizeqty = parseInt(rowNode.data[colsizename]);
          if (sizeqty > 0) {
            totalqty = totalqty + sizeqty;
            if (sizestring == "") { sizestring = sizename + "-" + sizeqty; }
            else { sizestring = sizestring + "," + sizename + "-" + sizeqty; }
          }
          sizename = "";
          sizeqty = 0;
        }
        else {
          sizename = rowNode.data[colsizename];
        }
      });
      /*return;*/
      /* alert(rowNode.data.S1);*/
      var orderItem = {
        UserID: parseInt(sessionStorage.getItem("userID")),
        FactoryID: parseInt(sessionStorage.getItem("factoryID")),

        LineTargetID: index,
        Module: rowNode.data.Module,
        ProcessCode: rowNode.data.Process,
        Section: rowNode.data.Section,
        Line: rowNode.data.Line,
        Style: rowNode.data.Style,
        SONo: rowNode.data.SONo,
        PONo: rowNode.data.PONo,

        Part: rowNode.data.Part,
        Color: rowNode.data.Color,
        SMV: parseFloat(rowNode.data.SMV),
        Operators: parseFloat(rowNode.data.Operators),
        Helpers: parseFloat(rowNode.data.Helpers),
        ShiftName: rowNode.data.ShiftName,
        ShiftHours: parseFloat(rowNode.data.ShiftHours),
        Date: moment.utc(rowNode.data.Date, 'DD/MM/YYYY'),
        PlannedEffeciency: parseFloat(rowNode.data.PlannedEffeciency),
        PlannedTarget: parseFloat(rowNode.data.PlannedTarget),
        SizeList: sizestring,

      }
      list.push(orderItem);
    });


    var re = JSON.stringify(list);
    // alert(re);
    var apiurl = sessionStorage.getItem("apiurl") + 'LineTarget/PostLineTarget';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "Post",
      data: JSON.stringify(list),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
        /*alert(data);*/
        sessionStorage.setItem("linetargetfilename", "");
        sessionStorage.setItem("linetargetfilepath", "");
        /*return (data);*/
      },
      error: function (data) {
        res = data;
        /*alert("Something went wrong.");*/
        /*return ("Something went wrong.");*/
      },
    });
    return res;
  }
  else {
    alert("Please remove all errors before save.");
  }
}

  function geterrorcount() {
  /*alert("errorcount call ");*/
  var list = [];
  var isAllOk = true;
  var errorcount = 0;
  var oneerror = "";
 var arrerror=[];
  $("#errortable").html('');
  gridOptions.api.forEachNode((rowNode, index) => {
    /* alert(rowNode.data.S1);*/
    var rowno = index + 1;
    var pono = rowNode.data.PONo;
    var date = new Date(rowNode.data.Date);
    var shift = rowNode.data.ShiftName;
    var rowcount = 0;
    //alert(pono);
    //alert(date);
    //alert(shift);
    // TotalPlannedTarget =TotalPlannedTarget+ parseInt(rowNode.data.PlannedTarget);


    gridOptions.api.forEachNode((rowNodechild, index) => {
      /*alert(rowNodechild.data.PONo);*/
      if (rowNodechild.data.PONo == pono && new Date(rowNodechild.data.Date) == new Date(date) && rowNodechild.data.ShiftName == shift) {
        /*alert(rowNodechild.data.PONo);*/
        rowcount++;
      }
    });
    if (rowcount > 1) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Different targets for same date and same shift.</td></tr>";
      $("#errortable").append(oneerror);
    }

    //// Excel Point No 1
    SizeColList = "";
    var LineTargetQtySum=0;
    Object.keys(sizecolumns).forEach(function (column) {
      if ((rowNode.data[column] == undefined || rowNode.data[column] == "") && column.length==5 && (rowNode.data[column+'Target'] != undefined && rowNode.data[column+'Target'] != "") ) {
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill "+column+".</td></tr>";
        $("#errortable").append(oneerror);
      }
      if(column.toLocaleLowerCase().includes('target') && Number.isInteger(parseInt(rowNode.data[column]) ))
      {
        LineTargetQtySum+= parseInt(rowNode.data[column]);
      }
      else if(!column.toLocaleLowerCase().includes('target')){
        if(rowNode.data[column] != undefined)
          SizeColList += rowNode.data[column] + ',';
      }

    });

    SizeColList = SizeColList.substring(0,SizeColList.length-1);

    if (LineTargetQtySum == 0 &&  (rowNode.data.PlannedTarget==0 || !rowNode.data.PlannedTarget)  ) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill either Planned Target or SizeWise quantities.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Module == null || rowNode.data.Module == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Module.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Process == null || rowNode.data.Process == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Process.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.Section == null || rowNode.data.Section == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Section.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Line == null || rowNode.data.Line == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Line.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Style == null || rowNode.data.Style == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Style.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.SONo == null || rowNode.data.SONo == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill SONo.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.PONo == null || rowNode.data.PONo == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PONo.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Part == null || rowNode.data.Part == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Part.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Color == null || rowNode.data.Color == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Color.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.SMV == null || rowNode.data.SMV == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill SMV.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Operators == null || rowNode.data.SMOperatorsV == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Operators.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Helpers == null || rowNode.data.Helpers == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Helpers.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.ShiftName == null || rowNode.data.ShiftName == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill ShiftName.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.ShiftHours == null || rowNode.data.ShiftHours == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill ShiftHours.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Date == null || rowNode.data.Date == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Date.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var PlanDate = moment.utc(rowNode.data.Date, 'DD/MM/YYYY');
    const dateObj = new Date();
    var CurrDate = moment.utc(dateObj.getDate(), 'DD/MM/YYYY');
    if (PlanDate < CurrDate) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned date can't be before than the current date.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.PlannedEffeciency == null || rowNode.data.PlannedEffeciency == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Planned Efficiency.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (rowNode.data.PlannedEffeciency == '0' || rowNode.data.PlannedEffeciency == 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Efficiency can't be zero</td></tr>";
      $("#errortable").append(oneerror);
    }

    //if (rowNode.data.PlannedTarget == null || rowNode.data.PlannedTarget == "") {
    //  errorcount = errorcount + 1;
    //  oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PlannedTarget.</td></tr>";
    //  $("#errortable").append(oneerror);
    //}
    if(rowNode.data.Date!=""  && rowNode.data.PONo != null && rowNode.data.PONo != ""){
      var DateArr = rowNode.data.Date.split('/');
      if( DateArr[1]>12  || DateArr[1]==undefined){
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>please follow dd/mm/yy.</td></tr>";
        $("#errortable").append(oneerror);
      }
      else{
        var data = {
        PONo: rowNode.data.PONo,
        SONo: rowNode.data.SONo,
        Color: rowNode.data.Color,
        Date: moment.utc(rowNode.data.Date, 'DD-MM-YYYYTHH:MM:SS')
      }
    }
  if(data!=undefined){
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsOrderRun';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 200) {
      rowNode.data.IsOrderRun=1;
    }
    else if(res.status == 401 || 400){
      rowNode.data.IsOrderRun=0;
    }
  }
}
    let FactoryID=parseInt(sessionStorage.getItem("factoryID"));
    // Excel Point No 3
    var data = {
      ModuleName: rowNode.data.Module,
      FactoryID:FactoryID

    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Module/PostIsModuleExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Module doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
    var data = {
      SizeList: SizeColList,
      PONo:rowNode.data.PONo,
      FactoryID:FactoryID,
      Color: rowNode.data.Color
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsSizeExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
     if (res.status == 401) {
      errorcount = errorcount + 1;
       oneerror = "<tr><th>Row " + rowno + " </th> <td>" +res.sizes+" sizes does not exist in PO.</td></tr>";
       $("#errortable").append(oneerror);

    }
    // Excel Point No 4
    var data = {
      LineName: rowNode.data.Line
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Line/PostIsLineExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Line doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.Part != null && rowNode.data.Part != ""){
      var data = {
        Part: rowNode.data.Part,
        PONo: rowNode.data.PONo,
        SONo: rowNode.data.SONo
      }
      var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsPartExist';
      $.ajax({
        cache: false,
        url: apiurl,
        headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
        type: "POST",
        data: JSON.stringify(data),
        contentType: 'application/json',
        async: false,
        success: function (data) {
          res = data;
        },
        error: function (data) {
          res = data;
        },
      });

      if (res.status == 401) {
        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + " </th> <td>Part doesn’t match for PONo.</td></tr>";
        $("#errortable").append(oneerror);
      }
    }
    // Excel Point No 5
    if (rowNode.data.Style != null && rowNode.data.Style != ""){
    var data = {
      Style: rowNode.data.Style,
      PONo: rowNode.data.PONo,
      SONo: rowNode.data.SONo
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsStyleExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });

    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Style doesn’t match for PONo.</td></tr>";
      $("#errortable").append(oneerror);
    }
  }
  if (rowNode.data.Color != null && rowNode.data.Color != ""){
    var data = {
      Color: rowNode.data.Color,
      FactoryID:FactoryID,
      PONo:rowNode.data.PONo
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsColorExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Color doesn’t exist in the Po.</td></tr>";
      $("#errortable").append(oneerror);
    }
  }
  if (rowNode.data.ShiftName != null && rowNode.data.ShiftName != ""){
    var data = {
      ShiftName: rowNode.data.ShiftName,
      FactoryID:FactoryID,
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsShiftExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>Shift doesn’t exist in the Factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
  }
    // Excel Point No 6
  if (rowNode.data.SONo != null && rowNode.data.SONo != ""){
      var data = {
      SONo: rowNode.data.SONo,
      FactoryID:FactoryID,
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsSOExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>SO doesn’t exist in the factory.</td></tr>";
      $("#errortable").append(oneerror);
    }
  }


// function getKeyByValue(object, value) {
//   return Object.keys(rowNode.data).find(key => object[key] === value);
// }


//   if(!column.toLocaleLowerCase().includes('target')){

// });
if(rowNode.data.IsOrderRun===1){
Object.keys(sizecolumns).forEach(function (column) {
  if(!column.toLocaleLowerCase().includes('target')){
var data = {
  PONo: rowNode.data.PONo,
  FactoryID:FactoryID,
  SONo: rowNode.data.SONo,
  Color: rowNode.data.Color,
  Size: rowNode.data[column],
  QCDate: moment.utc(rowNode.data.Date, 'DD-MM-YYYYTHH:MM:SS')
}
var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostTotalCheckedPcs';
$.ajax({
  cache: false,
  url: apiurl,
  headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
  type: "POST",
  data: JSON.stringify(data),
  contentType: 'application/json',
  async: false,
  success: function (data) {
    res = data;
  },
  error: function (data) {
    res = data;
  },
});
if (res.status == 200 && res.data > rowNode.data[column+"Target"]) {
  errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno +  " </th> <td>"+ rowNode.data[column]+" Target cannot be less than "+res.data+"</td></tr>";
  $("#errortable").append(oneerror);
}
}
})
  var data = {
  SONo: rowNode.data.SONo,
  FactoryID:FactoryID,
  PONo: rowNode.data.PONo,
  Color: rowNode.data.Color,
  Date: moment.utc(rowNode.data.Date, 'DD-MM-YYYYTHH:MM:SS')
}
var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostCheckLineTargetValues';
$.ajax({
  cache: false,
  url: apiurl,
  headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
  type: "POST",
  data: JSON.stringify(data),
  contentType: 'application/json',
  async: false,
  success: function (data) {
    res = data;
  },
  error: function (data) {
    res = data;
  },
});
if (res.status == 200 && (res.data && res.data.length>0)) {
  if(res.data[0].module !== rowNode.data.Module){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Module as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }if(res.data[0].section !== rowNode.data.Section){
    errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Section as Order has already started on floor</td></tr>";
  $("#errortable").append(oneerror);
  }if(res.data[0].line !== rowNode.data.Line){
    errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Line as Order has already started on floor</td></tr>";
  $("#errortable").append(oneerror);
  }if(res.data[0].style !== rowNode.data.Style){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot Style change as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }
  if(res.data[0].poNo !== rowNode.data.PONo){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change PONo as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }
  if(res.data[0].soNo !== rowNode.data.SONo){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change SONo  as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }if(res.data[0].part !== rowNode.data.Part){
    errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Part Order has already started on floor</td></tr>";
  $("#errortable").append(oneerror);
  }if(res.data[0].color !== rowNode.data.Color){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Color as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }if(res.data[0].operators.toString() !== rowNode.data.Operators.toString()){
    errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Operators as Order has already started on floor</td></tr>";
  $("#errortable").append(oneerror);
  }if(res.data[0].smv.toString() !== rowNode.data.SMV.toString()){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change SMV as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }if(res.data[0].helpers.toString()  !== rowNode.data.Helpers.toString() ){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change Helpers as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }if(res.data[0].shiftName !== rowNode.data.ShiftName){
    errorcount = errorcount + 1;
  oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change ShiftName as Order has already started on floor</td></tr>";
  $("#errortable").append(oneerror);
  }if(res.data[0].shiftHours.toString() !== rowNode.data.ShiftHours.toString()){
    errorcount = errorcount + 1;
    oneerror = "<tr><th>Row " + rowno + " </th> <td>Cannot change ShiftHours as Order has already started on floor</td></tr>";
    $("#errortable").append(oneerror);
  }


}
}


    // Excel Point No 8
    if (parseFloat(rowNode.data.SMV) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>SMV can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.SMV)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>SMV must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (parseFloat(rowNode.data.Operators) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Operators can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.Operators)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Operators must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (parseFloat(rowNode.data.Helpers) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Helpers can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.Helpers)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Helpers must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (parseFloat(rowNode.data.ShiftHours) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>ShiftHours can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.ShiftHours)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>ShiftHours must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (parseFloat(rowNode.data.PlannedEffeciency) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Efficiency can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (parseFloat(rowNode.data.PlannedEffeciency) > 100) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Efficiency should be less than 100.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.PlannedEffeciency)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Efficiency must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }
    // if (!$.isNumeric(rowNode.data.PlannedTarget) || parseFloat(rowNode.data.PlannedTarget) < 0) {
    //   errorcount = errorcount + 1;
    //   oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Target must be positive decimal/integer.</td></tr>";
    //   $("#errortable").append(oneerror);
    // }

    //if (parseInt(rowNode.data.PlannedTarget) < 0) {
    //  errorcount = errorcount + 1;
    //  oneerror = "<tr><th>Row " + rowno + "</th> <td>PlannedTarget can’t be negative.</td></tr>";
    //  $("#errortable").append(oneerror);
    //}
    //if (!$.isNumeric(rowNode.data.PlannedTarget)) {
    //  errorcount = errorcount + 1;
    //  oneerror = "<tr><th>Row " + rowno + "</th> <td>PlannedTarget must be positive integer.</td></tr>";
    //  $("#errortable").append(oneerror);
    //}
    var data = {
      PONo: rowNode.data.PONo,
      FactoryID:FactoryID,
      FromPage :"LineTarget"
    }
    var apiurl = sessionStorage.getItem("apiurl") + 'Order/PostIsPOExist';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "POST",
      data: JSON.stringify(data),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        res = data;
      },
      error: function (data) {
        res = data;
      },
    });
    if (res.status == 401) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>PO doesn’t exist in the factory</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (res.status == 402) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + " </th> <td>PO already completed in the factory</td></tr>";
      $("#errortable").append(oneerror);
    }
    var arr1 = [rowNode.data.PONo, rowNode.data.Part, rowNode.data.Color, rowNode.data.ShiftName, rowNode.data.Line,rowNode.data.Date];
    var count = 0;
    gridOptions.api.forEachNode((rowNode, index) => {
      var rowno = index + 1;
      var arr2 = [rowNode.data.PONo, rowNode.data.Part, rowNode.data.Color, rowNode.data.ShiftName, rowNode.data.Line,rowNode.data.Date];
      if (JSON.stringify(arr1) === JSON.stringify(arr2)) {
        count = count + 1;
      }
      if ((JSON.stringify(arr1) === JSON.stringify(arr2)) && (count > 1) && arrerror.indexOf(rowno) == -1) {
        arrerror.push(rowno)

        errorcount = errorcount + 1;
        oneerror = "<tr><th>Row " + rowno + "</th> <td>Multiple records for same PO, Part, Color, Line, Shift and Date are not allowed.</td></tr>";
        $("#errortable").append(oneerror);
      }
    });
    //console.log(data);
    //var apiurl = sessionStorage.getItem("apiurl") + 'LineTarget/PostIsPOPlanExist';
    //$.ajax({
    //  cache: false,
    //  url: apiurl,
    //  headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    //  type: "POST",
    //  data: JSON.stringify(data),
    //  contentType: 'application/json',
    //  async: false,
    //  success: function (data) {
    //    res = data;
    //  },
    //  error: function (data) {
    //    res = data;
    //  },
    //});
    //if (res.status == 200) {
    //  errorcount = errorcount + 1;
    //  oneerror = "<tr><th>Row " + rowno + " </th> <td>Plan already exists in the system.</td></tr>";
    //  $("#errortable").append(oneerror);
    //}

  });
  // if(TotalPlannedTarget == 0){
  //   errorcount = errorcount + 1;
  //   oneerror = "<tr><th>Col</th> <td>Total Planned Target Quantity can't be zero</td></tr>";
  //   $("#errortable").append(oneerror);
  // }
  $("#errocount").html(errorcount);
  if (errorcount > 0) {
    $(".edit-alert-primary").show();
    $(".edit-alert-green").hide();
  }
  else {
    $(".edit-alert-primary").hide();
    $(".edit-alert-green").show();
  }
  nexterror();
}

function nexterror() {
  /*alert("nexterror call ");*/
  var rowCount = $('#errortable tr').length;
  /*alert("rowCount " + rowCount);*/
  var curr_row = $('#errortable').find("tr th.activered").parent('tr').index();
  /*alert("curr_row " + curr_row);*/
  curr_row = curr_row + 1;
  if (curr_row < rowCount) {
    $('#errortable').find('tr th').each(function (index, element) {
      $(element).removeClass("activered");
    });
    $('#errortable').find('tr td').each(function (index, element) {
      $(element).removeClass("activered");
    });

    $('#errortable').find("tr:eq(" + curr_row + ") th").addClass("activered");
    $('#errortable').find("tr:eq(" + curr_row + ") td").addClass("activered");

  }
  var row = $('#errortable').find("tr:eq(" + curr_row + ")");
  var w = $('#errordiv');
  if (row.length) {
    w.scrollTop(row.offset().top - (w.height() / 2));
  }
}

function prverror() {
  /*alert("prverror call ");*/
  var rowCount = $('#errortable tr').length;
  /*alert("rowCount " + rowCount);*/
  var curr_row = $('#errortable').find("tr th.activered").parent('tr').index();
  /*alert("curr_row " + curr_row);*/
  curr_row = curr_row - 1;
  if (curr_row >= 0) {
    $('#errortable').find('tr th').each(function (index, element) {
      $(element).removeClass("activered");
    });
    $('#errortable').find('tr td').each(function (index, element) {
      $(element).removeClass("activered");
    });

    $('#errortable').find("tr:eq(" + curr_row + ") th").addClass("activered");
    $('#errortable').find("tr:eq(" + curr_row + ") td").addClass("activered");

    var row = $('#errortable').find("tr:eq(" + curr_row + ")");
    var w = $('#errordiv');
    if (row.length) {
      w.scrollTop(row.offset().top - (w.height() / 2));
    }
  }
}

$(document).ready(function () {
  //alert("$ready");
  // lookup the container we want the Grid to use
  var eGridDiv = document.querySelector('#myGrid');

  // create the grid passing in the div to use together with the columns & data we want to use
  new agGrid.Grid(eGridDiv, gridOptions);
  importExcel();
});

