// we expect the following columns to be present
var columns = {
  'A': 'SNo',
  'B': 'OperationCode',
  'C': 'OperationName',
  'D': 'SMV',
  'E': 'Location',
};

var gridOptions = {
  columnDefs: [
    { field: "SNo" },
    { field: "OperationCode" },
    { field: "OperationName" },
    { field: "SMV" },
    { field: "Location" }
  ],

  defaultColDef: {
    resizable: true,
    minWidth: 80,
    flex: 1,
    filter: true,
    editable: true,
  },
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
  /*alert("Hello in populateGrid");*/
  // our data is in the first sheet

  var res = "";

  var firstSheetName = workbook.SheetNames[0];
  var worksheet = workbook.Sheets[firstSheetName];

  var rowData = [];
  // start at the 2nd row - the first row are the headers
  var rowIndex = 2;
  // iterate over the worksheet pulling out the columns we're expecting
  while (worksheet['A' + rowIndex]) {
    var row = {};
    // row["PONo"] = sessionStorage.getItem("pono")
    row["SONo"] = sessionStorage.getItem("sono")
    row["SNo"] = parseInt(worksheet['A' + rowIndex].w);
    row["OperationCode"] = worksheet['B' + rowIndex].w;
    row["OperationName"] = worksheet['C' + rowIndex].w;
    row["Section"] = "NA";
    row["SMV"] = parseFloat(worksheet['D' + rowIndex].w);
    row["OBLocation"] = worksheet['E' + rowIndex].w;

    //Object.keys(columns).forEach(function (column) {
    //  if (worksheet[column + rowIndex]) {
    //    if(column="A")
    //      row[columns[column]] = parseInt(worksheet[column + rowIndex].w);
    //    else if (column = "D")
    //      row[columns[column]] = parseFloat(worksheet[column + rowIndex].w);
    //    else
    //      row[columns[column]] = worksheet[column + rowIndex].w;
    //  }
    //});
/*    row["Section"] = "NA";*/
    rowData.push(row);
    rowIndex++;
  }

  if (worksheet['G1']) {
    alert("Please check excel format.");
    return;
  }
  //var myJSON = JSON.stringify(rowData);
  //alert(myJSON);


  var apiurl = sessionStorage.getItem("apiurl") + 'OB/PostOB';
/*  alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
  $.ajax({
    cache: false,
    url: apiurl,
    headers: { 'Authorization': "Bearer " + sessionStorage.getItem('auth') },
    type: "Post",
    data: JSON.stringify(rowData),
    contentType: 'application/json',
    async: false,
    success: function (data) {
      res = data;
      //alert("success");
      sessionStorage.setItem("sono", "");
      sessionStorage.setItem("obexcelfilename", "");
      sessionStorage.setItem("obexcelfilepath", "");
    },
    error: function (data) {
      res = data.responseText;
      alert(res);
      //throw error;
    },
  });
  if (res.status == 200) {
    //alert(res.message);
   // gridOptions.api.setRowData(rowData);
    location.reload(true);
    this.getData();
  }
  return res;

  // finally, set the imported rowData into the grid
  /*gridOptions.api.setRowData(rowData);*/
}

function importExcel() {
  /*alert("Hello in Import excel");*/
  var filepath = sessionStorage.getItem("obexcelfilepath");
  makeRequest('GET',
    filepath,
    // success
    function (data) {
      /*alert(" Import excel data" );*/
      var workbook = convertDataToWorkbook(data);
      /*alert("workbook create");*/
      var res = populateGrid(workbook);
      //alert("After executeion");
      //alert(res);
      return res;
    },
    // error
    function (error) {
      alert("error found");
      throw error;
    }
  );
  //alert("Import excel final call");
}

function saveob(pono) {
  alert("saveob call ");
  //alert("pono" + pono);
  var list = [];
  var res = "";

  gridOptions.api.forEachNode((rowNode, index) => {
    console.log(rowNode.data);
    var orderItem = {
      PONo: pono,
      SNo: parseInt(rowNode.data.SNo),
      OperationCode: rowNode.data.OperationCode,
      OperationName: rowNode.data.OperationName,
      Section: 'NA',
      SMV: parseFloat(rowNode.data.SMV),
      OBLocation: rowNode.data.Location
    }
    list.push(orderItem);
  });
  var re = JSON.stringify(list);
  alert(re);
  //return;

  var apiurl = sessionStorage.getItem("apiurl") + 'OB/PostOB';
  /*alert(apiurl);*/
  /*  alert(sessionStorage.getItem('auth'));*/
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
      //alert("success");
       sessionStorage.setItem("obexcelfilename", "");
       sessionStorage.setItem("obexcelfilepath", "");
    },
    error: function (data) {
      res = data;
      alert("error");
    },
  });
  return res;
}

$(document).ready(function () {
  /*alert("$ready");*/
  // lookup the container we want the Grid to use
  /*var eGridDiv = document.querySelector('#myGrid');*/

  // create the grid passing in the div to use together with the columns & data we want to use
  /*new agGrid.Grid(eGridDiv, gridOptions);*/
  /*  importExcel();*/

  $(".image").mouseover(function () {
    $(this).find(".icon1").show();
    $(this).find(".icon2").show();
    $(this).find(".icon3").show();
  });

  $(".image").mouseout(function () {
    $(this).find(".icon1").hide();
    $(this).find(".icon2").hide();
    $(this).find(".icon3").hide();
  });
});
