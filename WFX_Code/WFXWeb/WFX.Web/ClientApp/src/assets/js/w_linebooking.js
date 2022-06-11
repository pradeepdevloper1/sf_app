
var gridOptions = {
  columnDefs: [
    { field: "Module", minWidth: 100 },
    { field: "Line", minWidth: 100 },
    { field: "Style", minWidth: 100 },
    { field: "SONo", minWidth: 100 },
    { field: "PONo", minWidth: 100 },
    { field: "Quantity", minWidth: 100 },
    { field: "SMV", minWidth: 100 },
    { field: "PlannedEffeciency", minWidth: 100 },
    { field: "StartDate", minWidth: 100 },
    { field: "EndDate", minWidth: 100 }
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

  // we expect the following columns to be present
  var columns = {
    'A': 'Module',
    'B': 'Line',
    'C': 'Style',
    'D': 'SONo',
    'E': 'PONo',
    'F': 'Quantity',
    'G': 'SMV',
    'H': 'PlannedEffeciency',
    'I': 'StartDate',
    'J': 'EndDate'
  };

  var rowData = [];

  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;

  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
/*    alert(worksheet['A' + rowIndex].w);*/
    var row = {};
    Object.keys(columns).forEach(function (column) {
      row[columns[column]] = worksheet[column + rowIndex].w;
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
  var filepath = sessionStorage.getItem("linebookingfilepath");
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
      /* alert(rowNode.data.S1);*/
      if (rowNode.data.S1 < 0) {
        isAllOk = false;
      }
      var orderItem = {
        UserID: parseInt(sessionStorage.getItem("userID")),
        FactoryID: parseInt(sessionStorage.getItem("factoryID")),
        LineBookingID: index,
        Module: rowNode.data.Module,
        Line: rowNode.data.Line,
        Style: rowNode.data.Style,
        SONo: rowNode.data.SONo,
        PONo: rowNode.data.PONo,
        Quantity: parseInt(rowNode.data.Quantity),
        SMV: parseFloat(rowNode.data.SMV),
        PlannedEffeciency: parseFloat(rowNode.data.PlannedEffeciency),
        StartDate: new Date(rowNode.data.StartDate),
        EndDate: new Date(rowNode.data.EndDate),
      }
      list.push(orderItem);
    });

    /*    var re = JSON.stringify(list);*/
    /*  alert(re);*/
    var apiurl = sessionStorage.getItem("apiurl") + 'LineBooking/PostLineBooking';
    $.ajax({
      cache: false,
      url: apiurl,
      headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
      type: "Post",
      data: JSON.stringify(list),
      contentType: 'application/json',
      async: false,
      success: function (data) {
        /*alert(data);*/
        res = data;
        sessionStorage.setItem("linebookingfilename", "");
        sessionStorage.setItem("linebookingfilepath", "");
      },
      error: function (data) {
        res = data;
      },
    });
    return res;
  }
  else {
    alert("Please remove all errors before save.");
  }
}

var errorcount = 0;

function geterrorcount() {
/*alert("errorcount call ");*/
  errorcount = 0;
  var list = [];
  var isAllOk = true;
  var oneerror = "";
  $("#errortable").html('');
  gridOptions.api.forEachNode((rowNode, index) => {
    /* alert(rowNode.data.S1);*/
    var rowno = index + 1;

    //// Excel Point No 1
    if (rowNode.data.Module == null || rowNode.data.Module == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Module.</td></tr>";
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

    if (rowNode.data.Quantity == null || rowNode.data.Quantity == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill Quantity.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.SMV == null || rowNode.data.SMV == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill SMV.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.PlannedEffeciency == null || rowNode.data.PlannedEffeciency == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill PlannedEffeciency.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.StartDate == null || rowNode.data.StartDate == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill StartDate.</td></tr>";
      $("#errortable").append(oneerror);
    }

    if (rowNode.data.EndDate == null || rowNode.data.EndDate == "") {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Please fill EndDate.</td></tr>";
      $("#errortable").append(oneerror);
    }


    // Excel Point No 3
    var data = {
      ModuleName: rowNode.data.Module
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

    // Excel Point No 5
    if (rowNode.data.Style != null && rowNode.data.Style != ""){
    var data = {
      Style: rowNode.data.Style,
      PONo:rowNode.data.PONo,
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

    // Excel Point No 6
    var data = {
      SONo: rowNode.data.SONo
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

    // Excel Point No 7
    var StartDate = new Date(rowNode.data.StartDate);
    var EndDate = new Date(rowNode.data.EndDate);
    if (EndDate < StartDate) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>End date cannot be before the start date.</td></tr>";
      $("#errortable").append(oneerror);
    }

    // Excel Point No 8
    if (parseInt(rowNode.data.Quantity) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Quantity can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.Quantity)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Quantity must be positive integer.</td></tr>";
      $("#errortable").append(oneerror);
    }

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

    if (parseFloat(rowNode.data.PlannedEffeciency) < 0) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Effeciency can’t be negative.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (parseFloat(rowNode.data.PlannedEffeciency) > 100) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>Planned Effeciency should be less than 100.</td></tr>";
      $("#errortable").append(oneerror);
    }
    if (!$.isNumeric(rowNode.data.PlannedEffeciency)) {
      errorcount = errorcount + 1;
      oneerror = "<tr><th>Row " + rowno + "</th> <td>PlannedEffeciency must be positive decimal/integer.</td></tr>";
      $("#errortable").append(oneerror);
    }
  });
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
  /* alert("$ready"); */
  // lookup the container we want the Grid to use
  var eGridDiv = document.querySelector('#myGrid');

  // create the grid passing in the div to use together with the columns & data we want to use
  new agGrid.Grid(eGridDiv, gridOptions);
  importExcel();
});
