// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataUpdate") // goi den DataUpdateHub duoc config trong Program.cs
    .build();

// giong "load" ben edit khi goi signalR update
// khi goi signalR update thi se goi den ham nay va truyen duong dan de "load"" lai trang
connection.on("load", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/StudentCrud/Index'
});
connection.on("loadCateGory", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/Admin/CateGoryManagement'
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});