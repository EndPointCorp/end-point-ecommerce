import { DataTable } from 'simple-datatables';

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-quote-items');
    if (datatablesSimple != null)
        new DataTable(datatablesSimple);
});
