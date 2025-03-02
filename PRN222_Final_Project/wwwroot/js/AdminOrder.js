let orders = [];
let filteredOrders = []; // This will be updated with the fetched data

let currentPage = 1;
const rowsPerPage = 20;
const maxPageButtons = 3; // Max number of page buttons to display
let currentFilter = 'all';
let currentSearch = '';


// Hàm áp dụng bộ lọc, tìm kiếm và sắp xếp ngày
function applyFiltersAndSearch() {
    filteredOrders = orders.filter(order => {
        // Áp dụng bộ lọc theo trạng thái đơn hàng
        if (currentFilter !== 'all') {
            if (currentFilter === "Đơn hàng mới" && order.orderStatus === "PENDING") return true;
            if (currentFilter === "Hoàn thành" && order.orderStatus === "COMPLETED") return true;
            if (currentFilter === "Hủy đơn hàng" && order.orderStatus === "CANCELLED") return true;
            return false;
        }
        return true;  // Không áp dụng lọc nếu chọn "Tất cả"
    });

    // Tìm kiếm theo mã đơn hàng hoặc tên khách hàng
    if (currentSearch.trim() !== '') {
        const searchTerm = currentSearch.trim().toLowerCase();
        filteredOrders = filteredOrders.filter(order =>
            order.id.toString().includes(searchTerm) ||
            order.account.name.toLowerCase().includes(searchTerm)
        );
    }

    // Sắp xếp đơn hàng theo ngày (mới nhất lên trên)
    filteredOrders.sort((a, b) => b.orderDate - a.orderDate);

    currentPage = 1; // Đặt lại trang hiện tại về 1 khi thay đổi bộ lọc
    displayOrders(); // Hiển thị lại danh sách đơn hàng đã lọc
}

// Lắng nghe sự kiện thay đổi bộ lọc trạng thái
document.getElementById('filterSelect').addEventListener('change', (event) => {
    currentFilter = event.target.value;  // Cập nhật giá trị bộ lọc
    applyFiltersAndSearch();  // Áp dụng bộ lọc và tìm kiếm lại
});

async function fetchOrders() {
    try {
        // Replace 'yourToken' with the actual token
        // const token = 'authToken';

        // Fetch data from the API
        const response = await fetch('https://thorough-louse-notably.ngrok-free.app/auth/getallorder', {
            method: 'GET',
            headers: {
                'ngrok-skip-browser-warning': '1',
                // 'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        // Check if the response is ok (status 200)
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        // Parse the JSON data
        const data = await response.json();

        // Call the function to populate the table
        populateOrderTable(data);
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

// Function to format the timestamp to a readable date
function formatDate(timestamp) {
    const date = new Date(timestamp);
    return date.toLocaleString(); // This will convert it to a readable date format
}

// Populate the table with data
function populateOrderTable(orders) {
    const tableBody = document.querySelector('#orderTable tbody');
    const cancelTotal = document.querySelector('#cancelTotal');
    const orderTotal = document.querySelector('#orderTotal');

    let totalCancel = 0;
    let totalOrder = 0;

    // Clear the current table rows
    tableBody.innerHTML = '';

    // Loop through each order and create a table row
    orders.forEach(order => {
        const row = document.createElement('tr');

        // Create table cells and insert data
        row.innerHTML = `
            <td>${order.id}</td>
            <td>${order.account.name}</td>
            <td>${order.orderStatus}</td>
            <td>${formatDate(order.orderDate)}</td>
            <td>
                <button class="action-button accept-button">Accept</button>
                <button class="action-button cancel-button">Cancel</button>
            </td>
            <td>${order.total}đ</td>
        `;

        // Append the row to the table body
        tableBody.appendChild(row);

        // Add the order total to the total
        totalOrder += order.total;

        // Add event listeners for the Accept and Cancel buttons
        const acceptButton = row.querySelector('.accept-button');
        const cancelButton = row.querySelector('.cancel-button');

        // Handle the Accept button action
        acceptButton.addEventListener('click', () => {
            handleAcceptOrder(order.id);
        });

        // Handle the Cancel button action
        cancelButton.addEventListener('click', () => {
            handleCancelOrder(order.id);
        });
    });

    // Update the totals
    orderTotal.textContent = `${totalOrder}đ`;
    cancelTotal.textContent = `${totalCancel}đ`; // You can update this if there's logic for canceled orders
}

// Function to handle the "Accept" action
async function handleAcceptOrder(orderId) {
    try {
        const response = await fetch(`https://thorough-louse-notably.ngrok-free.app/auth/approve/${orderId}`, {
            method: 'POST',
            headers: {
                'ngrok-skip-browser-warning': '1',
                // Authorization: `Bearer ${localStorage.getItem("authToken")}`,
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error('Failed to accept the order');
        }

        const data = await response.json();
        alert(`Order ${orderId} accepted successfully!`);
        fetchOrders(); // Refresh the order list after accepting
    } catch (error) {
        console.error('Error accepting the order:', error);
    }
}

// Function to handle the "Cancel" action
async function handleCancelOrder(orderId) {
    try {
        const response = await fetch(`https://thorough-louse-notably.ngrok-free.app/auth/cancel/${orderId}`, {
            method: 'POST',
            headers: {
                'ngrok-skip-browser-warning': '1',
                Authorization: `Bearer ${localStorage.getItem("authToken")}`,
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error('Failed to cancel the order');
        }

        const data = await response.json();
        alert(`Order ${orderId} canceled successfully!`);
        fetchOrders(); // Refresh the order list after canceling
    } catch (error) {
        console.error('Error canceling the order:', error);
    }
}

// Call the fetch function to get the orders and populate the table
fetchOrders();

// Hàm hiển thị đơn hàng
function displayOrders() {
    const tableBody = document.querySelector('#orderTable tbody');
    tableBody.innerHTML = '';

    // Nếu không có đơn hàng nào, hiển thị thông báo
    if (filteredOrders.length === 0) {
        const row = document.createElement('tr');
        const cell = document.createElement('td');
        cell.colSpan = 6; // Set colspan to cover all columns
        cell.textContent = 'Không có đơn hàng nào để hiển thị';
        row.appendChild(cell);
        tableBody.appendChild(row);
        return;
    }

    const start = (currentPage - 1) * rowsPerPage;
    const end = start + rowsPerPage;
    const currentOrders = filteredOrders.slice(start, end);

    currentOrders.forEach(order => {
        const row = document.createElement('tr');

        // ID
        const idCell = document.createElement('td');
        idCell.textContent = order.id;
        row.appendChild(idCell);

        // Name
        const nameCell = document.createElement('td');
        nameCell.textContent = order.account.name;
        row.appendChild(nameCell);

        // Status
        const statusCell = document.createElement('td');
        statusCell.textContent = order.orderStatus === "PENDING" ? "Đơn hàng mới" :
            order.orderStatus === "COMPLETED" ? "Hoàn thành" :
                order.orderStatus === "CANCELLED" ? "Hủy đơn hàng" : "Chưa xác định";
        statusCell.classList.add(order.orderStatus === "PENDING" ? 'status-new' :
            order.orderStatus === "COMPLETED" ? 'status-completed' : 'status-canceled');
        row.appendChild(statusCell);

        // Date
        const dateCell = document.createElement('td');
        const orderDate = new Date(order.orderDate);
        dateCell.textContent = orderDate.toLocaleDateString('vi-VN');
        row.appendChild(dateCell);

        // Control Buttons
        const controlCell = document.createElement('td');
        const controlDiv = document.createElement('div');
        controlDiv.classList.add('control-buttons');

        const checkBtn = document.createElement('button');
        checkBtn.innerHTML = '<i class="fa-solid fa-check"></i>';
        checkBtn.title = "Duyệt Đơn Hàng";
        checkBtn.classList.add('check-btn');

        const xBtn = document.createElement('button');
        xBtn.innerHTML = '<i class="fa-solid fa-times"></i>';
        xBtn.title = "Từ chối Đơn Hàng";
        xBtn.classList.add('x-btn');

        const deleteBtn = document.createElement('button');
        deleteBtn.innerHTML = '<i class="fa-solid fa-trash"></i>';
        deleteBtn.title = "Xóa Đơn Hàng";
        deleteBtn.classList.add('delete-btn');

        if (order.orderStatus === "CANCELLED" || order.orderStatus === "COMPLETED") {
            checkBtn.classList.add('disabled');
            xBtn.classList.add('disabled');
            deleteBtn.classList.add('disabled');
        } else {
            checkBtn.onclick = () => approveOrder(order.id);
            xBtn.onclick = () => rejectOrder(order.id);
            deleteBtn.onclick = () => deleteOrder(order.id);
        }

        controlDiv.appendChild(checkBtn);
        controlDiv.appendChild(xBtn);
        controlDiv.appendChild(deleteBtn);
        controlCell.appendChild(controlDiv);
        row.appendChild(controlCell);

        // Total
        const totalCell = document.createElement('td');
        totalCell.textContent = formatCurrency(order.total);
        row.appendChild(totalCell);

        tableBody.appendChild(row);
    });

    calculateTotal();
    displayPagination();
}

// Hàm tính tổng tiền đơn hàng và tổng tiền hủy
function calculateTotal() {
    const cancelTotal = filteredOrders
        .filter(order => order.orderStatus === "CANCELLED")
        .reduce((acc, order) => acc + order.total, 0);

    const orderTotal = filteredOrders
        .filter(order => order.orderStatus !== "CANCELLED")
        .reduce((acc, order) => acc + order.total, 0);

    document.getElementById('cancelTotal').innerHTML = `<strong>${formatCurrency(cancelTotal)}</strong>`;
    document.getElementById('orderTotal').innerHTML = `<strong>${formatCurrency(orderTotal)}</strong>`;
}

// Hàm chuyển đổi định dạng tiền tệ
function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN') + 'đ';
}

// Hàm áp dụng bộ lọc, tìm kiếm và sắp xếp ngày
function applyFiltersAndSearch() {
    filteredOrders = orders.filter(order => {
        // Apply filter based on order status
        if (currentFilter !== 'all') {
            const filterStatusOptions = ["Đơn hàng mới", "Hoàn thành", "Hủy đơn hàng"];
            const filterPaymentOptions = ["Đã thanh toán", "Chưa thanh toán"];

            // Map the current filter to the correct order status
            if (filterStatusOptions.includes(currentFilter)) return order.orderStatus === (currentFilter === "Đơn hàng mới" ? "PENDING" :
                currentFilter === "Hoàn thành" ? "COMPLETED" : "CANCELLED");
        }
        return true;
    });

    // Search by order id or customer name
    if (currentSearch.trim() !== '') {
        const searchTerm = currentSearch.trim().toLowerCase();
        filteredOrders = filteredOrders.filter(order =>
            order.id.toString().includes(searchTerm) ||
            order.account.name.toLowerCase().includes(searchTerm)
        );
    }

    // Sort by order date from recent to oldest
    filteredOrders.sort((a, b) => b.orderDate - a.orderDate);

    currentPage = 1;
    displayOrders();
}

// Hàm phân trang
function displayPagination() {
    const pagination = document.getElementById('pagination');
    pagination.innerHTML = '';

    const totalPages = Math.ceil(filteredOrders.length / rowsPerPage);
    const startPage = Math.max(1, currentPage - Math.floor(maxPageButtons / 2));
    const endPage = Math.min(totalPages, startPage + maxPageButtons - 1);

    if (currentPage > 1) {
        const prevButton = document.createElement('button');
        prevButton.textContent = '« Trước';
        prevButton.onclick = () => { currentPage--; displayOrders(); };
        pagination.appendChild(prevButton);
    }

    for (let page = startPage; page <= endPage; page++) {
        const pageButton = document.createElement('button');
        pageButton.textContent = page;
        if (page === currentPage) pageButton.disabled = true;
        pageButton.onclick = () => { currentPage = page; displayOrders(); };
        pagination.appendChild(pageButton);
    }

    if (currentPage < totalPages) {
        const nextButton = document.createElement('button');
        nextButton.textContent = 'Sau »';
        nextButton.onclick = () => { currentPage++; displayOrders(); };
        pagination.appendChild(nextButton);
    }
}

// Hàm xử lý duyệt đơn hàng
function approveOrder(id) {
    const order = orders.find(order => order.id === id);
    if (order && order.orderStatus === "PENDING") {
        order.orderStatus = "COMPLETED";
        applyFiltersAndSearch();
    }
}

// Hàm xử lý từ chối đơn hàng
function rejectOrder(id) {
    const order = orders.find(order => order.id === id);
    if (order && order.orderStatus === "PENDING") {
        order.orderStatus = "CANCELLED";
        applyFiltersAndSearch();
    }
}

// Hàm xử lý xóa đơn hàng
function deleteOrder(id) {
    const index = orders.findIndex(order => order.id === id);
    if (index !== -1) {
        orders.splice(index, 1);
        filteredOrders = [...orders];
        applyFiltersAndSearch();
    }
}

// Lắng nghe sự kiện tìm kiếm và lọc
document.getElementById('searchInput').addEventListener('input', (event) => {
    currentSearch = event.target.value;
    applyFiltersAndSearch();
});

document.getElementById('statusFilter').addEventListener('change', (event) => {
    currentFilter = event.target.value;
    applyFiltersAndSearch();
});

// Tải dữ liệu khi trang được load
window.onload = fetchOrders;
