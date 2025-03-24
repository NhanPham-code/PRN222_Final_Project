using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService
    {
        private readonly ICrudRepo<User, int> _userRepo;

        public UserService(ICrudRepo<User, int> userRepo)
        {
            this._userRepo = userRepo;
        }

        // Kiểm tra mật khẩu
        private bool VerifyPassword(string password, byte[] storedSalt, byte[] storedHash)
        {
            using (var hmac = new HMACSHA512(storedSalt)) // Dùng lại storedSalt để tạo HMACSHA512
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Tạo hash mới từ password nhập vào

                // So sánh toàn bộ 64 byte
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private void HashPassword(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // Tạo salt (128 byte)
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Băm ra 64 byte
            }
        }

        public async Task<bool> CheckUserExits(string email)
        {
            var users = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Orders);
            var user = users.FirstOrDefault(u => string.Equals(u.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            return user != null;
        }

        public async Task<User?> Login(string email, string password)
        {
            var users = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Orders);
            var user = users.FirstOrDefault(u => string.Equals(u.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            if (!VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User?> Register(string fullname, string email, string address, string phone, string password)
        {
            if(string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address)
                               || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            // Mã hóa mật khẩu
            byte[] passwordSalt, passwordHash;
            HashPassword(password, out passwordSalt, out passwordHash);

            // Tạo user mới
            User newUser = new User
            {
                FullName = fullname,
                Email = email,
                Address = address,
                PhoneNumber = phone,
                RegistrationDate = DateTime.Now,
                Role = "Customer",
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            };

            // Lưu user vào database
            await _userRepo.Add(newUser);

            return newUser;
        }

        public async Task<User> GetUserById(int UserId)
        {
            var user = await _userRepo.GetByIdWithInclude(UserId, "UserId", u => u.Carts, u => u.Feedbacks, u => u.Orders);
            return user;
        }

        public async Task UpdateUser(User user)
        {
            if (user == null)
            {
                return;
            }

            await _userRepo.Update(user);
        }
        
        public async Task<IEnumerable<User>> GetAllUser()
        {
            var userList = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Orders, u => u.Feedbacks);
            return (userList.ToList());
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _userRepo.GetByIdWithInclude(userId, "UserId", u => u.Feedbacks, u => u.Carts, u => u.Orders);
            

            if (user == null)
            {
                return false;
            } else if (user.Feedbacks.Any() || user.Orders.Any() || user.Carts.Any())
            {
                return false;
            } else
            {
                await _userRepo.Delete(user);
                return true;
            }
        }

        public async Task<bool> ResetPasswordByEmail(string email, string newPassword)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return false;
            }

            // get user by email
            var user = await GetUserByEmail(email);

            if(user == null)
            {
                return false;
            }

            // Hash password
            byte[] passwordSalt, passwordHash;
            HashPassword(newPassword, out passwordSalt, out passwordHash);

            // set new password
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            // update to DB
            await _userRepo.Update(user);

            return true;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var userList = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Feedbacks, u => u.Orders);

            var user = userList.FirstOrDefault(u => u.Email == email);
            return user;
        }

        public async Task<IEnumerable<User>> Search(string searchKey)
        {
            searchKey = searchKey.ToLower();

            var userList = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Feedbacks, u => u.Orders);

            var searchListResult = userList.Where(u => u.Email.ToLower().Contains(searchKey) || u.FullName.ToLower().Contains(searchKey)
                                            || u.PhoneNumber.Contains(searchKey))
                                            .ToList();

            return searchListResult;
        }
    }
}
