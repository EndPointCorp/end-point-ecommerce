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
    const submitButton = document.getElementById("SubmitOrderButton");
    submitButton.addEventListener("click", () => doSubmit(authNetLoginId, authNetClientKey, dotNet));
}

function doSubmit(authNetLoginId, authNetClientKey, dotNet) {
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
        cardNumber: document.getElementById("cardNumber").value,
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
        displayErrors(response.messages.message);
    } else {
        onDone(response);
    }
}

function clearErrors() {
    const errorList = document.getElementById("AuthNetErrors");
    errorList.innerHTML = "";
}

function displayErrors(errors) {
    const errorList = document.getElementById("AuthNetErrors");

    errors.forEach(error => {
        const li = document.createElement("li");
        li.textContent = error.text;
        li.className = "validation-message";
        errorList.appendChild(li);
    });
}
