$(document).ready(function () {
    $('#uriForm').bootstrapValidator({
        feedbackIcons: {
            valid: 'fa fa-check',
            invalid: 'fa fa-ban',
            validating: 'fa fa-refresh'
        },
        fields: {
            website: {
                validators: {
                    uri: {
                        message: 'The website URL is not valid'
                    }
                }
            }
        }
    });
});