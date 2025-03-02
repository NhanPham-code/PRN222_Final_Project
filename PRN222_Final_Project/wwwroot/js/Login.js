// Lấy các phần tử từ DOM
const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');

// Thêm sự kiện cho nút Đăng ký và Đăng nhập để chuyển đổi giao diện
signUpButton.addEventListener('click', () => {
    container.classList.add("right-panel-active");
});

signInButton.addEventListener('click', () => {
    container.classList.remove("right-panel-active");
});



// Xử lý sự kiện khi người dùng submit form đăng ký
// Xử lý sự kiện khi người dùng submit form đăng ký
// Xử lý sự kiện khi người dùng submit form đăng ký
// Xử lý sự kiện khi người dùng submit form đăng ký
document.querySelector('.sign-up-container form').addEventListener('submit', async function (event) {
    event.preventDefault();

    // Lấy giá trị từ các input
    const nameInput = this.querySelector('input[type="text"]');
    const emailInput = this.querySelector('input[type="email"]');
    const passwordInput = this.querySelector('input[type="password"]');
    const name = nameInput.value.trim();
    const email = emailInput.value.trim();
    const password = passwordInput.value;

    let hasError = false;

    // Reset thông báo lỗi
    document.getElementById('signupNameError').innerText = '';
    document.getElementById('signupEmailError').innerText = '';
    document.getElementById('signupPasswordError').innerText = '';

    // Kiểm tra độ dài tên (ít nhất 3 ký tự)
    if (name.length < 3) {
        document.getElementById('signupNameError').innerText = 'Tên phải có ít nhất 3 ký tự.';
        hasError = true;
    }

    // Kiểm tra định dạng email (chỉ chấp nhận Gmail)
    const emailPattern = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;
    if (!emailPattern.test(email)) {
        document.getElementById('signupEmailError').innerText = 'Vui lòng nhập email Gmail hợp lệ.';
        hasError = true;
    }

    // Kiểm tra độ dài mật khẩu
    if (password.length < 6) {
        alert('Mật khẩu phải có ít nhất 6 ký tự.');
        return;
    }


    // Nếu có lỗi, không gửi yêu cầu đăng ký
    if (hasError) {
        return;
    }

    try {
        const response = await fetch('https://thorough-louse-notably.ngrok-free.app/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, email, password })
        });

        if (response.ok) {
            const data = await response.json();
            alert('Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.');
            // Chuyển sang giao diện đăng nhập
            container.classList.remove("right-panel-active");
        } else {
            const errorData = await response.json();
            alert('Đăng ký không thành công: ' + (errorData.message || 'Vui lòng thử lại.'));
        }
    } catch (error) {
        console.error("Đã có lỗi xảy ra:", error);
        alert("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
    }
});

// Xử lý hiển thị mật khẩu cho form đăng ký
const toggleSignupPassword = document.getElementById('toggleSignupPassword');
const signupPasswordField = document.getElementById('signupPassword');

toggleSignupPassword.addEventListener('click', function () {
    const type = signupPasswordField.type === 'password' ? 'text' : 'password';
    signupPasswordField.type = type;
    this.classList.toggle('fa-eye-slash');
    this.classList.toggle('fa-eye');
    this.setAttribute('aria-label', type === 'password' ? 'Hiện mật khẩu' : 'Ẩn mật khẩu');
});


// Xử lý hiển thị mật khẩu cho form đăng nhập
const togglePassword = document.getElementById('togglePassword');
const passwordField = document.getElementById('password');

togglePassword.addEventListener('click', function () {
    const type = passwordField.type === 'password' ? 'text' : 'password';
    passwordField.type = type;
    this.classList.toggle('fa-eye-slash');
    this.classList.toggle('fa-eye');
    this.setAttribute('aria-label', type === 'password' ? 'Hiện mật khẩu' : 'Ẩn mật khẩu');
});