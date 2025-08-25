// Copyright 2025 End Point Corporation. Apache License, version 2.0.

export function initializeAuthNetAcceptJs(dotNet, authNetEnvironment, authNetLoginId, authNetClientKey) {
    loadScript(authNetEnvironment);
    setUpSubmit(authNetLoginId, authNetClientKey, dotNet);
}

function loadScript(authNetEnvironment) {
    const acceptJsUrl = authNetEnvironment == "Production" ?
        "https://js.authorize.net/v1/Accept.js" :
        "https://jstest.authorize.net/v1/Accept.js";

    // console.log("Loading Authorize.Net Accept.js from: ", acceptJsUrl);
    var script = document.createElement('script');
    script.src = acceptJsUrl;
    document.body.appendChild(script);
}

function setUpSubmit(authNetLoginId, authNetClientKey, dotNet) {
    const submitButton = getSubmitButton();
    submitButton.addEventListener("click", () => doSubmit(authNetLoginId, authNetClientKey, dotNet));
}

function doSubmit(authNetLoginId, authNetClientKey, dotNet) {
    disableSubmitButton();
    clearErrors();

    sendPaymentDataToAuthNet(
        authNetLoginId,
        authNetClientKey,
        response => {
            dotNet.invokeMethodAsync(
                "SubmitOrder",
                response.opaqueData.dataValue,
                response.opaqueData.dataDescriptor
            );
        }
    );
}

function sendPaymentDataToAuthNet(authNetLoginId, authNetClientKey, onDone) {
    var authData = {
        clientKey: authNetClientKey,
        apiLoginID: authNetLoginId
    };

    var cardData = {
        cardNumber: document.getElementById("cardNumber").value?.replace(/\s+/g, ''),
        month: document.getElementById("expMonth").value,
        year: document.getElementById("expYear").value,
        cardCode: document.getElementById("cardCode").value
    };

    /* var bankData = {
        accountNumber: document.getElementById('accountNumber').value,
        routingNumber: document.getElementById('routingNumber').value,
        nameOnAccount: document.getElementById('nameOnAccount').value,
        accountType: document.getElementById('accountType').value
    }; */

    var data = {
        authData,
        cardData
        /* bankData: */
    };

    // console.log("Request to Authorize.Net: ", data);
    Accept.dispatchData(data, response => handleAuthNetResponse(response, onDone));
}

function handleAuthNetResponse(response, onDone) {
    // console.log("Response from Authorize.Net: ", response);
    if (response.messages.resultCode === "Error") {
        console.log("Error from Authorize.Net: ", response);

        displayErrors(response.messages.message);
        enableSubmitButton();
    } else {
        onDone(response);
    }
}

function getSubmitButton() {
    return document.getElementById("SubmitOrderButton");
}

function enableSubmitButton() {
    const submitButton = getSubmitButton();
    submitButton.disabled = false;
}

function disableSubmitButton() {
    const submitButton = getSubmitButton();
    submitButton.disabled = true;
}

function clearErrors() {
    const errorList = document.getElementById("AuthNetErrors");
    errorList.innerHTML = "";
}

function displayErrors(errors) {
    const errorList = document.getElementById("AuthNetErrors");

    errors.forEach(error => {
        const li = document.createElement("li");
        li.textContent = getErrorMessage(error.code);
        li.className = "validation-message";
        errorList.appendChild(li);
    });
}

function getErrorMessage(errorCode) {
    const map = {
        "E_WC_04": "Please provide card number, expiration month, year and CVV.",
        "E_WC_05": "Please provide valid card number.",
        "E_WC_06": "Please provide valid expiration month.",
        "E_WC_07": "Please provide valid expiration year.",
        "E_WC_08": "Please provide a future expiration date.",
        "E_WC_15": "Please provide valid CVV.",
        "E_WC_20": "Please provide valid card number."
    };

    return map[errorCode] || "We couldn't process your card at this time. Please try again.";
}
