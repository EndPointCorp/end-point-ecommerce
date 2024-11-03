
// Populate the billing addresses for the selected customer
$("#Order_CustomerId").change(function () {
    $.get(location.protocol + '//' + location.host + location.pathname, {
        handler: "CustomerBillingAddresses",
        customerId: $("#Order_CustomerId").val()
    }, function (data) {
        $("#Order_BillingAddressId").html("");
        data.forEach(function (item, i) {
            $('#Order_BillingAddressId').append($('<option>', {
                value: item.id,
                text: item.fullAddress
            }));
        })
    });
});

$("#Order_CustomerId").trigger("change");

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-order-items');
    if (datatablesSimple != null)
        new simpleDatatables.DataTable(datatablesSimple);
});
