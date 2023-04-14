﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<IdentityRole> _userManager;

        public TasksController(ApplicationContext context, UserManager<IdentityRole> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Tasks.Include(t => t.Project);
            return View(await applicationContext.ToListAsync());
        }

        public async Task<IActionResult> AssignTask(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            return View();

            if(task == null)
            {
                return NotFound();
            }

            string devName = "Developer";
            var developers = await _userManager.GetUsersInRoleAsync("Developer");


            var developerSelectList = developers.Select(d => new SelectListItem
            {
                Value = d.Id,
                Text = d.NormalizedName
            });

            var viewModel = new AssignTaskViewModel
            {
                Task = task,
                Developers = (List<ApplicationUser>)developerSelectList
            };


            return View(viewModel);

        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create(int? Id)
        {

            if (Id == null)
            {
                return BadRequest();
            }

            ApplicationProject project = _context.Projects.FirstOrDefault(p => p.Id == Id);
            ViewBag.ProjectId = project.Id;

            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, [Bind("Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask tasks)
        {
            ApplicationProject project = _context.Projects.FirstOrDefault(p => p.Id == Id);
            ViewBag.ProjectId = project.Id;

            tasks.Project = project;
            tasks.ApplicationProjectId = project.Id;

            if (ModelState.IsValid)
            {
                project.Tasks.Add(tasks);
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects");
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return RedirectToAction("Index", "Projects");
        }


        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,RequiredHours,Completed,Priority,ApplicationProjectId")] ApplicationTask tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Title", tasks.ApplicationProjectId);
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationContext.Tasks'  is null.");
            }
            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks != null)
            {
                _context.Tasks.Remove(tasks);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
          return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
