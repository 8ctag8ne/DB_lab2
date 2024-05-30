using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DB_lab2;

namespace DB_lab2.Controllers_
{
    public class SubmissionController : Controller
    {
        private readonly DbLab2Context _context;

        public SubmissionController(DbLab2Context context)
        {
            _context = context;
        }

        // GET: Submission
        public async Task<IActionResult> Index()
        {
            var dbLab2Context = _context.Submissions.Include(s => s.Contest).Include(s => s.Group).Include(s => s.Problem).Include(s => s.SubmittedByNavigation);
            return View(await dbLab2Context.ToListAsync());
        }

        // GET: Submission/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .Include(s => s.Contest)
                .Include(s => s.Group)
                .Include(s => s.Problem)
                .Include(s => s.SubmittedByNavigation)
                .FirstOrDefaultAsync(m => m.SubmissionId == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // GET: Submission/Create
        public IActionResult Create()
        {
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name");
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name");
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name");
            ViewData["SubmittedBy"] = new SelectList(_context.Users, "UserId", "Login");
            List<string> lang = ["C++", "C#", "Python", "Go", "Pascal", "Rust"];
            ViewData["Language"] = new SelectList(lang);
            List<string> verdict = ["AC", "WA", "TL", "ML", "RE", "Ignored"];
            ViewData["Verdict"] = new SelectList(verdict);

            return View();
        }

        // POST: Submission/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubmissionId,GroupId,ContestId,SubmittedBy,ProblemId,Result,Verdict,SubmittedAt,Code,Language")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name", submission.ContestId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", submission.GroupId);
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name", submission.ProblemId);
            ViewData["SubmittedBy"] = new SelectList(_context.Users, "UserId", "Login", submission.SubmittedBy);
            List<string> lang = ["C++", "C#", "Python", "Go", "Pascal", "Rust"];
            ViewData["Language"] = new SelectList(lang);
            List<string> verdict = ["AC", "WA", "TL", "ML", "RE", "Ignored"];
            ViewData["Verdict"] = new SelectList(verdict);
            return View(submission);
        }

        // GET: Submission/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name", submission.ContestId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", submission.GroupId);
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name", submission.ProblemId);
            ViewData["SubmittedBy"] = new SelectList(_context.Users, "UserId", "Login", submission.SubmittedBy);
            List<string> lang = ["C++", "C#", "Python", "Go", "Pascal", "Rust"];
            ViewData["Language"] = new SelectList(lang);
            List<string> verdict = ["AC", "WA", "TL", "ML", "RE", "Ignored"];
            ViewData["Verdict"] = new SelectList(verdict);
            return View(submission);
        }

        // POST: Submission/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubmissionId,GroupId,ContestId,SubmittedBy,ProblemId,Result,Verdict,SubmittedAt,Code,Language")] Submission submission)
        {
            if (id != submission.SubmissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.SubmissionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name", submission.ContestId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", submission.GroupId);
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name", submission.ProblemId);
            ViewData["SubmittedBy"] = new SelectList(_context.Users, "UserId", "Login", submission.SubmittedBy);
            List<string> lang = ["C++", "C#", "Python", "Go", "Pascal", "Rust"];
            ViewData["Language"] = new SelectList(lang);
            List<string> verdict = ["AC", "WA", "TL", "ML", "RE", "Ignored"];
            ViewData["Verdict"] = new SelectList(verdict);
            return View(submission);
        }

        // GET: Submission/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .Include(s => s.Contest)
                .Include(s => s.Group)
                .Include(s => s.Problem)
                .Include(s => s.SubmittedByNavigation)
                .FirstOrDefaultAsync(m => m.SubmissionId == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Submission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission != null)
            {
                _context.Submissions.Remove(submission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.SubmissionId == id);
        }
    }
}
