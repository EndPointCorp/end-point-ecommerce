// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿
// Show the information related to the selected user type
$('#User_RoleName').change(function () {
    $('#customerIdContainer').hide();

    if ($('#User_RoleName option:selected').text() == 'Customer')
        $('#customerIdContainer').show();
});

$('#User_RoleName').trigger('change');
