var selectedRecord = '';
var showSelected = null;
var records;
 rebind = false;


//функция, выполняющая при успешном выполнении загрузки
function OnCommitSuccess() {
    $('#UploaderFlowWindow').modal('hide');
    rebind = true;
    Reloading("Grid");
}





//при чтении контента положить данные в кэш
function contentReady() {
    rebind = false;
}


//заполнение модели для грида
function ReturnData(controller) {
    var obj = {
        showSelected: null,
        selectedRecord: '',
        id: null,
        rebind: rebind,
        storedProcedure: "OMC_SELECT_SVR",
        controller: controller
    };
    return JSON.stringify(obj);
}



//маппинг кнопок и действия
function FiltersBeforeGridString(add, reloading, clearFilters, excel, settings,  otherFilters) {
    return $(
        "<button title='Добавить' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' id='" + add.id + "'  onclick='Add();' ><img src='/../../img/GridBtn/1-1.png' style='height:18px; width:auto;' alt='Добавить'></img></button>" +
        "<button class='btn btn_in_grid dx-button btn_pad_grid' title='Обновить' onclick='" + reloading.function + "' id='" + reloading.id + "'><img src='/../../img/GridBtn/1-5.png' style='height:18px; width:auto;' alt='Обновить' ></img></button>" +
        "<button title='Очистить фильтры' onclick='" + clearFilters.name + "();' class='btn btn_in_grid dx-button btn_pad_grid' id='" + clearFilters.id + "'><img src='/../../img/GridBtn/1-9.png' style='height:18px; width:auto;'  alt='Очистить фильтры'></img></button>" +
        "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + excel.name + "();' id='" + excel.id + "'><img src='/../../img/GridBtn/1-4.png' style='height:18px; width:auto;' alt='Выгрузить Excel'></img></button>" +
        "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#" + settings.name + "' id='" + settings.id + "' ><img src='/../../img/GridBtn/1-6.png' style='height:18px; width:auto;' alt='Настройки'></img></button>" +
         otherFilters);
}

//перечень кнопок для грида
function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift(
        {
        location: "after",
        template: FiltersBeforeGridString(
            { name: 'Add', id: 'Add' },
            { function: 'ReloadingGrid("Grid")', id: 'Reloading', grid: 'Grid' },
            { name: 'ClearFilters', id: 'ClearFilters' },
            { name: 'ExportExcel', id: 'ExportExcel' },
            { name: 'UserSettings', id: 'UserSettings' },
            ''
        )
    });

}



//функция очищения фильтров
function ClearFilters() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    dataGrid.clearFilter();
}

// по двойному щелчку по строке нужно открыть перечень позиции загруженной формы
function onRowClick(e) {
    var component = e.component,
        prevClickTime = component.lastClickTime;
    component.lastClickTime = new Date();
    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
        window.location = '/SVR_loading/SVR_items/?svr_id=' + e.data.id.toString() + "&link_information_param=" + e.data.id.toString();
    }
}



//выгрузка в Excel перечня загрузок
function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/SVR_loading/ExcelReport',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Загрузка СВР");
        }
    });
}

//добавление новой загрузки СВР
function Add() {
    //$('#UploaderFlowWindow').modal('show');    
    $('#ChooseProject').modal("show");
    //$('#GroupUpdate').modal('show');
    
}

//При выборе проекта, по которому производится загрузка, необходимо обновить модель всплывающего окна загрузки
function OnChangeChooseProject() {
    $.ajax({
        async: true,
        cache: false,
        url: '/SVR_loading/UpdateFileUploader',
        type: 'Get',
        data: {
            project_id: $('#notcloseproject').val(),
            project_description: $('#notcloseproject option:selected').html()
        },
        success: function (partialViewResult) {
            document.getElementById("notcloseproject").value = null;
            $("#part").html(partialViewResult);
            $('#ChooseProject').modal('hide');            
            $('#UploaderFlowWindow').modal('show');
        }
    });

}




