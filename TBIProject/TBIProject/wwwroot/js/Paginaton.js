$(window).scroll(function () {
    if ($(window).scrollTop() >= $(document).height() - $(window).height() - 10) {

        let multuplier = $("#multuplier");
        let searchBy = $("#searchBy");

        let data = {
            'filter': searchBy.val(),
            'multyplier': multuplier.val(),
        };


        $.ajax({
            url: "/ListEmails/AppendEmails/",
            method: "GET",
            data: data,
            success: function (response) {
                console.log(response.length);
                for (let i = 0; i < response.length; i++) {
                    let attached = "";
                    if (response[i].attachments !=0) {
                        attached +="<img src='/css/resource/Ellipse1.png'>"
                    }
                   
                    $("#table-body").append(
                        " <tr onclick=\"window.location='/EmailInfo/GetEmailInfo?emailId=" + response[i].emailId + "'\">"
                        + " <th scope='row'>"  
                        + response[i].emailId
                        + attached 
                        + " </th>"
                        + " <td>" + response[i].emailSender + "</td>"
                        + "<td>" + response[i].emailStatus + "</td>"
                        + " <td>" + response[i].emailreceived + "</td>"
                        + " </tr>"
                    );
                        console.log(response[i].emailSender);

                }
                console.dir(response);
                console.dir("udri");
                let nextMultiplier = +multuplier.val() + 1;
                multuplier.val(nextMultiplier);
            },
            error: function (msg) {
                console.dir(msg);
            }
        });
    }

});