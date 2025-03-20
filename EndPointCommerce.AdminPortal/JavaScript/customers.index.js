import DataTable from 'datatables.net-bs5';
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css';

window.addEventListener('DOMContentLoaded', _event => {
    let url = location.protocol + '//' + location.host + location.pathname;
    let query = `?handler=Search`;

    new DataTable('#table-customers', {
        ajax: `${url}${query}`,
        processing: true,
        serverSide: true,
        columns: [
            { name: 'name', data: 'name' },
            { name: 'lastName', data: 'lastName' },
            { name: 'email', data: 'email' },
            { name: 'dateCreated', data: 'dateCreated' },
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
