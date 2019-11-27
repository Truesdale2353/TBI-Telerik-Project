function validateForm() {
    let formPassed = true;

    let egn = $("#EGN").val();
    let phoneNumber = $("#phone").val()
    if (egn != undefined && phoneNumber != undefined) {
        let d = checkEgn(egn);
        if (!(/^((359)|(\+359)|(0)){1}[\d]{9,11}$/.test(phoneNumber))) {
            document.getElementById("phoneAlert").innerHTML = "Invalid phone number";
            formPassed = false;
        }
        else {
            document.getElementById("phoneAlert").innerHTML = "";
        }
        if (d === false) {

            document.getElementById("EGNAlert").innerHTML = "Invalid EGN";
            formPassed = false;
        }
        else {
            document.getElementById("EGNAlert").innerHTML = "";
        }
    }

    let name = $("#FullName").val();
    let amount = $("#Amount").val();
    if (name != undefined && amount != undefined) {


        if (!(/^[a-zA-Z\s]*$/.test(name))||name==="") {
            document.getElementById("FullNameAlert").innerHTML = "Invalid Name";
            formPassed = false;
        }
        else {
            document.getElementById("FullNameAlert").innerHTML = "";
        }

        if (!/^(?=.*\d)[\d ]+$/.test(amount)) {

            document.getElementById("amountAlert").innerHTML = "Amount must contain only numbers";
            formPassed = false;
        }
        else {
            document.getElementById("amountAlert").innerHTML = "";
        }
    }
    return formPassed;
}

$(".newStat").change(function () {
    var value = $(this).val();
    if (value === "Open") {
        let informationHolder = $("#data-holder");
        $("#data-holder").empty();
        informationHolder.append(
            "<label for='EGN'>EGN</label>" + "<br>"
            + "<input type = 'text' name = 'EGN' id = 'EGN' > "
            + " <p class='text-danger' id='EGNAlert'></p>"
            + "<label for='phone'>phone number</label>" + "<br>"
            + "<input type = 'text' name = 'PhoneNumber' id = 'phone' > "
            + "<br>"
            + " <p class='text-danger' id='phoneAlert'></p>");
    }
    else if (value === "Accepted") {
        let informationHolder = $("#data-holder");
        $("#data-holder").empty();
        informationHolder.append(
            "<label for='FullName'>Full Name</label>" + "<br>"
            + "<input type = 'text' name = 'FullName' id = 'FullName' > "
            + " <p class='text-danger' id='FullNameAlert'></p>"
            + "<label for='amount'>amount</label>" + "<br>"
            + "<input type = 'text' name = 'Amount' id = 'Amount' > "
            + "<br>"
            + " <p class='text-danger' id='amountAlert'></p>");
    }
    else {
        $("#data-holder").empty();
    }
});


function checkEgn(egn) {
    if (egn.length != 10)
        return false;
    if (/[^0-9]/.test(egn))
        return false;
    var year = Number(egn.slice(0, 2));
    var month = Number(egn.slice(2, 4));
    var day = Number(egn.slice(4, 6));

    if (month >= 40) {
        year += 2000;
        month -= 40;
    } else if (month >= 20) {
        year += 1800;
        month -= 20;
    } else {
        year += 1900;
    }

    if (!isValidDate(year, month, day))
        return false;

    var checkSum = 0;
    var weights = [2, 4, 8, 5, 10, 9, 7, 3, 6];

    for (var ii = 0; ii < weights.length; ++ii) {
        checkSum += weights[ii] * Number(egn.charAt(ii));
    }

    checkSum %= 11;
    checkSum %= 10;

    if (checkSum !== Number(egn.charAt(9)))
        return false;

    return true;

}
function isValidDate(y, m, d) {
    var date = new Date(y, m - 1, d);
    return date && (date.getMonth() + 1) == m && date.getDate() == Number(d);
}
