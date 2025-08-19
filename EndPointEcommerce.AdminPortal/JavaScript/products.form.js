// Copyright 2025 End Point Corporation. Apache License, version 2.0.

import Quill from 'quill';
import 'quill/dist/quill.snow.css';

window.addEventListener('DOMContentLoaded', _event => {
    const quill = new Quill('#product-description-editor', { theme: 'snow' });

    quill.on('text-change', (_delta, _oldDelta, _source) => {
        var productDescriptionField = document.getElementById('Product_Description');
        productDescriptionField.value = quill.getSemanticHTML();
    });
});