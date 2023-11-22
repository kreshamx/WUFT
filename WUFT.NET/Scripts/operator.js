$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $("#operatorSelectAll").on('change', function () {
        var checkboxes = $("input.selectRequest");
        if ($("#operatorSelectAll").is(':checked') == true) {
            checkboxes.each(function(i,o){
                if(o.closest('tr').style.display != 'none')
                    $(o).prop('checked', true);

            });
        } else {
            checkboxes.prop('checked', false);
        }

    });

    setStringList = function(){
        var ids = $("input.selectRequest:checked").map(function(){return $(this).data('requestid')}).get().join('V<3:)');
        $("#idList").val(ids);
        return true;
    };

    $("#OperatorIndex").on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });

    updateRows = function (_selection) {
        $('#warehouseFilter').attr('disabled', 'disabled');
        if (_selection == "Show All") {
            $('#operatorRequestTable').find('.Row').show();
        }
        else {
            $('#operatorRequestTable').find('.Row').hide();
            var criteriaAttribute = '';

            if (_selection != '0') {
                setWarehouseCookie(_selection);
                criteriaAttribute += '[data-warehouse="' + _selection + '"]';
            }

            $('#operatorRequestTable').find('.Row' + criteriaAttribute).show();
        }

        $('#warehouseFilter').removeAttr('disabled');
    }

    $('#warehouseFilter').bind('change', function () {
        updateRows($(this).val());
        setWarehouseCookie($(this).val());
    });

    $("#showall-button").on('click', function () {
        updateRows("Show All");
    });

    OpenBarcodeSheet = function (boxids, id) {
        window.open("/Operator/ViewBarcodes?boxIds=" + boxids.toString() + "&flagRequestId=" + id.toString(), "_blank", "toolbar=no, location=no,status=yes,menubar=no,scrollbars=yes,resizable=no, width=925,height=1000,left=430,top=100");
        return false
    }

    

    updateRows($("#warehouseFilter").val());
    $("#selectAll").show();
});

function setWarehouseCookie(cvalue) {
    cvalue = cvalue == 'undefined' ? "Show All" : cvalue;
    document.cookie = "warehouse=" + cvalue + ";expires=Mon, 1 Jan 2035 12:00:00 UTC; path=/";
}

function getWarehouseCookie() {
    var name = "warehouse=";
    var cookieBouqet = document.cookie.split(';');
    for (var i = 0; i < cookieBouqet.length; i++) {
        var monsterCookie = cookieBouqet[i];
        while (monsterCookie.charAt(0) == ' ') monsterCookie = monsterCookie.substring(1);
        if (monsterCookie.indexOf(name) == 0) return monsterCookie.substring(name.length, monsterCookie.length);
    }
    return "";
}