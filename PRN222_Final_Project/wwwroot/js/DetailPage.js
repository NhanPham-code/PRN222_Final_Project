document.addEventListener('DOMContentLoaded', function () {
    const decrementButton = document.getElementById('decrement-button');
    const incrementButton = document.getElementById('increment-button');
    const quantityInput = document.getElementById('quantity-input');

    // Đảm bảo giá trị mặc định là 1 nếu input rỗng hoặc không phải số
    if (quantityInput.value === '' || isNaN(quantityInput.value)) {
        quantityInput.value = 1;
    }

    // Xử lý sự kiện giảm số lượng
    decrementButton.addEventListener('click', function () {
        let currentQuantity = parseInt(quantityInput.value);
        if (currentQuantity > 1) {
            quantityInput.value = currentQuantity - 1;
        }
    });

    // Xử lý sự kiện tăng số lượng
    incrementButton.addEventListener('click', function () {
        let currentQuantity = parseInt(quantityInput.value);
        if (!isNaN(currentQuantity)) {
            quantityInput.value = currentQuantity + 1;
        }
    });
});