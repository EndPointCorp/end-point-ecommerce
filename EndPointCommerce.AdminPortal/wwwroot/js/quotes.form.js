window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-quote-items');
    if (datatablesSimple != null)
        new simpleDatatables.DataTable(datatablesSimple);
});
