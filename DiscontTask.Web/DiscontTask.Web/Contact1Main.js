var Contact1Main = (function () {
    const onLoad = function (executionContext) {

        const formContext = executionContext.getFormContext();
        const formType = formContext.ui.getFormType();

        switch (formType) {
            case formTypes.Update:
                disableEmailFieldOnUpdate(formContext);
                break;

            default:
                break;
        }
        initOnChange(formContext);
    };
    const formTypes = {
        Undefined: 0,
        Create: 1,
        Update: 2,
    };
    const initOnChange = function (formContext) {
        formContext.getAttribute("new_emailaddress").addOnChange(function () { setNotificationOnInvalidEmailAddress(formContext) });
        formContext.getAttribute("new_birthdate").addOnChange(function () { validateBirthDate(formContext) });
    };

    const disableEmailFieldOnUpdate = function (formContext) {
        if (!!formContext.data.entity.getId())
            formContext.getControl("new_emailaddress").setDisabled(true);
    };
    const setNotificationOnInvalidEmailAddress = function (formContext) {

        const emailAddressValue = formContext.getAttribute("new_emailaddress").getValue() ?? "";
        var execute_ValidateEmailAdress_Request = {
            // Parameters
            emailAddress: emailAddressValue, // Edm.String

            getMetadata: function () {
                return {
                    boundParameter: null,
                    parameterTypes: {
                        emailAddress: { typeName: "Edm.String", structuralProperty: 1 }
                    },
                    operationType: 0, operationName: "new_ValidateEmailAdress"
                };
            }
        };

        Xrm.WebApi.execute(execute_ValidateEmailAdress_Request).then(
            function success(response) {
                if (response.ok) {

                    response.json().then(function (result) {

                        if (!result.isValid)
                            formContext.getControl("new_emailaddress").setNotification(result.message, "emailNotification");
                        else
                            formContext.getControl("new_emailaddress").clearNotification("emailNotification");

                    });
                }
            }
        ).catch(function (error) {
            console.log(error.message);
        });
    };

    function validateBirthDate(formContext) {
        const selected = formContext.getAttribute("new_birthdate").getValue();

        const yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        if (selected >= yesterday) {
            formContext.getAttribute("new_birthdate").setValue(null);
            formContext.getControl("new_birthdate").setNotification("לא ניתן לבחור תאיך מאוחר מאתמול", "birthdateNotification");

            setTimeout(() => {
                formContext.getControl("new_birthdate").clearNotification("birthdateNotification");
            }, 3000);

        }
        else
            formContext.getControl("new_birthdate").clearNotification("birthdateNotification");

    }



    const onSave = function (executionContext) {
        const formContext = executionContext.getFormContext();
        disableEmailFieldOnUpdate(formContext);
    }
    return {
        onLoad: onLoad,
        onSave: onSave
    };
})();