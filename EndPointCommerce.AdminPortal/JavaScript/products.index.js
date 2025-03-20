import { DataTable } from 'simple-datatables';

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-products');
    new DataTable(datatablesSimple);
});
