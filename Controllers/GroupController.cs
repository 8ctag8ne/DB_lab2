using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DB_lab2;

namespace DB_lab2.Controllers
{
    public class GroupController : Controller
    {
        private readonly DbLab2Context _context;
        public GroupController(DbLab2Context context)
        {
            _context = context;
        }

        // GET: Group
        public async Task<IActionResult> Index()
        {
            var dbLab2Context = _context.Groups.Include(g => g.CreatedByNavigation);
            return View(await dbLab2Context.ToListAsync());
        }

        // GET: Group/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .Include(g => g.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }
            ViewData["Users"] = await _context.GroupUsers.Include(u => u.User).Where(u => u.GroupId == id).Select(u => u.User).ToListAsync();
            ViewData["Contests"] = await _context.GroupContests.Include(u => u.Contest).Where(u => u.GroupId == id).Select(u => u.Contest).ToListAsync();

            return View(@group);
        }

       // GET: Group/Create
        public IActionResult Create()
        {
            ViewData["Users"] = new SelectList(_context.Users, "UserId", "Login");
            ViewData["Contests"] = new SelectList(_context.Contests, "ContestId", "Name");
            return View();
        }

        // POST: Group/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,Name,Description,CreatedBy")]Group @group, List<int> users, List<int>contests)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@group);
                await _context.SaveChangesAsync();

                for(int i = 0; i<users.Count; i++)
                {
                    var  user = users[i];
                    if(user != -1)
                    {
                        var groupUser = new GroupUser(){
                            UserId = user,
                            GroupId = group.GroupId,
                            IsAdmin = false,
                        };
                        var duplicate = await _context.GroupUsers.FirstOrDefaultAsync(u => u.UserId == user && u.GroupId == groupUser.GroupId);
                        if(duplicate != null)
                        {
                            _context.GroupUsers.Remove(duplicate);
                        }
                        _context.GroupUsers.Add(groupUser);
                    }
                }

                for(int i = 0; i<contests.Count; i++)
                {
                    var  contest = contests[i];
                    if(contest != -1)
                    {
                        var groupContest = new GroupContest(){
                            ContestId = contest,
                            GroupId = group.GroupId,
                        };
                        var duplicate = await _context.GroupContests.FirstOrDefaultAsync(u => u.ContestId == contest && u.GroupId == groupContest.GroupId);
                        if(duplicate != null)
                        {
                            _context.GroupContests.Remove(duplicate);
                        }
                        _context.GroupContests.Add(groupContest);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<int> selectedUsers = new List<int>();
            foreach(var user in users)
            {
                if(user != -1)
                {
                    selectedUsers.Add(user);
                }
            }
            List<int> selectedContests = new List<int>();
            foreach(var contest in contests)
            {
                if(contest != -1)
                {
                    selectedContests.Add(contest);
                }
            }
            ViewData["SelectedContests"] = selectedContests;
            ViewData["SelectedUsers"] = selectedUsers;
            ViewData["Users"] = new SelectList(_context.Users, "UserId", "Login");
            ViewData["Contests"] = new SelectList(_context.Contests, "ContestId", "Name");
            return View(@group);
        }

        // GET: Group/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            var selectedUsers = await _context.GroupUsers.Where(gu => gu.GroupId == id).Select(gu => gu.UserId).ToListAsync();
            var selectedContests = await _context.GroupContests.Where(gu => gu.GroupId == id).Select(gu => gu.ContestId).ToListAsync();
            ViewData["SelectedUsers"] = selectedUsers;
            ViewData["SelectedContests"] = selectedContests;
            ViewData["Users"] = new SelectList(_context.Users, "UserId", "Login", @group.CreatedBy);
            ViewData["Contests"] = new SelectList(_context.Contests, "ContestId", "Name");
            return View(@group);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,Name,Description,CreatedBy")] Group @group, List<int> users, List<int>contests)
        {
            if (id != @group.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldUsers = await _context.GroupUsers.Where(gu => gu.GroupId == id).ToListAsync();
                    foreach(var user in oldUsers)
                    {
                        _context.GroupUsers.Remove(user);
                    }
                    var oldContests = await _context.GroupContests.Where(gu => gu.GroupId == id).ToListAsync();
                    foreach(var contest in oldContests)
                    {
                        _context.GroupContests.Remove(contest);
                    }
                    for(int i = 0; i<users.Count; i++)
                    {
                        var  user = users[i];
                        if(user != -1)
                        {
                            var groupUser = new GroupUser(){
                                UserId = user,
                                GroupId = group.GroupId,
                                IsAdmin = false,
                            };
                            var duplicate = await _context.GroupUsers.FirstOrDefaultAsync(u => u.UserId == user && u.GroupId == groupUser.GroupId);
                            if(duplicate != null)
                            {
                                _context.GroupUsers.Remove(duplicate);
                            }
                            _context.GroupUsers.Add(groupUser);
                        }
                    }
                    for(int i = 0; i<contests.Count; i++)
                    {
                        var  contest = contests[i];
                        if(contest != -1)
                        {
                            var groupContest = new GroupContest(){
                                ContestId = contest,
                                GroupId = group.GroupId,
                            };
                            var duplicate = await _context.GroupContests.FirstOrDefaultAsync(u => u.ContestId == contest && u.GroupId == groupContest.GroupId);
                            if(duplicate != null)
                            {
                                _context.GroupContests.Remove(duplicate);
                            }
                            _context.GroupContests.Add(groupContest);
                        }
                    }
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.GroupId))
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
            List<int> selectedUsers = new List<int>();
            foreach(var user in users)
            {
                if(user != -1)
                {
                    selectedUsers.Add(user);
                }
            }
            List<int> selectedContests = new List<int>();
            foreach(var contest in contests)
            {
                if(contest != -1)
                {
                    selectedContests.Add(contest);
                }
            }
            ViewData["SelectedContests"] = selectedContests;
            ViewData["SelectedUsers"] = selectedUsers;
            ViewData["Users"] = new SelectList(_context.Users, "UserId", "Login", @group.CreatedBy);
            ViewData["Contests"] = new SelectList(_context.Contests, "ContestId", "Name");
            return View(@group);
        }

        // GET: Group/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .Include(g => g.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                var users = await _context.GroupUsers.Where(ug => ug.GroupId == id).ToListAsync();
                foreach(var user in users)
                {
                    _context.GroupUsers.Remove(user);
                }
                _context.Groups.Remove(@group);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
