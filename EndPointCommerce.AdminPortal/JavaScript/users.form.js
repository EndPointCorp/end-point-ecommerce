
// Show the information related to the selected user type
$('#User_RoleName').change(function () {
    $('#customerIdContainer').hide();

    if ($('#User_RoleName option:selected').text() == 'Customer')
        $('#customerIdContainer').show();
});

$('#User_RoleName').trigger('change');
