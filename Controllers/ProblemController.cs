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
    public class ProblemController : Controller
    {
        private readonly DbLab2Context _context;

        public ProblemController(DbLab2Context context)
        {
            _context = context;
        }

        // GET: Problem
        public async Task<IActionResult> Index()
        {
            var dbLab2Context = _context.Problems.Include(p => p.CreatedByNavigation);
            return View(await dbLab2Context.ToListAsync());
        }

        // GET: Problem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problems
                .Include(p => p.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.ProblemId == id);
            if (problem == null)
            {
                return NotFound();
            }
            ViewData["Tags"] = await _context.ProblemTags.Include(pt => pt.Tag).Where(pt => pt.ProblemId == id).Select(pt => pt.Tag).ToListAsync();
            return View(problem);
        }

        // GET: Problem/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login");
            ViewData["Tags"] = new SelectList(_context.Tags, "TagId", "Name");
            return View();
        }

        // POST: Problem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProblemId,Name,Description,DifficultyLevel,CreatedBy,Editorial,TestFile")] Problem problem, List<int> tags)
        {
            if (ModelState.IsValid)
            {
                _context.Add(problem);
                for(int i = 0; i<tags.Count; i++)
                {
                    var  tag = tags[i];
                    if(tag != -1)
                    {
                        var problemTag = new ProblemTag(){
                            TagId = tag,
                            ProblemId = problem.ProblemId,
                            Problem = problem,
                        };
                        var duplicate = await _context.ProblemTags.FirstOrDefaultAsync(u => u.TagId == tag && u.ProblemId == problemTag.ProblemId);
                        if(duplicate != null)
                        {
                            _context.ProblemTags.Remove(duplicate);
                        }
                        _context.ProblemTags.Add(problemTag);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<int> selectedTags = new List<int>();
            foreach(var tag in tags)
            {
                if(tag != -1)
                {
                    selectedTags.Add(tag);
                }
            }
            ViewData["SelectedTags"] = selectedTags;
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", problem.CreatedBy);
            ViewData["Tags"] = new SelectList(_context.Tags, "TagId", "Name");
            return View(problem);
        }

        // GET: Problem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problems.FindAsync(id);
            if (problem == null)
            {
                return NotFound();
            }
            ViewData["SelectedTags"] = await _context.ProblemTags.Where(pt => pt.ProblemId == id).Select(pt => pt.TagId).ToListAsync();
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", problem.CreatedBy);
            ViewData["Tags"] = new SelectList(_context.Tags, "TagId", "Name");
            return View(problem);
        }

        // POST: Problem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProblemId,Name,Description,DifficultyLevel,CreatedBy,Editorial,TestFile")] Problem problem, List<int>tags)
        {
            if (id != problem.ProblemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTags = await _context.ProblemTags.Where(pt => pt.ProblemId == id).ToListAsync();
                    foreach(var tag in oldTags)
                    {
                        _context.ProblemTags.Remove(tag);
                    }
                    for(int i = 0; i<tags.Count; i++)
                    {
                        var  tag = tags[i];
                        if(tag != -1)
                        {
                            var problemTag = new ProblemTag(){
                                TagId = tag,
                                ProblemId = problem.ProblemId,
                                Problem = problem,
                            };
                            var duplicate = await _context.ProblemTags.FirstOrDefaultAsync(u => u.TagId == tag && u.ProblemId == problemTag.ProblemId);
                            if(duplicate != null)
                            {
                                _context.ProblemTags.Remove(duplicate);
                            }
                            _context.ProblemTags.Add(problemTag);
                        }
                    }
                    _context.Update(problem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProblemExists(problem.ProblemId))
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
            List<int> selectedTags = new List<int>();
            foreach(var tag in tags)
            {
                if(tag != -1)
                {
                    selectedTags.Add(tag);
                }
            }
            ViewData["SelectedTags"] = selectedTags;
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", problem.CreatedBy);
            ViewData["Tags"] = new SelectList(_context.Tags, "TagId", "Name");
            return View(problem);
        }

        // GET: Problem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problems
                .Include(p => p.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.ProblemId == id);
            if (problem == null)
            {
                return NotFound();
            }
            return View(problem);
        }

        // POST: Problem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var problem = await _context.Problems.FindAsync(id);
            if (problem != null)
            {
                var tags = await _context.ProblemTags.Where(p => p.ProblemId == id).ToListAsync();
                foreach (var tag in tags)
                {
                    _context.ProblemTags.Remove(tag);
                }

                var contests = await _context.ContestProblems.Where(p => p.ProblemId == id).ToListAsync();
                foreach (var contest in contests)
                {
                    _context.ContestProblems.Remove(contest);
                }
                _context.Problems.Remove(problem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProblemExists(int id)
        {
            return _context.Problems.Any(e => e.ProblemId == id);
        }

        public async Task<IActionResult> Query2(int? TagId)
        {
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "Name", TagId);
            if(TagId == null)
            {
                return View(new List<Problem>());
            }
            var ProblemsWithTagId = _context.Problems.FromSqlRaw(@$"
                                                    SELECT * 
                                                    FROM dbo.Problems
                                                    WHERE ProblemId IN 
                                                        (SELECT ProblemId
                                                        FROM dbo.ProblemTags
                                                        WHERE TagId = {TagId})");
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "Name", TagId);
            ViewData["Tag"] = await _context.Tags.FindAsync(TagId);
            return View(await ProblemsWithTagId.ToListAsync());
        }

        public async Task<IActionResult> Query3(int? ContestId)
        {
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name", ContestId);
            if(ContestId == null)
            {
                return View(new List<Problem>());
            }
            var SolvedProblems = _context.Problems.FromSqlRaw(@$"
                                                    SELECT * 
                                                    FROM dbo.Problems
                                                    WHERE ProblemId IN 
                                                        (SELECT ProblemId
                                                        FROM dbo.Submissions
                                                        WHERE ContestId = {ContestId} AND Verdict = 'AC')");
            ViewData["ContestId"] = new SelectList(_context.Contests, "ContestId", "Name", ContestId);
            ViewData["Contest"] = await _context.Contests.FindAsync(ContestId);
            return View(await SolvedProblems.ToListAsync());
        }
    }
}
