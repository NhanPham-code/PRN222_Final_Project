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