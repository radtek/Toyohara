var rebind = false;
var Function_create_or_update = true;
var selectedRecord = '';
var showSelected = null;


function AdditionalParams(controller) {
    var obj = {
        showSelected: showSelected,
        selectedRecord: selectedRecord,
        rebind: rebind,
        storedProcedure: "UI_SELECT_LINK_FUNCTIONS",
        controller: controller,
    };

    return JSON.stringify(obj);
}



function onRowClick(e) {
    var component = e.component,
        prevClickTime = component.lastClickTime;
    component.lastClickTime = new Date();
    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
        //Double click code
        //console.log('double click');
        var dataGrid = $("#Functions_grid").dxDataGrid("instance");
        dataGrid.deselectAll();
        dataGrid.selectRows(e.data.id);
        UpdateFunctionsOpen();

    }
    else {
        //Single click code
        //console.log('single click');
    }
}


$('#Functions_grid table').dblclick(function (event) {
    var dataGrid = $("#Functions_grid").dxDataGrid("instance");
    dataGrid.selectRows($(event).find('.row_id').html());
    UpdateFunctionsOpen();
});
function RemoveFunctions() {
    if (confirm("Вы действительно хотите удалить выбранные записи?")) {
        rebind = true;
        var dataGrid = $("#Functions_grid").dxDataGrid("instance");
        var keys = dataGrid.getSelectedRowKeys();
        if (keys[0] == ',')
            keys[0] = '';
        var FunctionsRecords = [];
        for (var i = 0; i < keys.length; i++) {
            var row_index = dataGrid.getRowIndexByKey(keys[i]);
            row_id = dataGrid.cellValue(row_index, "id");
            FunctionsRecords.push(row_id);
        }
        if (keys.length == 0)
            return alert("Вы не выбрали ни одной строки");

        $.ajax({
            async: true,
            cache: false,
            url: '/Functions/RemoveFunctions',
            type: 'POST',
            data: {
                FunctionsRecords: FunctionsRecords.toString()
            },
            success: function (data) {
                if (data != "" && data != null)
                    alert(data.replace(/__/gi, '\n'));
                $("#Functions_grid").dxDataGrid("getDataSource").reload();
                dataGrid.deselectAll();
                rebind = false;
            }
        });
    }
}
function ClearFilters() {
    var dataGrid = $("#Functions_grid").dxDataGrid("instance");
    dataGrid.clearFilter();
}
function GetLink() {
    var link = '/Functions/Index';
    return link;
}

function ExportExcel() {
    var dataGrid = $("#Functions_grid").dxDataGrid("instance");
    $.ajax({
        async: true,
        cache: false,
        url: '/Functions/ExcelReport',
        type: 'POST',
        data: {
            filter: dataGrid.option("filterValue") ? JSON.stringify(dataGrid.option("filterValue")) : "",
            sort: $("#Functions_grid").dxDataGrid("getDataSource").loadOptions().sort ? JSON.stringify($("#Functions_grid").dxDataGrid("getDataSource").loadOptions().sort) : "",
            showSelected: showSelected,
            selectedRecord: selectedRecord,
        },
        success: function (data) {
            window.location = "/Common/ReturnFile?physicalPath=" + data;
        }
    });
}

function FiltersBeforeGridString(reloading, clearFilters, excel, showSelected, settings, otherFilters) {

    return $("<button class='btn btn_in_grid dx-button btn_pad_grid' title='Искать' onclick='" + reloading.function + "' id='" + reloading.id + "'><img src='/../../img/GridBtn/1-5.png' width='18' height='18' alt='Искать' ></img></button>" +
        "<button title='Очистить фильтры' onclick='" + clearFilters.name + "();' class='btn btn_in_grid dx-button btn_pad_grid' id='" + clearFilters.id + "'><img src='/../../img/GridBtn/1-9.png' width='18' height='18'  alt='Очистить фильтры'></img></button>" +
        "<button title='Выгрузить Excel' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + excel.name + "();' id='" + excel.id + "'><img src='/../../img/GridBtn/1-4.png' width='18' height='18' alt='Выгрузить Excel'></img></button>" +
        "<button title='Показать выбранные' class='btn btn_in_grid dx-button btn_pad_grid' onclick='" + showSelected.function + "' id='" + showSelected.id + "'><img src='/../../img/GridBtn/1-7.png' width='18' height='18' alt='Показать выбранные'></img></button>" +
        "<button title='Настройки' class= 'btn btn_in_grid dx-button btn_pad_grid' data-toggle='modal' data-target='#" + settings.name + "' id='" + settings.id + "' ><img src='/../../img/GridBtn/1-6.png' width='18' height='18' alt='Настройки'></img></button>" +
        otherFilters);
}

function onToolbarPreparing(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift({
        location: "after",
        template: FiltersBeforeGridString(
            { function: 'Reloading("Functions_grid")', id: 'Reloading' },
            { name: 'ClearFilters', id: 'ClearFilters' },
            { name: 'ExportExcel', id: 'ExportExcel' },
            { function: 'showSelectedPicture(this, "Functions_grid");', id: 'showSelectedPicture', grid: 'Functions_grid' },
            { name: 'UserSettings', id: 'UserSettings' },
            "<button type='button' title='Удалить' onclick='RemoveFunctions();' class='btn btn_in_grid dx-button btn_pad_grid'><img src='/../../img/GridBtn/1-3.png' width='18 height='18' alt='Удалить' /></button>" +
            "<button type='button' title='Добавить' onclick='AddFunctionsOpen();' class='btn btn_in_grid dx-button btn_pad_grid'><img src='/../../img/GridBtn/1-1.png' width='18' height='18' alt='Добавить' /></button>" +
            "<button type='button' title='Редактировать' data-toggle='modal' onclick='UpdateFunctionsOpen();' class='btn btn_in_grid dx-button btn_pad_grid'><img src='/../../img/GridBtn/1-2.png' width='18' height='18' alt='Редактировать' /></button>"
        )
    });
}

function AddFunctionsOpen() {

    Function_create_or_update = true;
    $('#AddFunction_title').html('Добавление новой функции');
    $('#FunctionSection').val(0);
    $('#FunctionHeader').val('');
    $('#FunctionStoredProc').val('');
    $('#FunctionHtmlText').summernote('destroy');
    $('#FunctionHtmlText').val('');
    $('#FunctionHtmlText').summernote({
        addclass: {
            debug: true,
            classTags: ["alert alert-danger", "visible-sm", "hidden-xs", "hidden-md", "hidden-lg", "hidden-print"]
        },
        height: 300,
        toolbar: [
            ['img', ['picture']],
            ['style', ['style', 'addclass', 'clear']],
            ['fontstyle', ['bold', 'italic', 'ul', 'ol', 'link', 'paragraph']],
            ['fontstyleextra', ['strikethrough', 'underline', 'hr', 'color', 'superscript', 'subscript']],
            ['extra', ['video', 'table', 'height']],
            ['misc', ['undo', 'redo', 'codeview', 'help']]
        ]
    });
    $('#AddFunctionModal').modal('show');
}
var selected_row;
function UpdateFunctionsOpen() {
    Function_create_or_update = false;
    var dataGrid = $("#Functions_grid").dxDataGrid("instance");
    var keys = dataGrid.getSelectedRowKeys();
    if (keys.length == 0 | keys.length > 1)
        return alert("Выберите одну строку");

    var section_id = 0;
    var title = '';
    var stored_procedure = '';
    var description = '';

    for (var i = 0; i < keys.length; i++) {
        var row_index = dataGrid.getRowIndexByKey(keys[i]);
        section_id = dataGrid.cellValue(row_index, "section_id");
        title = dataGrid.cellValue(row_index, "title");
        stored_procedure = dataGrid.cellValue(row_index, "stored_procedure");
        description = dataGrid.cellValue(row_index, "description");
        selected_row = dataGrid.cellValue(row_index, "id")
    }

    $('#AddFunction_title').html('Изменение функции');
    $('#FunctionSection').val(section_id);
    $('#FunctionHeader').val(title);
    $('#FunctionStoredProc').val(stored_procedure);
    $('#FunctionHtmlText').summernote('destroy');
    $('#FunctionHtmlText').val(description);
    $('#FunctionHtmlText').summernote({
        lang: 'ru-RU',
        height: 500,
        focus: true,
        fontNames: ['Arial', 'Times New Roman', 'Helvetica']

    });
    $('#AddFunctionModal').modal('show');
}

function UpdateFunctions() {
    if ($('#FunctionSection').val() == null | $('#FunctionSection').val() == "" | $('#FunctionSection').val() == "undefined" | $('#FunctionSection').val() == 0)
        return alert('Раздел должен был заполнен');
    if ($('#FunctionHeader').val() == null | $('#FunctionHeader').val() == "" | $('#FunctionHeader').val() == "undefined")
        return alert('Заголовок должен был заполнен');
    //if ($('#FAQ_order_number').val() == null | $('#FAQ_order_number').val() == "" | $('#FAQ_order_number').val() == "undefined")
    //    return alert('Поле порядок должно быть заполнено');          
    $("#AddFunctionModal .note-editor button.active.btn-codeview").click();
    rebind = true;
    var id;
    if (!Function_create_or_update)
        id = selected_row;
    $.ajax({
        async: true,
        cache: false,
        url: '/Functions/UpdateFunctions',
        type: 'POST',
        data: {
            section_id: $('#FunctionSection').val(),
            title: $('#FunctionHeader').val(),
            description: $('#FunctionHtmlText').val().toString(),
            stored_procedure: $('#FunctionStoredProc').val(),
            id: id
        },
        success: function (data) {
            if (data != "" && data != null)
                alert(data);
            else {
                Reloading('Functions_grid');
                $('#AddFunctionModal').modal('hide');
            }
            rebind = false;

        }
    });
}

var records;
function selection_changed(selectedItems) {
    var data = selectedItems.selectedRowsData;
    records = [];
    $.each(data, function (i, val) {
        records.push(val.id);
    });
    selectedRecord = records.toString();
}
