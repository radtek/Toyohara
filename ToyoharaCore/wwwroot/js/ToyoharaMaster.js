Globalize.locale("ru");




function OnCommitSuccess() {
    window.location.reload();
}


function ReloadingGrid(gridName) {
    rebind = true;
    var dataGrid = $("#" + gridName).dxDataGrid("getDataSource");
    dataGrid.reload();
}

function ClearFiltersGrid(gridName) {
    var dataGrid = $("#" + gridName).dxDataGrid("instance");
    dataGrid.clearFilter();
}


function getOnRowPrepared(e) {
    if (e.rowType === 'data') {
        try {
            if (e.data.color.toString() !== '') {
                e.rowElement.css('background-color', '#' + e.data.color.toString());
            }
        }
        catch (err) { ; }
    }
}


function onToolbarPreparingFAQ(e) {
    var dataGrid = e.component;
    e.toolbarOptions.items.unshift({
        location: "after",
        template:
            "<button type='button title='Удалить' onclick='RemoveFAQ();' class='btn btn_margin btn_in_grid btn_icon'><img src='/../../img/GridBtn/1-3.png' width='22' height='22' alt='Удалить' /></button>" +
            "<button type='button' title='Добавить' onclick='AddFAQModalOpen();' class='btn btn_margin btn_in_grid btn_icon'><img src='/../../img/GridBtn/1-1.png' width='22' height='22' alt='Добавить' /></button>" +
            "<button type='button' title='Редактировать' data-toggle='0modal' onclick='UpdateFAQModalOpen();' class='btn btn_margin btn_in_grid btn_icon'><img src='/../../img/GridBtn/1-2.png' width='22' height='22' alt='Редактировать' /></button>"
    })
}

function hover(element, name) {
    if (name == "phone") { element.setAttribute('src', '/../../img/22.png'); $('#Call_link > div').toggle(); }
    if (name == "question") {
        element.setAttribute('src', '/../../img/10.png');
    }
    if (name == "history") {
        element.setAttribute('src', '/../../img/35.png');
    }
}

function unhover(element, name) {
    if (name == "phone") {
        element.setAttribute('src', '/../../img/21.png');

    }
    if (name == "question") {
        element.setAttribute('src', '/../../img/9.png');
    }
    if (name == "history") {
        element.setAttribute('src', '/../../img/34.png');
    }
}
var FAQrecords = [];
var row_id;
var FAQ_create_or_update;

function ClearFilters(gridName) {
    var dataGrid = $("#" + gridName).dxDataGrid("instance");
    dataGrid.clearFilter();
}

function UpdateFAQModalOpen() {
    FAQ_create_or_update = false;
    $('#FAQ_title').html('Редактирование');

    var dataGrid = $("#FAQ_Grid").dxDataGrid("instance");
    var keys = dataGrid.getSelectedRowKeys();
    if (keys.length == 0 | keys.length > 1)
        return alert("Выберите одну строку");

    var list_roles_id;
    var html_text = '';
    var header = '';
    var list = '';
    var order_number = '';

    for (var i = 0; i < keys.length; i++) {
        var row_index = dataGrid.getRowIndexByKey(keys[i]);
        list_roles_id = dataGrid.cellValue(row_index, "list_roles_id").split(',');
        html_text = dataGrid.cellValue(row_index, "html_text");
        header = dataGrid.cellValue(row_index, "header");
        row_id = dataGrid.cellValue(row_index, "id");
        order_number = dataGrid.cellValue(row_index, "order_number");
    }
    $('#FAQ_header').val(header);
    $('#FAQ_order_number').val(order_number);
    if (list_roles_id.length == 1) {
        list = list_roles_id + '';
        if (list != "")
            $('#role_id_list_layout').val(list);
        else
            $('#role_id_list_layout').val([]);
    }
    else
        $('#role_id_list_layout').val(list_roles_id);
    $('#role_id_list_layout').selectator('refresh');

    $('.FAQ_html_text').summernote('destroy');
    $('#FAQ_html_text').val(html_text);
    $('.FAQ_html_text').summernote({
        lang: 'ru-RU',
        height: 500,
        focus: true,
        fontNames: ['Arial', 'Times New Roman', 'Helvetica']
        //,
        //toolbar: [
        //[groupName, [list of button]]
        //['style', ['bold', 'italic', 'underline', 'clear']],
        //['font', ['strikethrough', 'superscript', 'subscript']],
        //['fontsize', ['fontsize']],
        //['color', ['color']],
        //['para', ['ul', 'ol', 'paragraph']],
        //['height', ['height']]
        //]
        //fontname: set font family
        //◦fontsize: set font size
        //◦color: set foreground and background color
        //◦bold: toggle font weight
        //◦italic: toggle italic
        //◦underline: toggle underline
        //◦strikethrough: toggle strikethrough
        //◦superscript: toggle superscript
        //◦subscript: toggle subscript
        //◦clear: clear font style
    });


    $('#ModalFAQ').modal('toggle')

    $("#AddFAQModal").modal('toggle');
}

function AddFAQModalOpen() {
    FAQ_create_or_update = true;
    row_id = null;
    $('#FAQ_title').html('Добавить');
    $('#FAQ_header').val('');
    $('#role_id_list_layout').val([]);
    $('#role_id_list_layout').selectator('refresh');
    $('.FAQ_html_text').summernote('destroy');
    $('#FAQ_html_text').val('');


    $('#FAQ_html_text').summernote({
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

    $('#ModalFAQ').modal('toggle')

    $("#AddFAQModal").modal('toggle');
}


function RemoveFAQ() {
    if (confirm("Вы действительно хотите удалить выбранные записи?")) {
        var dataGrid = $("#FAQ_Grid").dxDataGrid("instance");
        var keys = dataGrid.getSelectedRowKeys();
        if (keys[0] == ',')
            keys[0] = '';
        var FAQrecords = [];
        for (var i = 0; i < keys.length; i++) {
            var row_index = dataGrid.getRowIndexByKey(keys[i]);
            row_id = dataGrid.cellValue(row_index, "id");
            FAQrecords.push(row_id);
        }
        if (keys.length == 0)
            return alert("Вы не выбрали ни одной строки");

        $.ajax({
            async: true,
            cache: false,
            url: '/Common/DeleteFAQ',
            type: 'POST',
            data: {
                FAQrecords: FAQrecords.toString()//,
                //user_id: '@delegated_user.id'
            },
            success: function (data) {

                $("#FAQ_Grid").dxDataGrid("getDataSource").reload();
                dataGrid.deselectAll();
            }
        });
    }
}

function UpdateFAQAdd(info_id) {
    if ($('#FAQ_header').val() == null | $('#FAQ_header').val() == "" | $('#FAQ_header').val() == "undefined")
        return alert('Заголовок должен был заполнен');
    $("#AddFAQModal .note-editor button.active.btn-codeview").click();
    if ($('#FAQ_html_text').val() == null | $('#FAQ_html_text').val() == "" | $('#FAQ_html_text').val() == "undefined")
        return alert('Html текст справки должен был заполнен');
    if (!checkFloat($('#FAQ_order_number').val())) return;
    $.ajax({
        async: true,
        cache: false,
        url: '/Common/UpdateFAQAdd',
        type: 'POST',
        data: {
            FAQ_header: $('#FAQ_header').val(),
            http_text: $('#FAQ_html_text').val(),
            role_id_list: $('#role_id_list_layout').val().toString(),
           // control: control //'@ViewContext.RouteData.Values["Controller"].ToString()',
           // control: control //'@ViewContext.RouteData.Values["Controller"].ToString()',
           // view: view//'@ViewContext.RouteData.Values["Action"].ToString()',
            order_number: $('#FAQ_order_number').val(),
            id: row_id,
            info_id: info_id
        
        },
        success: function (data) {

            $('#span_FAQ').html(data);
            $("#FAQ_Grid").dxDataGrid("getDataSource").reload();
            $('#AddFAQModal').modal('toggle');
            $('#ModalFAQ').modal('toggle');
        }
    });
}
function CheckFAQ() {
    $('#span_FAQ').html($('#admin_FAQ').val());
}
function checkFloat(n) {
    n = n.replace(',', '.');
    if (isNaN(parseFloat(n)) && isFinite(n)) {
        alert("Должно быть указано положительное число c плавающей точкой");
        return false;
    }
    return true;
}
function Reloading(grid, rebindgrid) { rebind = rebindgrid; var dataGrid = $("#" + grid).dxDataGrid("getDataSource"); dataGrid.reload(); }
showSelectedPicture.flag = false;
function showSelectedPicture(element, grid) {
    if (selectedRecord == "" || selectedRecord == null || selectedRecord == "undefined") { return alert("Не выбрано ни одной записи!"); }

    showSelectedPicture.flag = !showSelectedPicture.flag;
    showSelected = showSelectedPicture.flag;
    if (showSelectedPicture.flag) {
        $('#' + element.id).find('img').attr('src', '/../../img/GridBtn/1-8.png');
        $('#' + element.id).find('img').attr('alt', 'Показать все');
        element.setAttribute('title', 'Показать все');
    }
    else {
        $('#' + element.id).find('img').attr('src', '/../../img/GridBtn/1-7.png');
        $('#' + element.id).find('img').attr('alt', 'Показать выбранные');
        element.setAttribute('title', 'Показать выбранные');
    }
    $("#" + grid).dxDataGrid("getDataSource").reload();
}
var fullSearchMass = []
var ekk_codes = [];
var selected_node = [];

var ekk_codesFullSearch = [];
var ekk_text = "";

function SaveFullSearchFilters(fullSearchName, ekkFlowWindowName, ekkTextId) {
    fullSearchMass = fullSearchMass.filter(function (elem) {
        return (elem.name != fullSearchName);
    });

    $('#' + fullSearchName + ' .full_search_check').each(function () {
        fullSearchMass.push({ data_type: $(this).attr('data_type'), id: $(this).attr("id"), value: $(this).prop('checked'), name: fullSearchName });
    });
    $('#' + fullSearchName + ' .full_search_text_select').each(function () {
        fullSearchMass.push({ data_type: $(this).attr('data_type'), id: $(this).attr("id"), value: $(this).val(), name: fullSearchName });
    });
    if (ekkTextId != "") {
        ekk_codesFullSearch = ekk_codes.slice();
        var s = ReturnEkkNodes(ekkFlowWindowName);
        if (s.selected_text != "" && s.selected_node.length != 0)
            $(' #' + ekkTextId).html(s.selected_text + '... ' + ' Всего:' + s.selected_node.length.toString());
        else
            $(' #' + ekkTextId).html('');
    }
}

function RemoveFullSearchFilters(fullSearchName, ekkFlowWindowName, ekkTextId) {
    var fullSearch = fullSearchMass.filter(function (elem) {
        return (elem.name == fullSearchName);
    });

    $.each(fullSearchMass, function (index, vale) {
        if (vale.data_type == 'check')
            $('#' + vale.id).prop("checked", vale.value);
        else
            $('#' + vale.id).val(vale.value);
    });
    if (ekkTextId != "") {
        ekk_codes = ekk_codesFullSearch.slice();
        var s = ReturnEkkNodes(ekkFlowWindowName);
        if (s.selected_text != "" && s.selected_node.length != 0)
            $('#' + ekkFlowWindowName + ' #' + ekkTextId).html(s.selected_text + '... ' + ' Всего:' + s.selected_node.length.toString());
        else
            $('#' + ekkFlowWindowName + ' #' + ekkTextId).html('');
    }
}
$(function () {




    $('#role_id_list_layout').selectator({
        showAllOptionsOnFocus: true,
        searchFields: 'value text subtitle right'
    });

    
    $('ul.nav li.dropdown').hover(function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(500);
    }, function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeOut(500);
    });

    $('#user_btn').hover(function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(500);
    }, function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeOut(500);
    });

    $('#Call_link').hover(function () {
        $(this).find('#phone_text').stop(true, true).delay(200).fadeIn(500);
    }, function () {
        $(this).find('#phone_text').stop(true, true).delay(200).fadeOut(500);
    });

 
});





function UpdateSettingsOfGrid(controller, action, procedure_name, checkboxClass, widthClass, positionClass, str, flowwindow, parsialDivName, openParsialDivFunction) {
    var columns_names = [];
    var columns_is_visible = [];
    var columns_width = [];
    var columns_position = [];
    //console.log(str);
    var width = $('input.' + widthClass);
    //var position = $('.' + positionClass);
    //for (var i = 0; i < width.length; i++) { 
    //    if (!checkFloat($(width[i]).val())) return;
    //    if (!checkFloat($(width[i])).closest('tr').find('.' + positionClass).val()) return;
    //    columns_names.push($(width[i]).closest('tr').find('.' + checkboxClass).val());
    //    columns_is_visible.push($(width[i]).closest('tr').find('.' + checkboxClass).prop("checked").toString());
    //    columns_width.push($(width[i]).val());
    //    columns_position.push($(width[i]).closest('tr').find('.' + positionClass).val());
    //}

    //var leng = $('.' + widthClass).length;
    $('.' + widthClass).each(function () {
        if (!checkFloat($(this).val())) return;
        if (!checkFloat($(this).closest('tr').find('.' + positionClass).val())) return;
        columns_names.push($(this).closest('tr').find('.' + checkboxClass).val());
        columns_is_visible.push($(this).closest('tr').find('.' + checkboxClass).prop("checked").toString());
        columns_width.push($(this).val());
        columns_position.push($(this).closest('tr').find('.' + positionClass).val());

    });
    uniqueArray = columns_position.filter(function (item, pos) {
        return columns_position.indexOf(item) == pos;
    });
    ////if (uniqueArray.length != columns_width.length)
    ////    return alert("!Номера позиций в таблице не могут повторяться!");
    //$('#LoadingGif').show();
    //var str = ;
    $.ajax({
        async: true,
        cache: false,
        url: '/' + controller + '/' + action,
        type: 'POST',
        data: {
            column_names: columns_names.toString(),
            columns_is_visible: columns_is_visible.toString(),
            columns_width: columns_width.toString(),
            procedure_name: procedure_name,
            columns_position: columns_position.toString(),
            defaultSettings: str
        },
        success: function (data) {
            $('#' + flowwindow.toString()).modal('hide');

            if (parsialDivName == "") {
                window.location.href = window.location.href;
            }
            else {
                $('#' + parsialDivName.toString()).modal('hide');
                window[openParsialDivFunction]();
            }
            
            // window.location = link;
        }
    });
    // $('#LoadingGif').hide();
}
//data: { column_names: columns_names.toString(), columns_is_visible: columns_is_visible.toString(), columns_width: columns_width.toString(), procedure_name: procedure_name, columns_position: columns_position, defaultSettings: JSON.stringify(('@*(JsonConvert.SerializeObject(Model.gridSettings))')) },*@

function DefaultSettingsOfGrid(procedure_name, flowwindow, parsialDivName, openParsialDivFunction) {

    $.ajax({
        async: true,
        cache: false,
        //traditional: true,
        url: '/Common/DefaultSettingsOfGrid',
        type: 'POST',
        data: { procedure_name: procedure_name },
        success: function (data) {
            $('#' + flowwindow.toString()).modal('hide');

            if (parsialDivName == "") {
                window.location.href = window.location.href;
            }
            else {
                $('#' + parsialDivName.toString()).modal('hide');
                window[openParsialDivFunction]();
            }
        }
    });
}
function isFloat(val) {
    var floatRegex = /^-?\d+(?:[.,]\d*?)?$/;
    if (!floatRegex.test(val))
        return false;

    val = parseFloat(val);
    if (isNaN(val))
        return false;
    return true;
}

function isInt(val) {
    var intRegex = /^-?\d+$/;
    if (!intRegex.test(val))
        return false;

    var intVal = parseInt(val, 10);
    return parseFloat(val) == intVal && !isNaN(intVal);
}

//function isInt(value) {
//    return !isNaN(value) && (function (x) { return (x | 0) === x; })(parseFloat(value))
//}
//function isFloat(value) {
//    n = value.replace(',', '.');
//    if (isNaN(parseFloat(value)) && isFinite(value)) {        
//        return false;
//    }
//    return true;
//}
//function InsertCard(flowWindowName, gridId, bindning, close_window, storedProcedure, id_func) {
//    var parameters = [];
//    var err = '';
//    var dataGrid = $("#" + gridId).dxDataGrid("instance");
//    bindning = bindning.toLowerCase();
//    close_window = close_window.toLowerCase();

//    var str = $('#' + flowWindowName + ' .grid_field').val();
//    var GridSettings = JSON.parse(str);
//    var grid_settings = GridSettings.filter(function (elem, i) {
//        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id");
//    });
//    $('#' + flowWindowName + ' table tr:visible .td_value.editable').each(function () {
//        if ($(this).attr('requd') == 'required') {
//            if ($(this).attr('element_type') != 'checkbox' && $(this).attr('element_type') != 'date')
//                if ($(this).val() == '') { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag=true; return false; }
//            if ($(this).attr('element_type') == 'date')
//            {
//                var dat = $(this).dxDateBox('instance').option('value');
//                if (dat == "" || dat == null || dat == "undefined"); { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); type_flag=true; return false; }
                              
//            }
//        }
//    });
//    //$('#' + flowWindowName + ' table tr:visible .td_value').children().each(function () {
//    //    if ($(this).attr('requd') == 'required') {
//    //        if ($(this).attr('element_type') != 'checkbox')
//    //            if ($(this).val() == '') { alert('Заполните поля, помеченные звездочкой, они обязательны для заполнения.'); err = 'some_err'; return false; }
//    //    }
//    //});
//    if (type_flag)
//        return;
//    var type_flag = false;
//    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_int.editable').each(function () {
//        if (!isInt($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть целым числом'); type_flag = true; return false;}        
//    });
//    if (type_flag)
//        return;
//    $('#' + flowWindowName + ' table tr:visible .td_value.textbox_float.editable').each(function () {
//        if (!isFloat($(this).val())) { alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть числом с плавающей точкой'); type_flag = true; return false; }
//    });
//    if (type_flag)
//        return;

//    $('#' + flowWindowName + ' table tr:visible .td_value.date.editable').each(function () {
//        var isValid = $(this).dxDateBox('instance').option('isValid');
//        if (!isValid) { type_flag = true; alert("Поле " + $(this).attr('rus_name').toLowerCase() + ' должно быть правильного формата даты'); return false; }
//    });
//    if (type_flag)
//        return;

//    $('#' + flowWindowName + ' table tr:visible .td_value').each(function () {
//        if ($(this).attr('element_type') == 'checkbox')
//            parameters.push({ Name: $(this).attr('name'), Value: $(this).prop('checked') });
//        else if ($(this).attr('element_type') == 'date') {
//            var dat = $(this).dxDateBox('instance').option('value');
//            if (dat != "" && dat != null && dat != "undefined")
//                dat = dat.toString().replace(/UTC\s/, "").replace(/GMT.+/, "");
//            else
//                dat = null
//            parameters.push({ Name: $(this).attr('name'), Value:dat  });

//        }else if ($(this).attr('element_type') == 'div_html')
//            parameters.push({ Name: $(this).attr('name'), Value: $(this).html().toString() });
//        else
//            parameters.push({ Name: $(this).attr('name'), Value: $(this).val() != null ? $(this).val().toString() : '' });
//    });

//    if (err != '')
//        return;
//        parameters.push({ Name: 'storedProcedure', Value: storedProcedure });
 
//        parameters.push({ Name: 'id', Value: object_id[id_func]() });
    
   

//    var selectedData = dataGrid.getSelectedRowsData()[0];

//    for (var propt in selectedData) {

//        var index = -1;
//        for (var i = 0; i < parameters.length; i++)
//        {
//            if (parameters[i].Name == propt) {
//                index = 1;
//                break;
//            }
//        }

//        if (index == -1)
//            parameters.push({ Name: propt, Value: selectedData[propt] });
//    }

//    $.ajax({
//        async: true,
//        cache: false,
//        traditional: true,
//        url: '/Common/InsertCard',
//        type: 'POST',
//        data: {
//            procedureParams: JSON.stringify(parameters)
//        },
//        success: function (data) {
//            if (data != "")
//                alert(data);
//            else {
//                if (bindning) {
//                    var dataGrid = $("#" + gridId).dxDataGrid("instance");
//                    rebind_obj.rebind = true;
//                    dataGrid.refresh();
//                    dataGrid.deselectAll();
//                }
//                if (close_window) {
//                    $('#' + flowWindowName).modal("hide");
//                }
//            }
//        }
//    });
//}

//function OpenCard(AddUpdateFlag, CardId, GridId, ParamStr) {
//    //var data = selectedItems.selectedRowsData;
//    //records = [];
//    //$.each(data, function (i, val) {
//    //    records.push(val.id);

//    //});
//    //$('#' + CardId + ' tr').show();
//    var str = $('#' + CardId + ' .grid_field').val();
//    var GridSettings = JSON.parse(str);
//    var Param = null;
//    if (ParamStr != null)
//        Param = ParamObj[ParamStr]();


//    // selectedData[0][grid_settings[a].field_description]
//    var dataGrid = $("#" + GridId).dxDataGrid("instance");
//    var selectedKeys = dataGrid.getSelectedRowKeys();
//    var selectedData = dataGrid.getSelectedRowsData();
//    if (AddUpdateFlag) {


//        $('#' + CardId + ' .text_b').each(function () {
//            $(this).val(selectedData[0][$(this).attr("name")]);
//        });
//        $('#' + CardId + ' .checkbox').each(function () {
//            $(this).prop('checked', selectedData[0][$(this).attr("name")]);
//        });
//        $('#' + CardId + ' .date').each(function () {
//            var disable = $(this).hasClass('non_editable');//attr('disabled');
//            var date = selectedData[0][$(this).attr("name")];
//            if (date == "" | date == "undefined" | date == null)
//                date = null;


//            //if (disable == "disabled")
//            //    disable = true
//            //else
//            //    disable = false;

//            if (date == null) {
//                $(this).dxDateBox({
//                    //min: new Date(2000, 0, 1),
//                    //max: new Date(2029, 11, 31),
//                    disabled: disable
//                });
//            }
//            else {
//                $(this).dxDateBox({
//                    //min: new Date(2000, 0, 1),
//                    //max: new Date(2029, 11, 31),
//                    disabled: disable,
//                    value: new Date(date)
//                });
//            }
//            //$(this).prop('checked', selectedData[0][$(this).attr("name")]);
//        });
//        $('#' + CardId + ' .div_val').each(function () {
//            $(this).html(selectedData[0][$(this).attr("name")]);
//        });

//        $('#' + CardId + ' .dropdown_list_description').each(function () {
//            $(this).html(selectedData[0][$(this).attr("name")]);
//        });

//        $('#' + CardId + ' .dropdown_description').each(function () {
//            $(this).html(selectedData[0][$(this).attr("name")]);
//        });


//        //$('#' + CardId + ' .non_editable.date').each(function () {
//        //    $(this).html(selectedData[0][$(this).attr("name")]);
//        //}); 
//        $('#' + CardId + ' .non_editable').show();
//        $('#' + CardId + ' .null_type').show();
//        $('#' + CardId + ' .dropdown_list_description').show();
//        $('#' + CardId + ' .dropdown_description').show();

//    }
//    else {
//        $('#' + CardId + ' .date').each(function () {
//            $(this).dxDateBox({
//                //min: new Date(2000, 0, 1),
//                //max: new Date(2029, 11, 31),
//                /* value:''*//* new Date('')*/
//            });
//            //$(this).prop('checked', selectedData[0][$(this).attr("name")]);
//        });

//        $('#' + CardId + ' .text_b').each(function () {
//            $(this).val('');
//        });

//        $('#' + CardId + ' .checkbox').each(function () {
//            $(this).prop('checked', false);
//        });

//    }
//    var grid_settings = GridSettings.filter(function (elem, i) {
//        return (elem.ui_type == "dropdown_id" | elem.ui_type == "dropdown_list_id" | elem.ui_type =="dropdown_txt_id");
//    });
//    //var dropdowns = [];
//    //for (var i = 0; i < grid_settings.length; i++) {
//    //    dropdowns.push(grid_settings[i].dropdown);
//    //}
//      //dropdowns: dropdowns,

//    $.ajax({
//        async: true,
//        cache: false,
//        traditional: true,
//        url: '/Common/DropDownCard',
//        type: 'POST',
//        data: {
//            grid_settings: grid_settings,
          
//            param: Param
//        },
//        success: function (data) {
//            var json_data = JSON.parse(data);
//            for (var a = 0; a < json_data.length; a++) {
//                $('#card' + grid_settings[a].field_description).html(json_data[a]);
//                try { $('#card' + grid_settings[a].field_description).selectator('destroy'); } catch (e) { }
//                if (AddUpdateFlag) {
//                    try {
//                        if (selectedData[0][grid_settings[a].field_description] != "" && selectedData[0][grid_settings[a].field_description] != null && selectedData[0][grid_settings[a].field_description] != "undefined") {
//                            if (grid_settings[a].ui_type != "dropdown_list_id")
//                                $('#card' + grid_settings[a].field_description).val(selectedData[0][grid_settings[a].field_description]);
//                            else {
//                                try {
//                                    var arr = [];
//                                    arr = selectedData[0][grid_settings[a].field_description].split(',');
//                                    $('#card' + grid_settings[a].field_description).val(arr)
//                                }
//                                catch (exc) { ; }
//                            }

//                        }

//                        //alert(selectedData[0][grid_settings[a].field_description]);
//                        //alert(grid_settings[a].field_description);
//                    }
//                    catch (ex) { ; }
//                    //$('#' + CardId + ' .dropdown_list_description').show();
//                    //$('#' + CardId + ' .dropdown_description').show();

//                    //$('#' + CardId + ' .non_editable').hide();
//                    //$('#' + CardId + ' .null_type').show();

//                    //$('#' + CardId + ' .dropdown_description').hide();
//                    //$('#' + CardId + ' .dropdown_list_description').();
//                    //if (selectedData[0][grid_settings[a].field_description] != "" && selectedData[0][grid_settings[a].field_description] != null && selectedData[0][grid_settings[a].field_description] != "undefined")
//                    //    $('#card' + grid_settings[a].field_description).val(selectedData[0][grid_settings[a].field_description]);

//                }
//                else {


//                    try { $('#card' + grid_settings[a].field_description).selectator('destroy'); } catch (e) { }


//                }

//                // try { $('#card' + grid_settings[a].field_description).selectator('refresh'); } catch (ex) { ;}
//                //$('#card' + grid_settings[a].field_description).selectator('refresh');

//                $('#card' + grid_settings[a].field_description).selectator({
//                    showAllOptionsOnFocus: true,
//                    searchFields: 'value text subtitle right'
//                });
//                //window.location = "/Common/ReturnFile?physicalPath=" + data;
//                //$('#card' + grid_settings[i].field_description).html(data);
//                //if (AddUpdateFlag) {
//                //    try {
//                //        $('#card' + grid_settings[i].field_description).val(selectedData[grid_settings[i].field_description]);
//                //    }
//                //    catch (ex) { ; }
//                //}
//                //$('#card' + grid_settings[i].field_description).selectator({
//                //    showAllOptionsOnFocus: true,
//                //    searchFields: 'value text subtitle right'
//                //});
//            }

//            if (AddUpdateFlag) {
//                $('#' + CardId + ' .dropdown_id.non_editable').each(function () {
//                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                    $(this).closest('tr').hide();
//                    $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
//                });
//                $('#' + CardId + ' .dropdown_id.editable').each(function () {
//                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                    $(this).closest('tr').show();
//                    $('#' + CardId + ' .dropdown_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                });
//                $('#' + CardId + ' .dropdown_list_id.non_editable').each(function () {
//                    //$('#' + CardId + ' .dropdown_list_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                    $(this).closest('tr').hide();
//                    $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').show();
//                });
//                $('#' + CardId + ' .dropdown_list_id.editable').each(function () {
//                    //$('#' + CardId + ' .dropdown_id[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                    $(this).closest('tr').show();
//                    $('#' + CardId + ' .dropdown_list_description[drop="' + $(this).attr('drop') + '"]').closest('tr').hide();
//                });
//                //    ($('#card'+$(this).attr("name")+'_id')).closest('tr').hide();
//                //});
//                //$('#' + CardId + ' .non_editable').show();
//                $('#' + CardId + ' .non_editable').each(function () { $(this).attr('disabled', 'disabled'); });
//                $('#' + CardId + ' .global_non_visible').hide();
//            }
//            else {
//                $('#' + CardId + ' .non_editable').each(function () { $(this).removeAttr('disabled'); });
//                $('#' + CardId + ' .non_editable').hide();
//                $('#' + CardId + ' .null_type').hide();
//                $('#' + CardId + ' .dropdown_list_description').hide();
//                $('#' + CardId + ' .dropdown_description').hide();
//                $('#' + CardId + ' .global_non_visible').hide();
//                $('#' + CardId + ' .dropdown_list_id').find($('option')).attr('selected', false);
//                $('#' + CardId + ' .dropdown_id').find($('option')).attr('selected', false);
//            }
//        }
//    });

//    $('#' + CardId).modal("show");
//}





