// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿import DataTable from 'datatables.net-bs5';
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css';

window.addEventListener('DOMContentLoaded', _event => {
    let url = location.protocol + '//' + location.host + location.pathname;
    let query = `?handler=Search`;

    new DataTable('#table-orders', {
        ajax: `${url}${query}`,
        processing: true,
        serverSide: true,
        columns: [
            { name: 'id', data: 'id' },
            { name: 'dateCreated', data: 'dateCreated' },
            { name: 'customerFullName', data: 'customerFullName' },
            { name: 'statusName', data: 'statusName' },
            { name: 'billingAddressStateName', data: 'billingAddressStateName' },
            { name: 'total', data: 'total' },
            {
                name: 'action',
                sortable: false,
                render: function (_data, _type, row, _meta) {
                    return `
                        <a class="btn btn-link p-0" role="button" href="${row.editUrl}">
                            <i class="fa-solid fa-pen-to-square"></i> Edit
                        </a>
                    `;
                }
            }
        ],
    });
});
