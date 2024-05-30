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
    public class ContestController : Controller
    {
        private readonly DbLab2Context _context;

        public ContestController(DbLab2Context context)
        {
            _context = context;
        }

        // GET: Contest
        public async Task<IActionResult> Index()
        {
            var dbLab2Context = _context.Contests;
            return View(await dbLab2Context.ToListAsync());
        }

        // GET: Contest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .FirstOrDefaultAsync(m => m.ContestId == id);
            if (contest == null)
            {
                return NotFound();
            }
            ViewData["Problems"] = await _context.ContestProblems.Include(cp => cp.Problem)
                                                        .Where(cp => cp.ContestId == id)
                                                        .Select(cp => cp.Problem)
                                                        .ToListAsync();

            return View(contest);
        }

        // GET: Contest/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login");
            ViewData["Problems"] = new SelectList(_context.Problems, "ProblemId", "Name");
            return View();
        }

        // POST: Contest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContestId,Name,Description,DifficultyLevel,CreatedBy,Editorial,StartTime,EndTime")] Contest contest, List<int>problems)
        {
            if (ModelState.IsValid)
            {
                for(int i = 0; i<problems.Count; i++)
                {
                    var  problem = problems[i];
                    if(problem != -1)
                    {
                        var contestProblem = new ContestProblem(){
                            ProblemId = problem,
                            ContestId = contest.ContestId,
                            Contest = contest,
                        };
                        var duplicate = await _context.ContestProblems.FirstOrDefaultAsync(u => u.ProblemId == problem && u.ContestId == contestProblem.ContestId);
                        if(duplicate != null)
                        {
                            _context.ContestProblems.Remove(duplicate);
                        }
                        _context.ContestProblems.Add(contestProblem);
                    }
                }
                _context.Add(contest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<int> selectedProblems = new List<int>();
            foreach(var problem in problems)
            {
                if(problem != -1)
                {
                    selectedProblems.Add(problem);
                }
            }
            ViewData["SelectedProblems"] = selectedProblems;
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", contest.CreatedBy);
            ViewData["Problems"] = new SelectList(_context.Problems, "ProblemId", "Name");
            return View(contest);
        }

        // GET: Contest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            ViewData["SelectedProblems"] = await _context.ContestProblems.Where(cp => cp.ContestId == id).Select(cp => cp.ProblemId).ToListAsync();
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", contest.CreatedBy);
            ViewData["Problems"] = new SelectList(_context.Problems, "ProblemId", "Name");
            return View(contest);
        }

        // POST: Contest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContestId,Name,Description,DifficultyLevel,CreatedBy,Editorial,StartTime,EndTime")] Contest contest, List<int> problems)
        {
            if (id != contest.ContestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldProblems = await _context.ContestProblems.Where(gu => gu.ContestId == id).ToListAsync();
                    foreach(var problem in oldProblems)
                    {
                        _context.ContestProblems.Remove(problem);
                    }
                    for(int i = 0; i<problems.Count; i++)
                    {
                        var  problem = problems[i];
                        if(problem != -1)
                        {
                            var contestProblem = new ContestProblem(){
                                ProblemId = problem,
                                ContestId = contest.ContestId
                            };
                            var duplicate = await _context.ContestProblems.FirstOrDefaultAsync(u => u.ProblemId == problem && u.ContestId == contestProblem.ContestId);
                            if(duplicate != null)
                            {
                                _context.ContestProblems.Remove(duplicate);
                            }
                            _context.ContestProblems.Add(contestProblem);
                        }
                    }
                    _context.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.ContestId))
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
            List<int> selectedProblems = new List<int>();
            foreach(var problem in problems)
            {
                if(problem != -1)
                {
                    selectedProblems.Add(problem);
                }
            }
            ViewData["SelectedProblems"] = selectedProblems;
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", contest.CreatedBy);
            ViewData["Problems"] = new SelectList(_context.Problems, "ProblemId", "Name");
            return View(contest);
        }

        // GET: Contest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .FirstOrDefaultAsync(m => m.ContestId == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // POST: Contest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            if (contest != null)
            {
                var problems = await _context.ContestProblems.Where(cp => cp.ContestId == id).ToListAsync();
                foreach(var problem in problems)
                {
                    _context.ContestProblems.Remove(problem);
                }

                var groups = await _context.GroupContests.Where(cp => cp.ContestId == id).ToListAsync();
                foreach(var group in groups)
                {
                    _context.GroupContests.Remove(group);
                }


                _context.Contests.Remove(contest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Query5(int? UserId)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            if(UserId == null)
            {
                return View(new List<Contest>());
            }
            var participations = _context.Contests.FromSql(@$"
                                                    SELECT *
                                                    FROM dbo.Contests
                                                    WHERE ContestId IN 
                                                        (SELECT DISTINCT ContestId
                                                        FROM dbo.Submissions
                                                        WHERE SubmittedBy = {UserId})");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            ViewData["User"] = await _context.Users.FindAsync(UserId);
            return View(await participations.ToListAsync());
        }

        private bool ContestExists(int id)
        {
            return _context.Contests.Any(e => e.ContestId == id);
        }
    }
}
