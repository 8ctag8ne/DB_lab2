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
    public class UserController : Controller
    {
        private readonly DbLab2Context _context;

        public UserController(DbLab2Context context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Login,PasswordHash,Rating,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Login,PasswordHash,Rating,IsAdmin")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Query1(int? GroupId)
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", GroupId);
            if(GroupId == null)
            {
                return View(new List<User>());
            }
            var GroupAdmins = _context.Users.FromSqlRaw(@$"
                                                    SELECT * 
                                                    FROM dbo.Users
                                                    WHERE IsAdmin = 1 AND UserId IN 
                                                        (SELECT UserId
                                                        FROM dbo.GroupUsers
                                                        WHERE GroupId = {GroupId})");
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", GroupId);
            ViewData["Group"] = await _context.Groups.FindAsync(GroupId);
            return View(await GroupAdmins.ToListAsync());
        }

        public async Task<IActionResult> Query4(int? ProblemId)
        {
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name", ProblemId);
            if(ProblemId == null)
            {
                return View(new List<User>());
            }
            var SubmissiveUsers = _context.Users.FromSqlRaw(@$"
                                                    SELECT *
                                                    FROM dbo.Users
                                                    WHERE UserId IN 
                                                        (SELECT DISTINCT SubmittedBy
                                                        FROM dbo.Submissions
                                                        WHERE ProblemId = {ProblemId})");
            ViewData["ProblemId"] = new SelectList(_context.Problems, "ProblemId", "Name", ProblemId);
            ViewData["Problem"] = await _context.Problems.FindAsync(ProblemId);
            return View(await SubmissiveUsers.ToListAsync());
        }

        public async Task<IActionResult> Query6(int? UserId)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            if(UserId == null)
            {
                return View(new List<User>());
            }
            var SubmissiveUsers = _context.Users.FromSqlRaw(@$"
                                                            SELECT *
                                                            FROM Users u
                                                            WHERE u.UserId IN (
                                                                SELECT gu.UserId
                                                                FROM GroupUsers gu
                                                                WHERE gu.GroupId IN (
                                                                    SELECT GroupId
                                                                    FROM GroupUsers
                                                                    WHERE UserId = ${UserId}
                                                                )
                                                                GROUP BY gu.UserId
                                                                HAVING COUNT(DISTINCT gu.GroupId) >= (
                                                                    SELECT COUNT(DISTINCT GroupId)
                                                                    FROM GroupUsers
                                                                    WHERE UserId = ${UserId}
                                                                )
                                                            )");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            ViewData["User"] = await _context.Users.FindAsync(UserId);
            return View(await SubmissiveUsers.ToListAsync());
        }

        public async Task<IActionResult> Query7(int? UserId)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            if(UserId == null)
            {
                return View(new List<User>());
            }
            var SubmissiveUsers = _context.Users.FromSqlRaw(@$"
                                                            SELECT *
                                                            FROM Users u
                                                            WHERE NOT EXISTS (
                                                                SELECT *
                                                                FROM GroupUsers gu
                                                                WHERE gu.UserId = u.UserId
                                                                AND gu.GroupId NOT IN (
                                                                    SELECT GroupId
                                                                    FROM GroupUsers
                                                                    WHERE UserId = ${UserId}
                                                                )
                                                            )");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            ViewData["User"] = await _context.Users.FindAsync(UserId);
            return View(await SubmissiveUsers.ToListAsync());
        }

        public async Task<IActionResult> Query8(int? UserId)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            if(UserId == null)
            {
                return View(new List<User>());
            }
            var SubmissiveUsers = _context.Users.FromSqlRaw(@$"
                                                            SELECT *
                                                            FROM Users u
                                                            WHERE NOT EXISTS (
                                                                SELECT *
                                                                FROM GroupUsers gu
                                                                WHERE gu.UserId = u.UserId
                                                                AND gu.GroupId NOT IN (
                                                                    SELECT GroupId
                                                                    FROM GroupUsers
                                                                    WHERE UserId = ${UserId}
                                                                )
                                                            )
                                                            AND
                                                            NOT EXISTS (
                                                                SELECT *
                                                                FROM GroupUsers gu
                                                                WHERE gu.UserId = ${UserId}
                                                                AND gu.GroupId NOT IN (
                                                                    SELECT GroupId
                                                                    FROM GroupUsers
                                                                    WHERE UserId = u.UserId
                                                                )
                                                            )");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login", UserId);
            ViewData["User"] = await _context.Users.FindAsync(UserId);
            return View(await SubmissiveUsers.ToListAsync());
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
