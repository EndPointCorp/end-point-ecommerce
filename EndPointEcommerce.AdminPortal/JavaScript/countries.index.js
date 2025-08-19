// Copyright 2025 End Point Corporation. Apache License, version 2.0.

import { DataTable } from 'simple-datatables';

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-countries');
    new DataTable(datatablesSimple);

    document.querySelectorAll('.is-enabled-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', event => {
            postUpdateCountry(
                event.target.getAttribute('data-country-id'),
                event.target.checked
            );
        });
    });
});

function postUpdateCountry(countryId, isEnabled) {
    const url = location.protocol + '//' + location.host + location.pathname + '?handler=UpdateCountry'

    var formData = new FormData();
    formData.append('countryId', countryId);
    formData.append('isEnabled', isEnabled);
    formData.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

    fetch(url, { method: 'POST', body: formData }).catch(_e => {
        alert('Error updating country');
    });
}
