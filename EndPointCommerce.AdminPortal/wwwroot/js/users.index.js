window.addEventListener('DOMContentLoaded', _event => {
    let url = location.protocol + '//' + location.host + location.pathname;
    let query = `?handler=Search`;

    new DataTable('#table-users', {
        ajax: `${url}${query}`,
        processing: true,
        serverSide: true,
        columns: [
            { name: 'email', data: 'email' },
            { name: 'role', data: 'role' },
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
