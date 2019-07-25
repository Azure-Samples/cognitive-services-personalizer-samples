// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function cleanChilds(element) {
    while (element.firstChild) {
        element.removeChild(element.firstChild);
    }
}

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});