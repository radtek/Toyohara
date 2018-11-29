var selectedRecord = '';
var showSelected = null;
var records;
rebind = true;

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
        storedProcedure: "OMC_SELECT_FORMS",
        controller: controller,
        hide_archive: document.getElementById('hide_archive').checked
    };
    return JSON.stringify(obj);
}



//маппинг кнопок и действия
function FiltersBeforeGridString(reloading, clearFilters, fullSearch, excel, settings, otherFilters) {
    return $("<button class='btn btn_in_grid dx-button btn_pad_grid' title='Обновить' onclick='" + reloading.function + "' id='" + reloading.id + "'><img src='/../../img/GridBtn/1-5.png' style='height:18px; width:auto;' alt='Обновить' ></img></button>" +
        "<button title='Очистить фильтры' onclick='" + clearFilters.name + "();' class='btn btn_in_grid dx-button btn_pad_grid' id='" + clearFilters.id + "'><img src='/../../img/GridBtn/1-9.png' style='height:18px; width:auto;'  alt='Очистить фильтры'></img></button>" +
        "<button title='Расширенный поиск'  class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' onclick='" + fullSearch.function + "' id='" + fullSearch.id + "' ><img src='/../../img/GridBtn/1-10.png' style='height:18px; width:auto;' alt='Расширенный поиск'></img></button>" +
        "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + excel.name + "();' id='" + excel.id + "'><img src='/../../img/GridBtn/1-4.png' style='height:18px; width:auto;' alt='Выгрузить Excel'></img></button>" +
        "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#" + settings.name + "' id='" + settings.id + "' ><img src='/../../img/GridBtn/1-6.png' style='height:18px; width:auto;' alt='Настройки'></img></button>" +                
        "<button title='История изменения' class='btn btn_in_grid dx-button btn_pad_grid' onclick='OpenLog()' id='o'><img src='/../../img/GridBtn/1-12.png' style='height:18px; width:auto;' alt='История изменения'></img></button>" +
        otherFilters);
}

//перечень кнопок для грида
function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift({
        location: "after",
        template: FiltersBeforeGridString(
            { function: 'ReloadingGrid("Grid")', id: 'Reloading', grid: 'Grid' },
            { name: 'ClearFilters', id: 'ClearFilters' },
            { function: 'SaveFullSearchFilters("FullSearch", "", ""); $("#FullSearch").modal("show");', id: 'FullSearch' },
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
        var dataGrid = $("#Grid").dxDataGrid("instance");
        dataGrid.deselectAll();
        dataGrid.selectRows(e.data.id);
        try {
            if (e.data.form_id.toString() == '5') {
                window.location = '/Forms/SVRItems/?form_id=' + e.data.id.toString() + "&link_information_param=" + e.data.id.toString();
            }
            if (e.data.form_id.toString() == '4') {
                window.location = '/Forms/RSSItems/?form_id=' + e.data.id.toString() + "&link_information_param=" + e.data.id.toString();
            }
        }
        catch (err) { }
    }
    else {
        //Single click code
        //console.log('single click');
    }

}

//выгрузка в Excel перечня загрузок
function ExportExcel() {
    var dataGrid = $("#Grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/Forms/ExcelReport',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
            hide_archive: document.getElementById('hide_archive').checked
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data + "&fileDownloadName=" + encodeURIComponent("Формы для расценки");
        }
    });
}







