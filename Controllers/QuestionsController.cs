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

namespace FreelanceApp.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Questions";
            ViewBag.IsAuth = IsAuth();
            return View(await _context.Questions.Include(d => d.User).ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Question = await _context.Questions.Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Question == null)
            {
                return NotFound();
            }

            ViewBag.IsAuth = IsAuth();
            ViewBag.Comments = await _context.Comments.Include(c => c.User)
                .Where(c => c.QuestionId == id).ToListAsync();
            ViewBag.UserId = GetUserID();
            return View(Question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewBag.UserId = GetUserID();
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Topic,Description,Date,UserId")] Question Question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = GetUserID();
            return View(Question);
        }

        public async Task<IActionResult> MyQuestions()
        {
            ViewBag.Title = "My Questions";
            ViewBag.IsAuth = IsAuth();
            return View("~/Views/Questions/Index.cshtml", await _context.Questions.Include(d => d.User)
                .Where(d => d.UserId == GetUserID()).ToListAsync());
        }
        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
        private bool IsAuth() => HttpContext.User.Identity.IsAuthenticated;
        private string GetUserID() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
