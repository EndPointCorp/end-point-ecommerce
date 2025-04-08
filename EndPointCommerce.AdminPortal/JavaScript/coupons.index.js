import DataTable from 'datatables.net-bs5';
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css';

window.addEventListener('DOMContentLoaded', _event => {
    let url = location.protocol + '//' + location.host + location.pathname;
    let query = `?handler=Search`;

    new DataTable('#table-coupons', {
        ajax: `${url}${query}`,
        processing: true,
        serverSide: true,
        columns: [
            { name: 'code', data: 'code' },
            { name: 'discount', data: 'discount' },
            {
                name: 'isDiscountFixed',
                data: 'isDiscountFixed',
                render: function (data, _type, _row, _meta) {
                    return data ?
                        '<input type="checkbox" disabled="disabled" checked>' :
                        '<input type="checkbox" disabled="disabled">';
                }
            },
            { name: 'dateCreated', data: 'dateCreated' },
            { name: 'dateModified', data: 'dateModified' },
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
