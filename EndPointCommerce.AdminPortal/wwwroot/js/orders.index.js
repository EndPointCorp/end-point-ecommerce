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
