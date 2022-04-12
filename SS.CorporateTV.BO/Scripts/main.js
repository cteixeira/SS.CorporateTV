﻿/* Begin configurações*/

var formataData = 'DD/MM/YYYY'; //formato para o moment apresentar as datas
var separadorData = '  |  '; //formato para o moment apresentar as datas

$.ajaxSetup({ cache: false }); //previne cache dos pedidos ajax no IE

/* End configurações*/

var isTriggered = false;

function detailOnClick() {
    if ($('#divModal').length && $('#divModal').is(":visible")) { //abrir pop-up dentro de pop-up
        triggerClickModal('table tbody tr[id]', '#formgetrec');
        triggerClickModal('#divModal table tbody tr[id]', '#formgetrecsub');
    }
    else if ($('#formgetrec').length)
        triggerClickModal('table tbody tr[id]', '#formgetrec');
    else //se não existe form deve ser feito redirect
        triggerClickRedirect('table tbody tr[id]');
}

function triggerClickModal(selector, form) {
    $(selector).off('click').off('dblclick').on('click', function (event) {

        $(form).find('#id').val($(this).attr('id'));
        isTriggered = true;
        $(form).find(':submit').trigger('click');

    }).on('dblclick', function (e) {
        return false;
    });
    $('input').off('click').on('click', function (event) { event.stopPropagation(); });
    $('select').off('click').on('click', function (event) { event.stopPropagation(); });
}

function triggerClickRedirect(selector) {
    $(selector).off('click').off('dblclick').on('click', function (event) {
        window.location = window.location + '/Detalhe/' + $(this).attr('id');
    }).on('dblclick', function (e) {
        return false;
    });
}

function ClearValidators() {
    //remove a porcaria dos validators de tipo de dados
    $("input[data-val-date]").removeAttr("data-val-date");
    $("input[data-val-number]").removeAttr("data-val-number");
}

function ResetValidators() {
    ClearValidators();
    //é necessário registar os validators para eles dispararem
    $.validator.unobtrusive.parse($('form'));
}

function Notifica(message, styleClass) {
    $.notify(message, { className: styleClass });
}

function MostraModal(selector, showModal) {
    $(selector).modal({ show: showModal, backdrop: false, keyboard: true })
    isTriggered = false;
    ResetValidators();
}

function EscondeModal(id) {
    if (!id) {
        if ($('#divModalSub').length && $('#divModalSub').is(":visible"))
            id = '#divModalSub';
        else
            id = '#divModal';
    }
    else id = '#' + id;
    $(id).modal('hide');
}


//---- ini Get Data ----
var keysOut = [40, 35, 13, 27, 36, 37, 34, 33, 39, 32, 9, 38]; //left, up, enter...
var delayCall = null;
var searchData;

function registaPesquisa(url) {

    $("input#Pesquisa").on('keyup', function (d) {
        if ($.inArray(d.keyCode, keysOut) == -1) {
            PesquisaIconChange($("#ispan"), $("#iicon"), $("input#Pesquisa"));
            if (url) {
                clearTimeout(delayCall);
                delayCall = setTimeout(function () { ActualizaLista(url); }, 300);
            }
        }
    });

    $("#ispan").on('click', function (d) {
        DeleteSearch();
        if (url)
            ActualizaLista(url);
    });
}

function registaPesquisaDateRange(url, useDetails) {
    searchData = $("#filtro input").serialize();
    $("input#Pesquisa").on('keyup', function (d) {
        if ($.inArray(d.keyCode, keysOut) == -1) {
            PesquisaIconChange($("#ispan"), $("#iicon"), $("input#Pesquisa"));
            searchData = $("#filtro input").serialize();
            clearTimeout(delayCall);
            delayCall = setTimeout(function () { ActualizaLista(url, useDetails); }, 500);
        }
    });

    $("#ispan").on('click', function (d) {
        DeleteSearch();
        searchData = { Pesquisa: "" };
        ActualizaLista(url, useDetails);
    });
}

function ActualizaLista(requestRelativeUri, useDetails) {
    searchData = $("#filtro input,#filtro select").serialize();
    countRec = 0;
    GetMorePrepare();
    $.ajax({
        type: 'GET',
        contentType: "application/html; charset=utf-8",
        url: requestRelativeUri,
        data: searchData + "&skip=" + countRec,
        success: function (data) {
            $('#ListaConteudo').html(data);
            if (useDetails == undefined || useDetails == null || useDetails == true)
                detailOnClick();
            GetMoreComplete();
        }
    });
}
//---- end Get Data ----


//---- ini Get More Data ----
var getMoreUrl;
var can_load = true;
var cheking_scroll = false;
var takeIni;
var takeNext;
var countRec = 0;

function registaGetMore(url, tkIni, tkNext) {
    getMoreUrl = url;
    takeIni = tkIni;
    takeNext = tkNext;

    countRec = $('.linha').length;
    if (takeIni > countRec) {
        can_load = false;
        $('#btGetMore').css('display', 'none');
    }

}

function GetMorePrepare() {
    $('#btGetMore').css('display', 'none');
    $('.loader').css('display', 'block');
}

function GetMore() {
    if (can_load) {
        can_load = false;
        GetMorePrepare();
        $.ajax({
            type: 'GET',
            contentType: "application/html; charset=utf-8",
            url: getMoreUrl,
            data: searchData + "&skip=" + countRec,
            success: function (data) {
                $('#ListaConteudo').append(data);
                //detailOnClick();
                GetMoreComplete();
            }
        });
    }
}


function GetMoreComplete() {
    $('.loader').css('display', 'none');
    var totRec = $('.linha').length;
    if (totRec >= takeIni && totRec > countRec && totRec % takeNext == 0) {
        can_load = true;
        $('#btGetMore').css('display', 'block');
    } else {
        can_load = false;
        $('#btGetMore').css('display', 'none');
    }
    countRec = totRec;
}


$(window).bind('scroll', function (e) {
    if ($('#btGetMore').length > 0 && !cheking_scroll && can_load && isOnScreen($('#btGetMore'))) {
        cheking_scroll = true;
        setTimeout(function () { cheking_scroll = false; }, 300);
        GetMore();
    }
});

function isOnScreen(element) {
    var curPos = element.offset();
    var curBottom = curPos.top + (element.height() / 2);
    if (/webkit.*mobile/i.test(navigator.userAgent)) {
        //corrigir bug do offset no iphone
        curBottom -= 50;
    }
    var screenHeight = $(window).height();
    return (curBottom > (screenHeight + $(window).scrollTop())) ? false : true;
}
//---- end Get More Data ----


function CallDetailAfterSave(id) {
    $('#formgetrec').find('#id').val(id);
    $('#submitgetrec').trigger('click');
}

function RemoverAcentos(string) {
    string = string.replace(new RegExp('[ÁÀÂÃ]', 'gi'), 'a');
    string = string.replace(new RegExp('[ÉÈÊ]', 'gi'), 'e');
    string = string.replace(new RegExp('[ÍÌÎ]', 'gi'), 'i');
    string = string.replace(new RegExp('[ÓÒÔÕ]', 'gi'), 'o');
    string = string.replace(new RegExp('[ÚÙÛ]', 'gi'), 'u');
    string = string.replace(new RegExp('[Ç]', 'gi'), 'c');
    return string;
}

/* *
 * span - span element id, example: $('#spanid') that contains the icon
 * icon - icon element id, example: $('#iconid') the icon
 * pesquisa - pesquisa textbox element id, example: $('#Pesquisa') the textbox
 * url - the string that for the ActualizaLista
 * */

function DeleteSearch(selectortarget, spantarget, icontarget) {
    var pesquisaSelector = selectortarget || "#Pesquisa";
    var spanSelector = spantarget || "#ispan";
    var iconSelector = icontarget || "#iicon";
    var span = $(spanSelector);
    var icon = $(iconSelector);
    var pesquisa = $(pesquisaSelector);

    pesquisa.val('');
    icon.removeClass('glyphicon-remove').addClass('glyphicon-search');
    icon.css("color", "#555");
    span.css("background-color", "#eee");
    span.css("border-color", "#ccc");
    span.css("cursor", "default");
}

/* *
 * span - span element id, example: $('#spanid') that contains the icon
 * icon - icon element id, example: $('#iconid') the icon
 * pesquisa - pesquisa textbox element id, example: $('#Pesquisa') the textbox
 * url - the string that for the ActualizaLista
 * */

function PesquisaIconChange(span, icon, pesquisa) {
    if (pesquisa.val().length > 0) {
        icon.removeClass('glyphicon-search').addClass('glyphicon-remove');
        icon.css("color", "#fff");
        span.css("background-color", "#d9534f");
        span.css("border-color", "#d43f3a");
        span.css("cursor", "pointer");
    } else {
        icon.removeClass('glyphicon-remove').addClass('glyphicon-search');
        icon.css("color", "#555");
        span.css("background-color", "#eee");
        span.css("border-color", "#ccc");
        span.css("cursor", "default");
    }
}

var emEspera;

function StartWaiting(elemento) {
    emEspera = elemento;
    var children = $(emEspera).children();
    for (var i = 0; i < children.length; ++i)
        children[i].style.display = 'none';

    //$(emEspera + ':not(:has(.formLoader))').append($('.formLoader:first'));
    //$('.formLoader').css('display', 'block');
}

function StopWaiting(display) {
    var children = $(emEspera).children();
    for (var i = 0; i < children.length; ++i)
        children[i].style.display = display;

    //$('.formLoader').css('display', 'none');
}

// Para impedir o scroll de mover a página em background
$(window).bind('load', function() {
    $('.modal')
        .on('shown', function() {
            console.log('show');
            $('body').css({ overflow: 'hidden' });
        })
        .on('hidden', function() {
            $('body').css({ overflow: '' });
        });
});


//---- ini Get Tab Content ----
var activeTab;

function tabOnClick() {
    var selector = getTabSelector();
    $(selector + ' a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        activeTab = $(e.target).attr('id');
        getTab();
    });
}

function getTab(sel) {
    var selector = sel || getTabContentSelector();
    $.ajax({
        type: 'GET',
        contentType: "text/html; charset=utf-8",
        url: activeTab,
        success: function (data) {
            $(selector).html(data);
            ResetValidators();
        },
        fail: function (data) {
            alert("Ocorreu um erro.");
        }
    });
}

function getTabSelector() {
    var selector = ".main-block-body #myTab li";
    if ($('#divModal').length)
        selector = ".modal-body #myTab li";
    return selector;
}

function getTabContentSelector() {
    var selector = "#tabcontent";
    if ($('#divModal:visible').length)
        selector = "#divModal #tabcontent";
    return selector;
}
//---- end Get Tab Content ----


function ListarFicheirosIds(divId) {
    var hfValidar = '';
    $('#' + divId + ' input:checkbox.Validar:checked').each(function () {
        var itemId = $(this).attr("id");
        hfValidar = hfValidar.concat(itemId + ',');
    });

    if (hfValidar == '') {
        //Notifica("Selecione os ficheiros que pretende apagar !", "alert");
        return false;
    }

    var old_fulladdr = $('#btnApagar').attr('href');

    //var old_addr_parts = old_fulladdr.split('?');

    var new_querystring = '&ids=' + hfValidar.slice(0, -1);
    var index = old_fulladdr.indexOf('&ids=');
    if (index > 0)
        old_fulladdr = old_fulladdr.substring(0, index);

    $('#btnApagar').attr('href', old_fulladdr + new_querystring);
}

function ListarSortedListIds(divId) {
    var hfValidar = '';
    $('#' + divId + ' li').each(function () {
        var itemId = $(this).attr("id");
        hfValidar = hfValidar.concat(itemId + ',');
    });

    if (hfValidar == '') {
        //Notifica("Selecione os ficheiros que pretende apagar !", "alert");
        return false;
    }

    var old_fulladdr = $('#btnGravar').attr('href');

    //var old_addr_parts = old_fulladdr.split('?');

    var new_querystring = '&ids=' + hfValidar.slice(0, -1);
    var index = old_fulladdr.indexOf('&ids=');
    if (index > 0)
        old_fulladdr = old_fulladdr.substring(0, index);

    $('#btnGravar').attr('href', old_fulladdr + new_querystring);
}


function NovoFicheiro(url, div) {
    if (ValidaNovoFicheiro(div)) {
        $('.Designacao').removeClass('input-validation-error');
        $('.Duracao').removeClass('input-validation-error');
        $('.file').removeClass('input-validation-error');

        var formHasFile = false, formHasNome = false, formHasExtansao = false;
        var formData = new FormData();
        var files = $('div#' + div + ' input.file').prop("files");
        var ficheiros = new Array();
        var numFicheiros = files.length;
        if (files && files.length > 0) {
            for (var i = 0; i < files.length; ++i) {
                formData.append("FileUpload", files[i]);
                var ficheiro = new Object();
                formHasFile = true;
                var splitDot = files[0].name.split('.');
                if (splitDot) {
                    var fileExtension = splitDot[splitDot.length - 1];
                    if (fileExtension) {
                        ficheiro.Extensao = fileExtension;
                        formHasExtansao = true;
                    }
                }

                var fileName = $('div#' + div + ' #Designacao' + (i == 0 ? '' : i)).val();

                if (fileName)
                    ficheiro.Designacao = fileName;
                else
                    ficheiro.Designacao = 'default';

                formHasNome = true;

                var duracao = $('div#' + div + ' #Duracao' + (i == 0 ? '' : i)).val();
                if (duracao)
                    ficheiro.Duracao = duracao;
                else
                    ficheiro.Duracao = '1';

                ficheiros.push(ficheiro);

            }
            formData.append('registos', JSON.stringify(ficheiros));

            if (formHasFile && formHasNome && formHasExtansao) {
                var selector = getTabContentSelector();

                $.ajax({
                    url: url,
                    data: formData,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        $('#' + div).modal('hide');
                        $('.Designacao').val(null);
                        $('.file').val(null);
                        $('.checkbox').prop('checked', false);
                        
                        if (data[0] != 'N') {
                            if (numFicheiros > 1)
                                Notifica('Imagens adicionadas com sucesso!', 'success');
                            else
                                Notifica('Imagem adicionada com sucesso!', 'success');
                            $(selector).html(data);
                        }
                        StopWaiting('inline');
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        Notifica('Ocorreu um erro ao enviar o ficheiro, tamanho máximo permitido 8Mb', "alert");
                        StopWaiting('inline');
                    }
                });
            } else {
                Notifica('Aviso: O ficheiro poderá estar corrompido.', "alert");
                StopWaiting('inline');
            }
            //}
        }
    } else {
        Notifica('Os campos assinalados são obrigatórios', "alert");
        StopWaiting('inline');
    }
}

function ValidaNovoFicheiro(div) {
    var validated = true;

    $('div#' + div + ' input[id^="Designacao"]').each(function () {
        if ($(this).val() == '') {
            $(this).addClass('input-validation-error');
            $('div#' + div + ' #NomeValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
            validated = false;
        }
    });

    $('div#' + div + ' input[id^="Duracao"]').each(function () {
        if ($(this).val() == '') {
            $(this).addClass('input-validation-error');
            $('div#' + div + ' #DuracaoValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
            validated = false;
        }
    });

    if (!$('div#' + div + ' .file').val()) {
        $('div#' + div + ' .file').addClass('input-validation-error');
        $('div#' + div + ' #fileValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
        validated = false;
    }
    return validated;
}

function LimpaValidators() {
    $('.Designacao').removeClass('input-validation-error');
    $('.file').removeClass('input-validation-error');
    $('#NomeValidationMessage').addClass('field-validation-valid').removeClass('field-validation-error').val(null);
    $('#fileValidationMessage').addClass('field-validation-valid').removeClass('field-validation-error').val(null);
}


function RemoverDivs(divId) {
    $('div[id^="' + divId + '"]').remove();
}

function MudaNomeFicheiro(divId) {
    $('div[id^="MudarNomeDiv"]').find('input:text').val('');
    $('div[id^="MudarNomeDiv_"]').remove();
    var fileInput = $('div#' + divId + ' input.file');

    for (var i = 0; i < fileInput.get(0).files.length; ++i) {
        if (i > 0) {
            var novaDiv = $('div#MudarNomeDiv').clone()
                .insertAfter('div#MudarNomeDiv')
                .attr("id", "MudarNomeDiv_" + i)
                .find("*")
                .each(function () {
                    var id = this.id || "";
                    if (id == "Designacao") {
                        this.value = fileInput.get(0).files[i].name.split('.')[0];
                        this.id = id + (i);
                    }
                    else if (id == "extensaoFicheiro") {
                        this.value = fileInput.get(0).files[i].fileExtension;
                        this.id = id + (i);
                    }
                    else if (id == "Duracao") {
                        this.value = '1';
                        this.id = id + (i);
                    }
                });
        }
        else if (i == 0) {
            $('div#MudarNomeDiv #Designacao').val(fileInput.get(0).files[i].name.split('.')[0]);
            $('div#MudarNomeDiv #extensaoFicheiro').val(fileInput.get(0).files[i].fileExtension);
            $('div#MudarNomeDiv #Duracao').val('1');
        }
    }
}

function NovoVideo(url, div) {
    if (ValidaNovoVideo(div)) {
        $('.Designacao').removeClass('input-validation-error');
        $('.Url').removeClass('input-validation-error');
        $('.Duracao').removeClass('input-validation-error');

        var formHasFile = false, formHasNome = false, formHasExtansao = false;
        var formData = new FormData();
        
        var ficheiro = new Object();

        var fileName = $('div#' + div + ' #Designacao').val();

        if (fileName)
            ficheiro.Designacao = fileName;
        else
            ficheiro.Designacao = 'default';

        var urlvideo = $('div#' + div + ' #Url').val();

        if (urlvideo)
            ficheiro.Url = urlvideo;
        else
            ficheiro.Url = '';

        var duracao = $('div#' + div + ' #Duracao').val();
        if (duracao)
            ficheiro.Duracao = duracao;
        else
            ficheiro.Duracao = '';

        formData.append('registos', JSON.stringify(ficheiro));
        var selector = getTabContentSelector();
        $.ajax({
            url: url,
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false,
            success: function (data) {
                $('#' + div).modal('hide');
                $('.Designacao').val(null);
                $('.checkbox').prop('checked', false);
                //Notifica('Vídeo adicionado com sucesso!', 'success');

                if (data[0] != 'N') {
                    $(selector).html(data);
                }
                StopWaiting('inline');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                Notifica('Ocorreu um erro ao adicionar o vídeo', "alert");
                StopWaiting('inline');
            }
        });

    } else {
        Notifica('Os campos assinalados são obrigatórios', "alert");
        StopWaiting('inline');
    }
}

function ValidaNovoVideo(div) {
    var validated = true;

    $('div#' + div + ' input[id^="Designacao"]').each(function () {
        if ($(this).val() == '') {
            $(this).addClass('input-validation-error');
            $('div#' + div + ' #NomeValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
            validated = false;
        }
    });

    $('div#' + div + ' input[id^="Url"]').each(function () {
        if ($(this).val() == '') {
            $(this).addClass('input-validation-error');
            $('div#' + div + ' #UrlValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
            validated = false;
        }
    });

    $('div#' + div + ' input[id^="Duracao"]').each(function () {
        if ($(this).val() != '') {
            var value = $(this).val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
            var intRegex = /^\d+$/;
            if (!intRegex.test(value)) {
                $(this).addClass('input-validation-error');
                $('div#' + div + ' #DuracaoValidationMessage').addClass('field-validation-error').removeClass('field-validation-valid').val('Obrigatório');
                validated = false;
            }
        }
    });

    return validated;
}

function VideoPauseStream() {
    //var ua = window.navigator.userAgent;
    //var msie = ua.indexOf("MSIE ");

    //if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        var iddivvideo = document.getElementById("iddivvideo");
        while (iddivvideo.firstChild)
            iddivvideo.removeChild(iddivvideo.firstChild);
    //}
    //else {
    //    var idembed = document.getElementById("embedid");
    //    idembed.playlist.stop();
    //}
}

function VideoPause() {
        $("#video")[0].pause();
}



/*
PluginDetect v0.9.1
www.pinlady.net/PluginDetect/license/
[ VLC ]
[ isMinVersion getVersion hasMimeType ]
[ AllowActiveX ]
*/
(function () { var j = { version: "0.9.1", name: "PluginDetect", addPlugin: function (p, q) { if (p && j.isString(p) && q && j.isFunc(q.getVersion)) { p = p.replace(/\s/g, "").toLowerCase(); j.Plugins[p] = q; if (!j.isDefined(q.getVersionDone)) { q.installed = null; q.version = null; q.version0 = null; q.getVersionDone = null; q.pluginName = p; } } }, uniqueName: function () { return j.name + "998" }, openTag: "<", hasOwnPROP: ({}).constructor.prototype.hasOwnProperty, hasOwn: function (s, t) { var p; try { p = j.hasOwnPROP.call(s, t) } catch (q) { } return !!p }, rgx: { str: /string/i, num: /number/i, fun: /function/i, arr: /array/i }, toString: ({}).constructor.prototype.toString, isDefined: function (p) { return typeof p != "undefined" }, isArray: function (p) { return j.rgx.arr.test(j.toString.call(p)) }, isString: function (p) { return j.rgx.str.test(j.toString.call(p)) }, isNum: function (p) { return j.rgx.num.test(j.toString.call(p)) }, isStrNum: function (p) { return j.isString(p) && (/\d/).test(p) }, isFunc: function (p) { return j.rgx.fun.test(j.toString.call(p)) }, getNumRegx: /[\d][\d\.\_,\-]*/, splitNumRegx: /[\.\_,\-]/g, getNum: function (q, r) { var p = j.isStrNum(q) ? (r && j.isString(r) ? new RegExp(r) : j.getNumRegx).exec(q) : null; return p ? p[0] : null }, compareNums: function (w, u, t) { var s, r, q, v = parseInt; if (j.isStrNum(w) && j.isStrNum(u)) { if (j.isDefined(t) && t.compareNums) { return t.compareNums(w, u) } s = w.split(j.splitNumRegx); r = u.split(j.splitNumRegx); for (q = 0; q < Math.min(s.length, r.length) ; q++) { if (v(s[q], 10) > v(r[q], 10)) { return 1 } if (v(s[q], 10) < v(r[q], 10)) { return -1 } } } return 0 }, formatNum: function (q, r) { var p, s; if (!j.isStrNum(q)) { return null } if (!j.isNum(r)) { r = 4 } r--; s = q.replace(/\s/g, "").split(j.splitNumRegx).concat(["0", "0", "0", "0"]); for (p = 0; p < 4; p++) { if (/^(0+)(.+)$/.test(s[p])) { s[p] = RegExp.$2 } if (p > r || !(/\d/).test(s[p])) { s[p] = "0" } } return s.slice(0, 4).join(",") }, pd: { getPROP: function (s, q, p) { try { if (s) { p = s[q] } } catch (r) { this.errObj = r; } return p }, findNavPlugin: function (u) { if (u.dbug) { return u.dbug } var A = null; if (window.navigator) { var z = { Find: j.isString(u.find) ? new RegExp(u.find, "i") : u.find, Find2: j.isString(u.find2) ? new RegExp(u.find2, "i") : u.find2, Avoid: u.avoid ? (j.isString(u.avoid) ? new RegExp(u.avoid, "i") : u.avoid) : 0, Num: u.num ? /\d/ : 0 }, s, r, t, y, x, q, p = navigator.mimeTypes, w = navigator.plugins; if (u.mimes && p) { y = j.isArray(u.mimes) ? [].concat(u.mimes) : (j.isString(u.mimes) ? [u.mimes] : []); for (s = 0; s < y.length; s++) { r = 0; try { if (j.isString(y[s]) && /[^\s]/.test(y[s])) { r = p[y[s]].enabledPlugin } } catch (v) { } if (r) { t = this.findNavPlugin_(r, z); if (t.obj) { A = t.obj } if (A && !j.dbug) { return A } } } } if (u.plugins && w) { x = j.isArray(u.plugins) ? [].concat(u.plugins) : (j.isString(u.plugins) ? [u.plugins] : []); for (s = 0; s < x.length; s++) { r = 0; try { if (x[s] && j.isString(x[s])) { r = w[x[s]] } } catch (v) { } if (r) { t = this.findNavPlugin_(r, z); if (t.obj) { A = t.obj } if (A && !j.dbug) { return A } } } q = w.length; if (j.isNum(q)) { for (s = 0; s < q; s++) { r = 0; try { r = w[s] } catch (v) { } if (r) { t = this.findNavPlugin_(r, z); if (t.obj) { A = t.obj } if (A && !j.dbug) { return A } } } } } } return A }, findNavPlugin_: function (t, s) { var r = t.description || "", q = t.name || "", p = {}; if ((s.Find.test(r) && (!s.Find2 || s.Find2.test(q)) && (!s.Num || s.Num.test(RegExp.leftContext + RegExp.rightContext))) || (s.Find.test(q) && (!s.Find2 || s.Find2.test(r)) && (!s.Num || s.Num.test(RegExp.leftContext + RegExp.rightContext)))) { if (!s.Avoid || !(s.Avoid.test(r) || s.Avoid.test(q))) { p.obj = t } } return p }, getVersionDelimiter: ",", findPlugin: function (r) { var q, p = { status: -3, plugin: 0 }; if (!j.isString(r)) { return p } if (r.length == 1) { this.getVersionDelimiter = r; return p } r = r.toLowerCase().replace(/\s/g, ""); q = j.Plugins[r]; if (!q || !q.getVersion) { return p } p.plugin = q; p.status = 1; return p } }, getPluginFileVersion: function (s, u, w, r) { var p, q, v, y, t = -1; if (!s) { return u } r = r || "version"; if (s[r]) { p = j.getNum(s[r] + "", w) } if (!p || !u) { return u || p || null } q = (j.formatNum(u)).split(j.splitNumRegx); v = (j.formatNum(p)).split(j.splitNumRegx); for (y = 0; y < q.length; y++) { if (t > -1 && y > t && q[y] != "0") { return u } if (v[y] != q[y]) { if (t == -1) { t = y } if (q[y] != "0") { return u } } } return p }, AXO: (function () { var q; try { q = new window.ActiveXObject() } catch (p) { } return q ? null : window.ActiveXObject })(), getAXO: function (p) { var r = null; try { r = new j.AXO(p) } catch (q) { j.errObj = q; } if (r) { j.browser.ActiveXEnabled = !0 } return r }, browser: { detectPlatform: function () { var r = this, q, p = window.navigator ? navigator.platform || "" : ""; j.OS = 100; if (p) { var s = ["Win", 1, "Mac", 2, "Linux", 3, "FreeBSD", 4, "iPhone", 21.1, "iPod", 21.2, "iPad", 21.3, "Win.*CE", 22.1, "Win.*Mobile", 22.2, "Pocket\\s*PC", 22.3, "", 100]; for (q = s.length - 2; q >= 0; q = q - 2) { if (s[q] && new RegExp(s[q], "i").test(p)) { j.OS = s[q + 1]; break } } } }, detectIE: function () { var r = this, u = document, t, q, v = window.navigator ? navigator.userAgent || "" : "", w, p, y; r.ActiveXFilteringEnabled = !1; r.ActiveXEnabled = !1; try { r.ActiveXFilteringEnabled = !!window.external.msActiveXFilteringEnabled() } catch (s) { } p = ["Msxml2.XMLHTTP", "Msxml2.DOMDocument", "Microsoft.XMLDOM", "TDCCtl.TDCCtl", "Shell.UIHelper", "HtmlDlgSafeHelper.HtmlDlgSafeHelper", "Scripting.Dictionary"]; y = ["WMPlayer.OCX", "ShockwaveFlash.ShockwaveFlash", "AgControl.AgControl"]; w = p.concat(y); for (t = 0; t < w.length; t++) { if (j.getAXO(w[t]) && !j.dbug) { break } } if (r.ActiveXEnabled && r.ActiveXFilteringEnabled) { for (t = 0; t < y.length; t++) { if (j.getAXO(y[t])) { r.ActiveXFilteringEnabled = !1; break } } } q = u.documentMode; try { u.documentMode = "" } catch (s) { } r.isIE = r.ActiveXEnabled; r.isIE = r.isIE || j.isNum(u.documentMode) || new Function("return/*@cc_on!@*/!1")(); try { u.documentMode = q } catch (s) { } r.verIE = null; if (r.isIE) { r.verIE = (j.isNum(u.documentMode) && u.documentMode >= 7 ? u.documentMode : 0) || ((/^(?:.*?[^a-zA-Z])??(?:MSIE|rv\s*\:)\s*(\d+\.?\d*)/i).test(v) ? parseFloat(RegExp.$1, 10) : 7) } }, detectNonIE: function () { var q = this, p = 0, t = window.navigator ? navigator : {}, s = q.isIE ? "" : t.userAgent || "", u = t.vendor || "", r = t.product || ""; q.isGecko = !p && (/Gecko/i).test(r) && (/Gecko\s*\/\s*\d/i).test(s); p = p || q.isGecko; q.verGecko = q.isGecko ? j.formatNum((/rv\s*\:\s*([\.\,\d]+)/i).test(s) ? RegExp.$1 : "0.9") : null; q.isOpera = !p && (/(OPR\s*\/|Opera\s*\/\s*\d.*\s*Version\s*\/|Opera\s*[\/]?)\s*(\d+[\.,\d]*)/i).test(s); p = p || q.isOpera; q.verOpera = q.isOpera ? j.formatNum(RegExp.$2) : null; q.isEdge = !p && (/(Edge)\s*\/\s*(\d[\d\.]*)/i).test(s); p = p || q.isEdge; q.verEdgeHTML = q.isEdge ? j.formatNum(RegExp.$2) : null; q.isChrome = !p && (/(Chrome|CriOS)\s*\/\s*(\d[\d\.]*)/i).test(s); p = p || q.isChrome; q.verChrome = q.isChrome ? j.formatNum(RegExp.$2) : null; q.isSafari = !p && ((/Apple/i).test(u) || !u) && (/Safari\s*\/\s*(\d[\d\.]*)/i).test(s); p = p || q.isSafari; q.verSafari = q.isSafari && (/Version\s*\/\s*(\d[\d\.]*)/i).test(s) ? j.formatNum(RegExp.$1) : null; }, init: function () { var p = this; p.detectPlatform(); p.detectIE(); p.detectNonIE() } }, init: { hasRun: 0, library: function () { window[j.name] = j; var q = this, p = document; j.win.init(); j.head = p.getElementsByTagName("head")[0] || p.getElementsByTagName("body")[0] || p.body || null; j.browser.init(); q.hasRun = 1; } }, ev: { addEvent: function (r, q, p) { if (r && q && p) { if (r.addEventListener) { r.addEventListener(q, p, false) } else { if (r.attachEvent) { r.attachEvent("on" + q, p) } else { r["on" + q] = this.concatFn(p, r["on" + q]) } } } }, removeEvent: function (r, q, p) { if (r && q && p) { if (r.removeEventListener) { r.removeEventListener(q, p, false) } else { if (r.detachEvent) { r.detachEvent("on" + q, p) } } } }, concatFn: function (q, p) { return function () { q(); if (typeof p == "function") { p() } } }, handler: function (t, s, r, q, p) { return function () { t(s, r, q, p) } }, handlerOnce: function (s, r, q, p) { return function () { var u = j.uniqueName(); if (!s[u]) { s[u] = 1; s(r, q, p) } } }, handlerWait: function (s, u, r, q, p) { var t = this; return function () { t.setTimeout(t.handler(u, r, q, p), s) } }, setTimeout: function (q, p) { if (j.win && j.win.unload) { return } setTimeout(q, p) }, fPush: function (q, p) { if (j.isArray(p) && (j.isFunc(q) || (j.isArray(q) && q.length > 0 && j.isFunc(q[0])))) { p.push(q) } }, call0: function (q) { var p = j.isArray(q) ? q.length : -1; if (p > 0 && j.isFunc(q[0])) { q[0](j, p > 1 ? q[1] : 0, p > 2 ? q[2] : 0, p > 3 ? q[3] : 0) } else { if (j.isFunc(q)) { q(j) } } }, callArray0: function (p) { var q = this, r; if (j.isArray(p)) { while (p.length) { r = p[0]; p.splice(0, 1); if (j.win && j.win.unload && p !== j.win.unloadHndlrs) { } else { q.call0(r) } } } }, call: function (q) { var p = this; p.call0(q); p.ifDetectDoneCallHndlrs() }, callArray: function (p) { var q = this; q.callArray0(p); q.ifDetectDoneCallHndlrs() }, allDoneHndlrs: [], ifDetectDoneCallHndlrs: function () { var r = this, p, q; if (!r.allDoneHndlrs.length) { return } if (j.win) { if (!j.win.loaded || j.win.loadPrvtHndlrs.length || j.win.loadPblcHndlrs.length) { return } } if (j.Plugins) { for (p in j.Plugins) { if (j.hasOwn(j.Plugins, p)) { q = j.Plugins[p]; if (q && j.isFunc(q.getVersion)) { if (q.OTF == 3 || (q.DoneHndlrs && q.DoneHndlrs.length) || (q.BIHndlrs && q.BIHndlrs.length)) { return } } } } } r.callArray0(r.allDoneHndlrs); } }, isMinVersion: function (v, u, r, q) { var s = j.pd.findPlugin(v), t, p = -1; if (s.status < 0) { return s.status } t = s.plugin; u = j.formatNum(j.isNum(u) ? u.toString() : (j.isStrNum(u) ? j.getNum(u) : "0")); if (t.getVersionDone != 1) { t.getVersion(u, r, q); if (t.getVersionDone === null) { t.getVersionDone = 1 } } if (t.installed !== null) { p = t.installed <= 0.5 ? t.installed : (t.installed == 0.7 ? 1 : (t.version === null ? 0 : (j.compareNums(t.version, u, t) >= 0 ? 1 : -0.1))) } return p }, getVersion: function (u, r, q) { var s = j.pd.findPlugin(u), t, p; if (s.status < 0) { return null } t = s.plugin; if (t.getVersionDone != 1) { t.getVersion(null, r, q); if (t.getVersionDone === null) { t.getVersionDone = 1 } } p = (t.version || t.version0); p = p ? p.replace(j.splitNumRegx, j.pd.getVersionDelimiter) : p; return p }, hasMimeType: function (t) { if (t && window.navigator && navigator.mimeTypes) { var w, v, q, s, p = navigator.mimeTypes, r = j.isArray(t) ? [].concat(t) : (j.isString(t) ? [t] : []); s = r.length; for (q = 0; q < s; q++) { w = 0; try { if (j.isString(r[q]) && /[^\s]/.test(r[q])) { w = p[r[q]] } } catch (u) { } v = w ? w.enabledPlugin : 0; if (v && (v.name || v.description)) { return w } } } return null }, codebase: { isDisabled: function () { if (j.browser.ActiveXEnabled && j.isDefined(j.pd.getPROP(document.createElement("object"), "object"))) { return 0 } return 1 }, isMin: function (v, u, s) { var t = this, r, q, p = 0; if (!j.isStrNum(u) || t.isDisabled()) { return p } t.init(v); if (!s || t.isActiveXObject(v, j.formatNum(v.DIGITMIN.join(",")))) { if (!v.L) { v.L = {}; for (r = 0; r < v.Lower.length; r++) { if (t.isActiveXObject(v, v.Lower[r])) { v.L = t.convert(v, v.Lower[r]); break } } } if (v.L.v) { q = t.convert(v, u, 1); if (q.x >= 0) { p = (v.L.x == q.x ? t.isActiveXObject(v, q.v) : j.compareNums(u, v.L.v) <= 0) ? 1 : -1 } } } return p }, search: function (v, D) { var B = this, w = v.$$, q = 0, r; r = v.searchHasRun || B.isDisabled() ? 1 : 0; v.searchHasRun = 1; if (r) { return v.version || null } B.init(v); if (!D || B.isActiveXObject(v, j.formatNum(v.DIGITMIN.join(",")))) { var G, F, E, s = v.DIGITMAX, t, p, C = 99999999, u = [0, 0, 0, 0], H = [0, 0, 0, 0]; var A = function (y, K) { var I = [].concat(u), J; I[y] = K; J = B.isActiveXObject(v, I.join(",")); if (J) { q = 1; u[y] = K } else { H[y] = K } return J }; for (G = 0; G < H.length; G++) { u[G] = Math.floor(v.DIGITMIN[G]) || 0; t = u.join(","); p = u.slice(0, G).concat([C, C, C, C]).slice(0, u.length).join(","); for (E = 0; E < s.length; E++) { if (j.isArray(s[E])) { s[E].push(0); if (s[E][G] > H[G] && j.compareNums(p, v.Lower[E]) >= 0 && j.compareNums(t, v.Upper[E]) < 0) { H[G] = Math.floor(s[E][G]) } } } for (F = 0; F < 30; F++) { if (H[G] - u[G] <= 16) { for (E = H[G]; E >= u[G] + (G ? 1 : 0) ; E--) { if (A(G, E)) { break } } break } A(G, Math.round((H[G] + u[G]) / 2)) } if (!q) { break } H[G] = u[G]; } if (q) { v.version = B.convert(v, u.join(",")).v } } return v.version || null }, emptyNode: function (p) { try { p.innerHTML = "" } catch (q) { } }, HTML: [], len: 0, onUnload: function (r, q) { var p, t = q.HTML, s; for (p = 0; p < t.length; p++) { s = t[p]; if (s) { t[p] = 0; q.emptyNode(s.span()); s.span = 0; s.spanObj = 0; s = 0 } } q.iframe = 0 }, init: function (u) { var t = this; if (!t.iframe) { var s = j.DOM, q; q = s.iframe.insert(0, "$.codebase{ }"); t.iframe = q; s.iframe.write(q, " "); s.iframe.close(q); } if (!u.init) { u.init = 1; var p, r; j.ev.fPush([t.onUnload, t], j.win.unloadHndlrs); u.tagA = '<object width="1" height="1" style="display:none;" codebase="#version='; r = u.classID || u.$$.classID || ""; u.tagB = '" ' + ((/clsid\s*:/i).test(r) ? 'classid="' : 'type="') + r + '">' + (u.ParamTags ? u.ParamTags : "") + j.openTag + "/object>"; for (p = 0; p < u.Lower.length; p++) { u.Lower[p] = j.formatNum(u.Lower[p]); u.Upper[p] = j.formatNum(u.Upper[p]); } } }, isActiveXObject: function (u, q) { var t = this, p = 0, s = u.$$, r = (j.DOM.iframe.doc(t.iframe) || document).createElement("span"); if (u.min && j.compareNums(q, u.min) <= 0) { return 1 } if (u.max && j.compareNums(q, u.max) >= 0) { return 0 } r.innerHTML = u.tagA + q + u.tagB; if (j.pd.getPROP(r.firstChild, "object")) { p = 1 } if (p) { u.min = q; t.HTML.push({ spanObj: r, span: t.span }) } else { u.max = q; r.innerHTML = "" } return p }, span: function () { return this.spanObj }, convert_: function (t, p, q, s) { var r = t.convert[p]; return r ? (j.isFunc(r) ? j.formatNum(r(q.split(j.splitNumRegx), s).join(",")) : q) : r }, convert: function (v, r, u) { var t = this, q, p, s; r = j.formatNum(r); p = { v: r, x: -1 }; if (r) { for (q = 0; q < v.Lower.length; q++) { s = t.convert_(v, q, v.Lower[q]); if (s && j.compareNums(r, u ? s : v.Lower[q]) >= 0 && (!q || j.compareNums(r, u ? t.convert_(v, q, v.Upper[q]) : v.Upper[q]) < 0)) { p.v = t.convert_(v, q, r, u); p.x = q; break } } } return p }, z: 0 }, win: { disable: function () { this.cancel = true }, cancel: false, loaded: false, unload: false, hasRun: 0, init: function () { var p = this; if (!p.hasRun) { p.hasRun = 1; if ((/complete/i).test(document.readyState || "")) { p.loaded = true; } else { j.ev.addEvent(window, "load", p.onLoad) } j.ev.addEvent(window, "unload", p.onUnload) } }, loadPrvtHndlrs: [], loadPblcHndlrs: [], unloadHndlrs: [], onUnload: function () { var p = j.win; if (p.unload) { return } p.unload = true; j.ev.removeEvent(window, "load", p.onLoad); j.ev.removeEvent(window, "unload", p.onUnload); j.ev.callArray(p.unloadHndlrs) }, onLoad: function () { var p = j.win; if (p.loaded || p.unload || p.cancel) { return } p.loaded = true; j.ev.callArray(p.loadPrvtHndlrs); j.ev.callArray(p.loadPblcHndlrs); } }, DOM: { isEnabled: { objectTag: function () { var q = j.browser, p = q.isIE ? 0 : 1; if (q.ActiveXEnabled) { p = 1 } return !!p }, objectTagUsingActiveX: function () { var p = 0; if (j.browser.ActiveXEnabled) { p = 1 } return !!p }, objectProperty: function (p) { if (p && p.tagName && j.browser.isIE) { if ((/applet/i).test(p.tagName)) { return (!this.objectTag() || j.isDefined(j.pd.getPROP(document.createElement("object"), "object")) ? 1 : 0) } return j.isDefined(j.pd.getPROP(document.createElement(p.tagName), "object")) ? 1 : 0 } return 0 } }, HTML: [], div: null, divID: "plugindetect", divWidth: 500, getDiv: function () { return this.div || document.getElementById(this.divID) || null }, initDiv: function () { var q = this, p; if (!q.div) { p = q.getDiv(); if (p) { q.div = p; } else { q.div = document.createElement("div"); q.div.id = q.divID; q.setStyle(q.div, q.getStyle.div()); q.insertDivInBody(q.div) } j.ev.fPush([q.onUnload, q], j.win.unloadHndlrs) } p = 0 }, pluginSize: 1, iframeWidth: 40, iframeHeight: 10, altHTML: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", emptyNode: function (q) { var p = this; if (q && (/div|span/i).test(q.tagName || "")) { if (j.browser.isIE) { p.setStyle(q, ["display", "none"]) } try { q.innerHTML = "" } catch (r) { } } }, removeNode: function (p) { try { if (p && p.parentNode) { p.parentNode.removeChild(p) } } catch (q) { } }, onUnload: function (u, t) { var r, q, s, v, w = t.HTML, p = w.length; if (p) { for (q = p - 1; q >= 0; q--) { v = w[q]; if (v) { w[q] = 0; t.emptyNode(v.span()); t.removeNode(v.span()); v.span = 0; v.spanObj = 0; v.doc = 0; v.objectProperty = 0 } } } r = t.getDiv(); t.emptyNode(r); t.removeNode(r); v = 0; s = 0; r = 0; t.div = 0 }, span: function () { var p = this; if (!p.spanObj) { p.spanObj = p.doc.getElementById(p.spanId) } return p.spanObj || null }, width: function () { var t = this, s = t.span(), q, r, p = -1; q = s && j.isNum(s.scrollWidth) ? s.scrollWidth : p; r = s && j.isNum(s.offsetWidth) ? s.offsetWidth : p; s = 0; return r > 0 ? r : (q > 0 ? q : Math.max(r, q)) }, obj: function () { var p = this.span(); return p ? p.firstChild || null : null }, readyState: function () { var p = this; return j.browser.isIE && j.isDefined(j.pd.getPROP(p.span(), "readyState")) ? j.pd.getPROP(p.obj(), "readyState") : j.UNDEFINED }, objectProperty: function () { var r = this, q = r.DOM, p; if (q.isEnabled.objectProperty(r)) { p = j.pd.getPROP(r.obj(), "object") } return p }, onLoadHdlr: function (p, q) { q.loaded = 1 }, getTagStatus: function (q, A, E, D, t, H, v) { var F = this; if (!q || !q.span()) { return -2 } var y = q.width(), r = q.obj() ? 1 : 0, s = q.readyState(), p = q.objectProperty(); if (p) { return 1.5 } var u = /clsid\s*\:/i, C = E && u.test(E.outerHTML || "") ? E : (D && u.test(D.outerHTML || "") ? D : 0), w = E && !u.test(E.outerHTML || "") ? E : (D && !u.test(D.outerHTML || "") ? D : 0), z = q && u.test(q.outerHTML || "") ? C : w; if (!A || !A.span() || !z || !z.span()) { return -2 } var x = z.width(), B = A.width(), G = z.readyState(); if (y < 0 || x < 0 || B <= F.pluginSize) { return 0 } if (v && !q.pi && j.isDefined(p) && j.browser.isIE && q.tagName == z.tagName && q.time <= z.time && y === x && s === 0 && G !== 0) { q.pi = 1 } if (x < B || !q.loaded || !A.loaded || !z.loaded) { return q.pi ? -0.1 : 0 } if (y == B || !r) { return q.pi ? -0.5 : -1 } else { if (y == F.pluginSize && r && (!j.isNum(s) || s === 4)) { return 1 } } return q.pi ? -0.5 : -1 }, setStyle: function (q, t) { var s = q.style, p; if (s && t) { for (p = 0; p < t.length; p = p + 2) { try { s[t[p]] = t[p + 1] } catch (r) { } } } q = 0; s = 0 }, getStyle: { iframe: function () { return this.span() }, span: function (r) { var q = j.DOM, p; p = r ? this.plugin() : ([].concat(this.Default).concat(["display", "inline", "fontSize", (q.pluginSize + 3) + "px", "lineHeight", (q.pluginSize + 3) + "px"])); return p }, div: function () { var p = j.DOM; return [].concat(this.Default).concat(["display", "block", "width", p.divWidth + "px", "height", (p.pluginSize + 3) + "px", "fontSize", (p.pluginSize + 3) + "px", "lineHeight", (p.pluginSize + 3) + "px", "position", "absolute", "right", "9999px", "top", "-9999px"]) }, plugin: function (q) { var p = j.DOM; return "background-color:transparent;background-image:none;vertical-align:baseline;outline-style:none;border-style:none;padding:0px;margin:0px;visibility:" + (q ? "hidden;" : "visible;") + "display:inline;font-size:" + (p.pluginSize + 3) + "px;line-height:" + (p.pluginSize + 3) + "px;" }, Default: ["backgroundColor", "transparent", "backgroundImage", "none", "verticalAlign", "baseline", "outlineStyle", "none", "borderStyle", "none", "padding", "0px", "margin", "0px", "visibility", "visible"] }, insertDivInBody: function (v, t) { var u = "pd33993399", q = null, s = t ? window.top.document : window.document, p = s.getElementsByTagName("body")[0] || s.body; if (!p) { try { s.write('<div id="' + u + '">.' + j.openTag + "/div>"); q = s.getElementById(u) } catch (r) { } } p = s.getElementsByTagName("body")[0] || s.body; if (p) { p.insertBefore(v, p.firstChild); if (q) { p.removeChild(q) } } v = 0 }, iframe: { onLoad: function (p, q) { j.ev.callArray(p); }, insert: function (s, v) { var q = this, w = j.DOM, p, r = document.createElement("iframe"), x, t; w.setStyle(r, w.getStyle.iframe()); r.width = w.iframeWidth; r.height = w.iframeHeight; w.initDiv(); p = w.getDiv(); p.appendChild(r); try { q.doc(r).open() } catch (u) { } r[j.uniqueName()] = []; x = j.ev.handlerOnce(j.isNum(s) && s > 0 ? j.ev.handlerWait(s, q.onLoad, r[j.uniqueName()], v) : j.ev.handler(q.onLoad, r[j.uniqueName()], v)); j.ev.addEvent(r, "load", x); if (!r.onload) { r.onload = x } t = q.win(r); j.ev.addEvent(t, "load", x); if (t && !t.onload) { t.onload = x } return r }, addHandler: function (q, p) { if (q) { j.ev.fPush(p, q[j.uniqueName()]) } }, close: function (p) { try { this.doc(p).close() } catch (q) { } }, write: function (q, u) { var t = this.doc(q), p = -1, s; try { s = new Date().getTime(); t.write(u); p = new Date().getTime() - s } catch (r) { } return p }, win: function (p) { try { return p.contentWindow } catch (q) { } return null }, doc: function (p) { var r; try { r = p.contentWindow.document } catch (q) { } try { if (!r) { r = p.contentDocument } } catch (q) { } return r || null } }, insert: function (t, s, u, p, z, y, v) { var E = this, G, F, D, C, B, w; if (!v) { E.initDiv(); v = E.getDiv() } if (v) { if ((/div/i).test(v.tagName)) { C = v.ownerDocument } if ((/iframe/i).test(v.tagName)) { C = E.iframe.doc(v) } } if (C && C.createElement) { } else { C = document } if (!j.isDefined(p)) { p = "" } if (j.isString(t) && (/[^\s]/).test(t)) { t = t.toLowerCase().replace(/\s/g, ""); G = j.openTag + t + " "; G += 'style="' + E.getStyle.plugin(y) + '" '; var r = 1, q = 1; for (B = 0; B < s.length; B = B + 2) { if (/[^\s]/.test(s[B + 1])) { G += s[B] + '="' + s[B + 1] + '" ' } if ((/width/i).test(s[B])) { r = 0 } if ((/height/i).test(s[B])) { q = 0 } } G += (r ? 'width="' + E.pluginSize + '" ' : "") + (q ? 'height="' + E.pluginSize + '" ' : ""); if (t == "embed" || t == "img") { G += " />" } else { G += ">"; for (B = 0; B < u.length; B = B + 2) { if (/[^\s]/.test(u[B + 1])) { G += j.openTag + 'param name="' + u[B] + '" value="' + u[B + 1] + '" />' } } G += p + j.openTag + "/" + t + ">" } } else { t = ""; G = p } F = { spanId: "", spanObj: null, span: E.span, loaded: null, tagName: t, outerHTML: G, DOM: E, time: new Date().getTime(), insertDomDelay: -1, width: E.width, obj: E.obj, readyState: E.readyState, objectProperty: E.objectProperty, doc: C }; if (v && v.parentNode) { if ((/iframe/i).test(v.tagName)) { E.iframe.addHandler(v, [E.onLoadHdlr, F]); F.loaded = 0; F.spanId = j.name + "Span" + E.HTML.length; D = '<span id="' + F.spanId + '" style="' + E.getStyle.span(1) + '">' + G + "</span>"; F.time = new Date().getTime(); w = E.iframe.write(v, D); if (w >= 0) { F.insertDomDelay = w } } else { if ((/div/i).test(v.tagName)) { D = C.createElement("span"); E.setStyle(D, E.getStyle.span()); v.appendChild(D); try { F.time = new Date().getTime(); D.innerHTML = G; F.insertDomDelay = new Date().getTime() - F.time } catch (A) { } F.spanObj = D } } } D = 0; v = 0; E.HTML.push(F); return F } }, Plugins: {} }; j.init.library(); var f = { compareNums: function (s, r) { var A = s.split(j.splitNumRegx), y = r.split(j.splitNumRegx), w, q, p, v, u, z; for (w = 0; w < Math.min(A.length, y.length) ; w++) { z = /([\d]+)([a-z]?)/.test(A[w]); q = parseInt(RegExp.$1, 10); v = (w == 2 && RegExp.$2.length > 0) ? RegExp.$2.charCodeAt(0) : -1; z = /([\d]+)([a-z]?)/.test(y[w]); p = parseInt(RegExp.$1, 10); u = (w == 2 && RegExp.$2.length > 0) ? RegExp.$2.charCodeAt(0) : -1; if (q != p) { return (q > p ? 1 : -1) } if (w == 2 && v != u) { return (v > u ? 1 : -1) } } return 0 }, setPluginStatus: function (r, p, s) { var q = this; q.installed = p ? 1 : (s ? (s > 0 ? 0.7 : -0.1) : (r ? 0 : -1)); if (p) { q.version = j.formatNum(p) } q.getVersionDone = q.installed == 1 || q.installed == -1 || q.instance.hasRun ? 1 : 0; }, getVersion: function (t, q) { var u = this, s, p = null, r; if ((!s || j.dbug) && u.nav.query().installed) { s = 1 } if ((!p || j.dbug) && u.nav.query().version) { p = u.nav.version } if ((!s || j.dbug) && u.axo.query().installed) { s = 1 } if ((!p || j.dbug) && u.axo.query().version) { p = u.axo.version } if (!p || j.dbug) { r = u.codebase.isMin(t); if (r) { u.setPluginStatus(0, 0, r); return } } if (!p || j.dbug) { r = u.codebase.search(); if (r) { s = 1; p = r } } if ((!p && q) || j.dbug) { r = u.instance.query().version; if (r) { s = 1; p = r } } u.setPluginStatus(s, p, 0) }, nav: { hasRun: 0, installed: 0, version: null, mimeType: ["application/x-vlc-plugin", "application/x-google-vlc-plugin", "application/mpeg4-muxcodetable", "application/x-matroska", "application/xspf+xml", "video/divx", "video/webm", "video/x-mpeg", "video/x-msvideo", "video/ogg", "audio/x-flac", "audio/amr", "audio/amr"], find: "VLC.*Plug-?in", find2: "VLC|VideoLAN", avoid: "Totem|Helix", plugins: ["VLC Web Plugin", "VLC Multimedia Plug-in", "VLC Multimedia Plugin", "VLC multimedia plugin"], query: function () { var s = this, p, r, q = s.hasRun || !j.hasMimeType(s.mimeType); s.hasRun = 1; if (q) { return s } r = j.pd.findNavPlugin({ find: s.find, avoid: s.avoid, mimes: s.mimeType, plugins: s.plugins }); if (r) { s.installed = 1; if (r.description) { p = j.getNum(r.description + "", "[\\d][\\d\\.]*[a-z]*") } p = j.getPluginFileVersion(r, p); if (p) { s.version = p } } return s } }, instance: { hasRun: 0, installed: 0, version: null, mimeType: "application/x-vlc-plugin", HTML: 0, isDisabled: function () { var q = this, p = 1; if (q.hasRun) { } else { if (j.dbug) { p = 0 } else { if (f.nav.installed && j.hasMimeType(q.mimeType)) { p = 0 } } } return p }, query: function () { var p = this, s = j.DOM.altHTML, r = null, u = 0, q = p.isDisabled(); p.hasRun = 1; if (q) { return p } p.HTML = j.DOM.insert("object", ["type", p.mimeType], ["autoplay", "no", "loop", "no", "volume", "0"], s, f); u = p.HTML.obj(); if (u) { try { if (!r) { r = j.getNum(u.VersionInfo) } } catch (t) { } try { if (!r) { r = j.getNum(u.versionInfo()) } } catch (t) { } } if (r) { p.version = r; p.installed = 1 } return p } }, axo: { hasRun: 0, installed: 0, version: null, progID: "VideoLAN.VLCPlugin", query: function () { var q = this, s, p, r = q.hasRun; q.hasRun = 1; if (r) { return q } s = j.getAXO(q.progID); if (s) { q.installed = 1; p = j.getNum(j.pd.getPROP(s, "VersionInfo"), "[\\d][\\d\\.]*[a-z]*"); if (p) { q.version = p } } return q } }, codebase: { classID: "clsid:9BE31822-FDAD-461B-AD51-BE1D1C159921", isMin: function (p) { this.$$ = f; return j.codebase.isMin(this, p) }, search: function () { this.$$ = f; return j.codebase.search(this) }, DIGITMAX: [[11, 11, 16]], DIGITMIN: [0, 0, 0, 0], Upper: ["999"], Lower: ["0"], convert: [1] } }; j.addPlugin("vlc", f); })();