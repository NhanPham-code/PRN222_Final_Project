﻿using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN222_Final_Project.Controllers
{
    public class ContactController : Controller
    {
        private ICrudRepo<Feedback, int> _feedbackRepo;

        public ContactController(ICrudRepo<Feedback, int> feedbackRepo)
        {
            this._feedbackRepo = feedbackRepo;
        }

        public async Task<IActionResult> Index()
        {
            var feedbacks = await _feedbackRepo.GetAllWithInclude(f => f.User);
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var customerFeedback = feedbacks.FirstOrDefault(f => f.User.UserId == userId.Value);
                ViewBag.CustomerFeedback = customerFeedback;
            }

            return View(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(string suggestion)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Common");
            }

            int parsedCustomerId = userId.Value;

            var feedbacks = await _feedbackRepo.GetAllWithInclude(f => f.User);
            Console.WriteLine($"Tổng số feedback lấy được: {feedbacks.Count()}");

            var existingFeedback = feedbacks.FirstOrDefault(f => f.User != null && f.User.UserId == parsedCustomerId);

            if (existingFeedback != null)
            {
                Console.WriteLine("Cập nhật feedback cũ.");
                existingFeedback.Description = suggestion;
                await _feedbackRepo.Update(existingFeedback);
            }
            else
            {
                Console.WriteLine("Thêm mới feedback.");
                var newFeedback = new Feedback
                {
                    UserId = parsedCustomerId,
                    Description = suggestion,
                    SubmittedDate = DateTime.Now
                };
                await _feedbackRepo.Add(newFeedback);
            }

            return RedirectToAction("Index");
        }
    }
}