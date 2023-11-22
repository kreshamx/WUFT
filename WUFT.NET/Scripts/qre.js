function sendEmail(id) {
    showLoading();
    $.ajax({
        type: 'POST',
        url: '/Email/SendReminder/' + id,
        success: function () {
            hideLoading();
            alertify.success("Email reminder sent.");
        },
        error: function () {
            hideLoading();
            alertify.error("Email could not be sent.");
        }
    })
}

function trim(value) {
    return value.replace(/^\s+|\s+$/g, "");
}

function cancelUpload() {
    showLoading();
    window.history.go(-1);
}

function cancelConfirmation() {
    showLoading();
    window.history.go(-2);
}

function okError() {
    showLoading();
    window.history.go(-3);
}

function combineRequests(qrejob, mrb) {
    showLoading();
    $.ajax({
        type: 'POST',
        url: '/QRE/ConfirmMerge',
        async: false,
        dataType: 'json',
        data: {
            qrejobid: qrejob,
            mrbid: mrb
        },
        success: function (data) {
            hideLoading();
            window.location.href = data;
        },
        error: function () {
            hideLoading();
        }
    })
}

function removeHighlightRow() {
    $("#qreIndexTable tr").removeClass('highlightRow');
}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $("#qreSelectAll").on('change', function () {
        var checkboxes = $("input.selectRequest");
        if ($("#qreSelectAll").is(':checked') == true) {
            checkboxes.prop('checked', true);
        } else {
            checkboxes.prop('checked', false);
        }

    });

    setStringList = function () {
        var ids = $("input.selectRequest:checked").map(function () { return $(this).data('requestid') }).get().join('V<3:)');
        $("#idList").val(ids);
        return true;
    };

    $("#QREIndex").on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });

    $("#createRequestForm input").on('keyup blur change', function () {
        if (this.id === "mrb") {
            this.value = trim(this.value);
        }
        if ($("#createRequestForm").valid()) {
            $("#submitBtn").prop('disabled', false);
        } else {
            $("#submitBtn").prop('disabled', 'disabled');
        }
    });
});

