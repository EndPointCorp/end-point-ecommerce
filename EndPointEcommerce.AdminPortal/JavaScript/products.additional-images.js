window.addEventListener('DOMContentLoaded', _event => {
    let additionaImageInputCount = 0;

    function addNewFileInput(event) {
        convertToSubmittable(event.target)
        createNewFileInput();

        additionaImageInputCount++;
    }

    function convertToSubmittable(fileInput) {
        fileInput.setAttribute('name', 'Product.AdditionalImageFiles');
        fileInput.setAttribute('id', `Product_AdditionalImageFiles_${additionaImageInputCount}`);

        fileInput.removeEventListener('change', addNewFileInput);
    }

    function createNewFileInput() {
        const template = document.querySelector('#product-additional-image-template');
        const clone = template.content.cloneNode(true);

        let fileInput = clone.querySelector('input');
        fileInput.setAttribute('id', 'new-product-additional-image-file');

        appendToContainer(clone);

        fileInput.addEventListener('change', addNewFileInput);

        return fileInput;
    }

    function appendToContainer(element) {
        const container = document.querySelector('#product-additional-images-container');
        container.appendChild(element);
    }

    const newFileInput = document.querySelector('#new-product-additional-image-file');
    newFileInput.addEventListener('change', addNewFileInput);
});
