$(function () {

    $("#btnAnalyze").click(function (evt) {
        evt.preventDefault();


        let model = {
            "Page1": $("#Page1").val(),
            "Page2": $("#Page2").val()
        };

        $('#loading').html('<img src="/images/loading.gif"></br>');

        $.ajax({
            type: "POST",
            url: "/Home/AnalysiseAsync",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (data) {
                if (data == "Error!") { 
                    alert(data);
                } else {
                    $.each(data, function (index, value) {
                        var page2 = "";
                        if (value.page2Word.length > 1) {
                            page2 = ' (' + value.page2Count + ' hits)';
                        }

                        $('#tblResult').append(
                            '<tr>' +
                            '<td><b>' + value.page1Word + '</b> (' + value.page1Count + ' hits)</td>' +
                            '<td><b>' + value.page2Word + '</b> ' + page2 + '</td>' +
                            '</tr>');
                    });
                }
                $('#loading').html("");
            },
            error: function () {
                alert("there was error!");
            }
        });
    });
});