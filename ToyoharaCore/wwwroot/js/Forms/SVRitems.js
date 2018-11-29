var selectedRecord = '';
var showSelected = null;
var records;
rebind = true;





//для группового обновления - настройка доступности выбранных полей
function GroupUpdateAttribute(attributename) {
    if (document.getElementById(attributename).checked== false)
    {
        document.getElementById(attributename + 'Value').disabled = 'disabled';
        document.getElementById(attributename).checked = false;
    }
    else
    {
        document.getElementById(attributename + 'Value').disabled = '';
        document.getElementById(attributename).checked = true;
    };

}



//при чтении контента положить данные в кэш
function contentReady() {
    rebind = false;
}


//функция очищения фильтров
function ClearFilters() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    dataGrid.clearFilter();
}


//выгрузка в excel грида с позициями загрузки
function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/Forms/ExcelReportSVRitems',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            form_id: parseFloat(document.getElementById("form_id").value),
            show_only_for_pricing: document.getElementById('show_only_for_pricing').checked
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Позиции СВР (код:" + document.getElementById("form_id").value+")");
        }
    });
}




function SVRitemsDiff() {
    window.location = "/Forms/SVRitemsDiff?form_id=" + document.getElementById("form_id").value + "&link_information_param=" + document.getElementById("form_id").value;
}

function ManualCommitChanges() {
    window.location = "/Forms/SVRItemsManualDiff?form_id=" + document.getElementById("form_id").value + "&link_information_param=" + document.getElementById("form_id").value;
}


function Back()
{
    window.location = "/Forms/Index";
}



//Отчет "Форма субподрядчика" 
function SVRDeliveryTypeTemplate() {
    $.ajax({
        async: true,
        cache: false,
        url: '/Forms/SVRDeliveryTypeTemplate',
        type: 'POST',
        data: {
            form_id: document.getElementById("form_id").value
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Загрузка типов поставки для СВР (код:" + document.getElementById("form_id").value + ")");
        }
    });
}

function SVRDeliveryTypeInsert() {
    $('#UploaderFlowWindow').modal('show');
}


//Отчет "Форма субподрядчика" 
function ExcelReportSVRitemsSubcontractorForm() {
    $.ajax({
        async: true,
        cache: false,
        url: '/Forms/ExcelReportSVRitemsSubcontractorForm',
        type: 'POST',
        data: {
            form_id: document.getElementById("form_id").value
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Форма субподрядчика СВР (код:" + document.getElementById("form_id").value + ")");
        }
    });
}


//Отчет "Форма субподрядчика" 
function ReportDeliveryTypeDifference() {
    $.ajax({
        async: true,
        cache: false,
        url: '/Forms/ReportDeliveryTypeDifferenceSVR',
        type: 'POST',
        data: {
            form_id: document.getElementById("form_id").value
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Отчет о несоответствии типов поставки СВР (код:" + document.getElementById("form_id").value + ")");
        }
    });
}


//Сохарнение текущей версии как архив
function InsertArchive() {
    if (confirm("Вы действительно хотите сохранить текущую версию как архив?")) {
         $.ajax({
            async: true,
            cache: false,
             url: '/Forms/InsertArchive',
            type: 'POST',
            data: {
                form_id: parseFloat(document.getElementById("form_id").value),
                form_type_id: 5
            },
            success: function (data) {
                if (data != "" && data != null)
                    alert(data.replace(/__/gi, '\n'));
                else alert("Версия успешно сохранена");
            }
        });
    }
}

//заполнение модели для грида
function ReturnData(controller) {
    var obj = {
        showSelected: showSelected,
        selectedRecord: selectedRecord,
        id: null,
        form_id: parseFloat(document.getElementById("form_id").value),
        rebind: rebind,
        storedProcedure: "OMC_SELECT_FORM_SVR_ITEMS",
        controller: controller,
        show_only_for_pricing: document.getElementById('show_only_for_pricing').checked
    };
    return JSON.stringify(obj);
}


// Показать выбранные/показать все
function selection_changed(selectedItems) {
    var data = selectedItems.selectedRowsData;
    records = [];
    $.each(data, function (i, val) {
        records.push(val.id);
    });
    selectedRecord = records.toString();
}


//функция открытия окна группового обновления
function BeforeGroupUpdate() {
        var dataGrid = $("#Grid").dxDataGrid("instance");
        var keys = dataGrid.getSelectedRowKeys();
        if (keys.length == 0)
            return alert("Вы не выбрали ни одной строки");
        $("#GroupUpdate").modal("show");
}


//Групповое обновление
function GroupUpdate() {
        var dataGrid = $("#Grid").dxDataGrid("instance");
        var keys = dataGrid.getSelectedRowKeys();
        if (keys[0] == ',')
            keys[0] = '';
        var FunctionsRecords = [];
        for (var i = 0; i < keys.length; i++) {
            var row_index = dataGrid.getRowIndexByKey(keys[i]);
            row_id = dataGrid.cellValue(row_index, "id");
            FunctionsRecords.push(row_id);
        }


    var DeclineComment = null;
    var AddInfo = null;
    var PriceManager = null;
    var ForPricing = null;
    var DeliveryType = null;
    if (document.getElementById('isDeclineComment').checked == true) {

        if ($('#isDeclineCommentValue').val() == null | $('#isDeclineCommentValue').val() == "undefined" | $('#isDeclineCommentValue').val() == '') {
            DeclineComment = "";
        }
        else {
            DeclineComment = $('#isDeclineCommentValue').val();
        }
    }
    if (document.getElementById('isAddInfo').checked == true) {
        if ($('#isAddInfoValue').val() == null | $('#isAddInfoValue').val() == "undefined" | $('#isAddInfoValue').val() == '') {
            AddInfo = "";
        }
        else
        {
            AddInfo = $('#isAddInfoValue').val();
        }
    }


    if (document.getElementById('isPriceManager').checked == true) 
            PriceManager = $('#isPriceManagerValue').val();
   

    if (document.getElementById('isForPricing').checked == true)
        ForPricing = document.getElementById('isForPricingValue').checked;

    if (document.getElementById('isDeliveryType').checked == true)
        DeliveryType = $('#isDeliveryTypeValue').val();



        $.ajax({
            async: true,
            cache: false,
            url: '/Forms/GroupUpdate',
            type: 'POST',
            data: {
                form_type_id:5,
                selectedRecord: FunctionsRecords.toString(),
                isDeclineComment: document.getElementById('isDeclineComment').checked,
                DeclineComment: DeclineComment,
                isAddInfo: document.getElementById('isAddInfo').checked,
                AddInfo: AddInfo,
                PriceManager: PriceManager,
                ForPricing: ForPricing,
                DeliveryType: DeliveryType
            },
            success: function (data) {
                if (data != "" && data != null) { alert(data.replace(/__/gi, '\n')); }
                else {
                    alert("Позиции успешно обновлены");
                    $("#Grid").dxDataGrid("getDataSource").reload();
                    dataGrid.deselectAll();
                    rebind = true;
                    ClearGroupUpdate();
                }

            }
        });
 
}


function ClearGroupUpdate() {

    document.getElementById('isDeclineComment').checked = false;
    document.getElementById('isDeclineCommentValue').disabled = 'disabled';
    document.getElementById('isDeclineCommentValue').value = '';

    document.getElementById('isAddInfo').checked = false;
    document.getElementById('isAddInfoValue').disabled = 'disabled';
    document.getElementById('isAddInfoValue').value = '';

    document.getElementById('isPriceManager').checked = false;
    document.getElementById('isPriceManagerValue').disabled = 'disabled';
    document.getElementById('isPriceManagerValue').value= '';

    document.getElementById('isForPricing').checked = false;
    document.getElementById('isForPricingValue').disabled = 'disabled';
    document.getElementById('isForPricingValue').checked = false;

    document.getElementById('isDeliveryType').checked = false;
    document.getElementById('isDeliveryTypeValue').disabled = 'disabled';
    document.getElementById('isDeliveryTypeValue').value = '';

}

