using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Freelance.Data;
using Freelance.Models;
using System.Security.Claims;

namespace Freelance.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int QuestionId)
        {
            ViewBag.UserId = GetUserID();
            ViewBag.QuestionId = QuestionId;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Date,UserId,QuestionId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var Question = _context.Questions.Find(comment.QuestionId);
                Question.Comments.Add(comment);
                _context.Update(Question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Questions", new { id = comment.QuestionId });
            }
            ViewBag.UserId = GetUserID();
            ViewBag.QuestionId = Request.Form["QuestionId"];
            return View(comment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Questions", new { id = comment.QuestionId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
        private bool IsAuth() => HttpContext.User.Identity.IsAuthenticated;
        private string GetUserID() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
