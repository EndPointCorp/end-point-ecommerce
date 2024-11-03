window.addEventListener('DOMContentLoaded', _event => {
    let url = location.protocol + '//' + location.host + location.pathname;
    let query = `?handler=Search`;

    new DataTable('#table-quotes', {
        ajax: `${url}${query}`,
        processing: true,
        serverSide: true,
        columns: [
            { name: 'id', data: 'id' },
            { name: 'dateCreated', data: 'dateCreated' },
            { name: 'email', data: 'email' },
            { name: 'isOpen', data: 'isOpen' },
            { name: 'shippingAddressStateName', data: 'shippingAddressStateName' },
            { name: 'total', data: 'total', sortable: false },
            {
                name: 'action',
                sortable: false,
                render: function (_data, _type, row, _meta) {
                    return `
                        <a class="btn btn-link p-0" role="button" href="${row.detailsUrl}">
                            <i class="fa-solid fa-eye"></i> Details
                        </a>
                    `;
                }
            }
        ],
    });
});
