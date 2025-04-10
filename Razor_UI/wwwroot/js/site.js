﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataUpdate") // goi den DataUpdateHub duoc config trong Program.cs
    .build();

// giong "load" ben edit khi goi signalR update
// khi goi signalR update thi se goi den ham nay va truyen duong dan de "load"" lai trang

connection.on("feedback", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/Admin/FeedbackManagement'
});

connection.on("Dang", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/Admin/CateGoryManagement'
});

connection.on("loadProduct", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/Admin/ProductManagement'
});

$('.order-status').change(function () {

    const orderId = $(this).data('order-id');
    const orderStatus = $(this).val();

    $.ajax({
        url: window.location.pathname,
        type: 'POST',
        data: {
            OrderId: orderId,
            OrderStatus: orderStatus,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            console.log("Order status updated successfully");
        },
        error: function (error) {
            console.error("Error updating order status:", error);
        }
    });
});

//order status
connection.on("ReceiveOrderStatusUpdate", function (orderId, orderStatus) {
    $(`select.order-status[data-order-id="${orderId}"]`).val(orderStatus);
});

$('.payment-status').change(function () {

    const orderId = $(this).data('order-id');
    const paymentStatus = $(this).val();

    $.ajax({
        url: window.location.pathname,
        type: 'POST',
        data: {
            OrderId: orderId,
            PaymentStatus: paymentStatus,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            console.log("Payment status updated successfully");
        },
        error: function (error) {
            console.error("Error updating payment status:", error);
        }
    });
});

//payment status
connection.on("ReceivePaymentUpdate", function (orderId, paymentStatus) {
    // Find the row with the matching order ID and update its payment status
    $(`select.payment-status[data-order-id="${orderId}"]`).val(paymentStatus);
});

connection.on("DeleteUser", function () {
    location.href = '/Admin/AccountManagement'
});


connection.start().catch(function (err) {
    return console.error(err.toString());
});