// Copyright 2025 End Point Corporation. Apache License, version 2.0.

import { DataTable } from 'simple-datatables';

window.addEventListener('DOMContentLoaded', _event => {
    const datatablesSimple = document.getElementById('table-categories');
    new DataTable(datatablesSimple);
});
