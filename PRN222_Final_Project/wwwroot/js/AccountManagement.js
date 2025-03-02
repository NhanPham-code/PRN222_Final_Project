document.addEventListener('DOMContentLoaded', function () {
    const accountTable = document.getElementById('accountTable');
    const searchInput = document.getElementById('search');
    const searchBtn = document.getElementById('searchBtn');
    const addAccountBtn = document.getElementById('addAccountBtn');
    const formContainer = document.getElementById('formContainer');
    const accountForm = document.getElementById('accountForm');
    const formTitle = document.getElementById('formTitle');
    const cancelFormBtn = document.getElementById('cancelForm');
    const paginationContainer = document.getElementById('pagination');

    let accounts = [];
    let nextId = 1;

    let token = localStorage.getItem('authToken');
    const apiUrl = 'https://thorough-louse-notably.ngrok-free.app/auth/doctors-and-staff';

    async function fetchAccounts() {
        try {
            const response = await fetch(apiUrl, {
                method: 'GET',
                headers: {
                    "ngrok-skip-browser-warning": "1",
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error fetching accounts:', errorText);
                return;
            }

            const responseText = await response.text();
            if (responseText) {
                const data = JSON.parse(responseText);
                accounts = data;
                nextId = Math.max(...accounts.map(account => account.id)) + 1;
                renderAccounts();
            }
        } catch (error) {
            console.error('Error fetching accounts:', error);
        }
    }

    function handleSearch() {
        const searchText = searchInput.value.toLowerCase();
        const filteredAccounts = accounts.filter(account => {
            const accountRole = account.role || ''; // Lấy vai trò của tài khoản
            return account.email.toLowerCase().includes(searchText) || accountRole.toLowerCase().includes(searchText);
        });
        renderAccounts(filteredAccounts); // Render lại danh sách tài khoản đã lọc
    }

    searchBtn.addEventListener('click', function () {
        handleSearch(); // Tìm kiếm khi nhấn nút "Tìm Kiếm"
    });

    searchInput.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            handleSearch(); // Tìm kiếm khi nhấn Enter trong ô input
        }
    });


    fetchAccounts();

    let editingRow = null;
    const itemsPerPage = 15;
    let currentPage = 1;

    function renderAccounts(filteredAccounts = accounts) {
        accountTable.innerHTML = '';

        const totalItems = filteredAccounts.length;
        const totalPages = Math.ceil(totalItems / itemsPerPage);
        const startIndex = (currentPage - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;

        const currentAccounts = filteredAccounts.slice(startIndex, endIndex);

        currentAccounts.forEach((account) => {
            let status = account.active ? 'Đang hoạt động' : 'Ngưng hoạt động';
            let statusColor = account.active ? 'green' : 'red';

            let role = '';
            if (account.authorities && account.authorities.length > 0) {
                const authority = account.authorities[0].authority;
                role = authority === 'ROLE_DOCTOR' ? 'Bác sĩ' : authority === 'ROLE_STAFF' ? 'Nhân viên' : 'Chưa xác định';
            }

            const dateCreated = new Date(account.registerDate).toLocaleString();
            const password = account.password;

            const newRow = document.createElement('tr');
            newRow.innerHTML =
                `<td>${account.id}</td>
                <td>${account.email}</td>
                <td>
                    <div class="password-container">
                        <input type="password" class="password-input" value="${password}" readonly />
                        <i class="fa fa-eye toggle-password" style="cursor: pointer;"></i>
                    </div>
                </td>
                <td>${role}</td>
                <td style="color: ${statusColor}; font-weight: bold;">${status}</td>
                <td>${dateCreated}</td>
                <td class="actions">
                    <button class="icon user fa fa-user"></button>
                    <button class="icon inactive fa fa-times"></button>
                    <button class="icon delete fa fa-trash"></button>
                </td>`;

            accountTable.appendChild(newRow);

            const passwordInput = newRow.querySelector('.password-input');
            const togglePasswordBtn = newRow.querySelector('.toggle-password');

            togglePasswordBtn.addEventListener('click', function () {
                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    togglePasswordBtn.classList.remove('fa-eye');
                    togglePasswordBtn.classList.add('fa-eye-slash');
                } else {
                    passwordInput.type = 'password';
                    togglePasswordBtn.classList.remove('fa-eye-slash');
                    togglePasswordBtn.classList.add('fa-eye');
                }
            });

            const userBtn = newRow.querySelector('.user');
            userBtn.addEventListener('click', function () {
                if (!account.active) {
                    if (confirm('Bạn có chắc chắn muốn kích hoạt lại tài khoản này không?')) {
                        activateAccount(account.id);
                    }
                } else {
                    alert('Tài khoản này đã đang hoạt động.');
                }
            });

            const inactiveBtn = newRow.querySelector('.inactive');
            inactiveBtn.addEventListener('click', function () {
                if (account.active) {
                    if (confirm('Bạn có chắc chắn muốn ngừng hoạt động tài khoản này không?')) {
                        banAccount(account.id);
                    }
                } else {
                    alert('Tài khoản này đã ngừng hoạt động.');
                }
            });

            const deleteBtn = newRow.querySelector('.delete');
            deleteBtn.addEventListener('click', function () {
                if (confirm('Bạn có chắc chắn muốn xóa tài khoản này vĩnh viễn không?')) {
                    deleteAccount(account.id);
                }
            });
        });

        createPagination(totalPages); // Sử dụng hàm đã định nghĩa
    }

    function createPagination(totalPages) {
        paginationContainer.innerHTML = '';

        const prevBtn = document.createElement('button');
        prevBtn.innerHTML = 'Previous';
        prevBtn.disabled = currentPage === 1;
        prevBtn.addEventListener('click', function () {
            if (currentPage > 1) {
                currentPage--;
                renderAccounts();
            }
        });
        paginationContainer.appendChild(prevBtn);

        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.innerHTML = i;
            pageBtn.classList.toggle('active', i === currentPage);
            pageBtn.addEventListener('click', function () {
                currentPage = i;
                renderAccounts();
            });
            paginationContainer.appendChild(pageBtn);
        }

        const nextBtn = document.createElement('button');
        nextBtn.innerHTML = 'Next';
        nextBtn.disabled = currentPage === totalPages;
        nextBtn.addEventListener('click', function () {
            if (currentPage < totalPages) {
                currentPage++;
                renderAccounts();
            }
        });
        paginationContainer.appendChild(nextBtn);
    }

    async function banAccount(accountId) {
        try {
            const response = await fetch(`https://thorough-louse-notably.ngrok-free.app/auth/ban/${accountId}`, {
                method: 'PUT',
                headers: {
                    "ngrok-skip-browser-warning": "1",
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ active: false })
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error banning account:', errorText);
                return;
            }

            alert('Tài khoản đã được ngừng hoạt động!');
            await fetchAccounts();
        } catch (error) {
            console.error('Error banning account:', error);
        }
    }

    async function activateAccount(accountId) {
        try {
            const response = await fetch(`https://thorough-louse-notably.ngrok-free.app/auth/stop/${accountId}`, {
                method: 'PUT',
                headers: {
                    "ngrok-skip-browser-warning": "1",
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ active: true })
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error activating account:', errorText);
                return;
            }

            alert('Tài khoản đã được kích hoạt lại!');
            await fetchAccounts();
        } catch (error) {
            console.error('Error activating account:', error);
        }
    }

    async function deleteAccount(accountId) {
        try {
            const response = await fetch(`https://thorough-louse-notably.ngrok-free.app/auth/delete/${accountId}`, {
                method: 'DELETE',
                headers: {
                    "ngrok-skip-browser-warning": "1",
                    "Authorization": `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error deleting account:', errorText);
                return;
            }

            alert('Tài khoản đã được xóa vĩnh viễn!');
            await fetchAccounts();
        } catch (error) {
            console.error('Error deleting account:', error);
        }
    }

    addAccountBtn.addEventListener('click', function () {
        formTitle.textContent = 'Thêm tài khoản';
        formContainer.style.display = 'flex';
        accountForm.reset();
    });

    accountForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        const emailValue = document.getElementById('formUsername').value;
        const passwordValue = document.getElementById('formPassword').value;
        const roleValue = document.getElementById('role').value;

        const newAccount = {
            email: emailValue,
            pass: passwordValue,
            role: roleValue
        };

        try {
            const response = await fetch('https://thorough-louse-notably.ngrok-free.app/auth/createaccount', {
                method: 'POST',
                headers: {
                    "ngrok-skip-browser-warning": "1",
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(newAccount)
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error adding account:', errorText);
            }

            await fetchAccounts();
            formContainer.style.display = 'none';
            accountForm.reset();
        } catch (error) {
            console.error('Error adding account:', error);
        }
    });

    cancelFormBtn.addEventListener('click', function () {
        formContainer.style.display = 'none';
    });

    function handleSearch() {
        const searchText = searchInput.value.toLowerCase();
        const filteredAccounts = accounts.filter(account => {
            const accountRole = account.role || '';
            return account.email.toLowerCase().includes(searchText) || accountRole.toLowerCase().includes(searchText);
        });
        renderAccounts(filteredAccounts);
    }

    searchBtn.addEventListener('click', function () {
        handleSearch();
    });

    searchInput.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            handleSearch();
        }
    });
});
