
// Show the information related to the selected user type
$("#User_RoleName").change(function () {
    $("#customerIdContainer").hide();

    if ($("#User_RoleName option:selected").text() == "Customer")
        $("#customerIdContainer").show();
});

$("form").submit(function (e) {

    if (($("#User_Id").val() == "0") &&
        ($("#User_Password").val() == "")) {
        $("#User_RepeatPasswordValidation").text("The password and repeat password fields are required.");
        e.preventDefault();
    }

    if (($("#User_Password").val() != "") &&
        ($("#User_Password").val() != $("#User_RepeatPassword").val()))
    {
        $("#User_RepeatPasswordValidation").text("The password and its confirmation doesn't match.");
        e.preventDefault();
    }
})

$("#User_RoleName").trigger("change");
