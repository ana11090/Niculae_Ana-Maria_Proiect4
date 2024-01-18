﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Niculae_Ana_Maria_Proiect4.Data;
using Niculae_Ana_Maria_Proiect4.Models;
using Niculae_Ana_Maria_Proiect4.Models.View;
using Niculae_Ana_Maria_Proiect4.Models;

namespace Niculae_Ana_Maria_Proiect4.Controllers
{
    public class ProiectsController : Controller
    {
        private readonly LibraryContext _context;

        public ProiectsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Proiects
        public async Task<IActionResult> Index()
        {
            var proiecteWithManagerNames = await _context.Proiecte
                .Include(p => p.ManagerProiect)
                .ToListAsync();

            // înlocuiți ManagerId cu numele managerului
            foreach (var proiect in proiecteWithManagerNames)
            {
                proiect.ManagerProiect = await _context.Manageri.FindAsync(proiect.ManagerId);
            }

            var proiecte = proiecteWithManagerNames.Select(p => new ProiectViewModel
            {
                ProiectId = p.ProiectId,
                Nume = p.Nume,
                Descriere = p.Descriere,
                DataIncepere = p.DataIncepere,
                DataFinalizare = p.DataFinalizare,
                Status = p.Status,
                ManagerId = p.ManagerId,
                ManagerNume = p.ManagerProiect.Nume,
                NumarSarcini = p.Sarcini.Count
            }).ToList();

            return View("Index", proiecte);
        }




        // GET: Proiects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Proiecte == null)
            {
                return NotFound();
            }

            var proiect = await _context.Proiecte
                .Include(p => p.ManagerProiect)
                .FirstOrDefaultAsync(m => m.ProiectId == id);
            if (proiect == null)
            {
                return NotFound();
            }

            // Fetch the manager's name based on the ManagerId
            var managerName = proiect.ManagerProiect != null ? proiect.ManagerProiect.Nume : "N/A"; // Display "N/A" if no manager is associated

            // Create a view model or use ViewBag to pass the manager's name to the view
            ViewBag.ManagerName = managerName;

            return View(proiect);
        }


        // GET: Proiects/Create 
        public IActionResult Create()
        {
            // Folosește "Nume" în loc de "ManagerId" pentru a afișa numele managerului
            ViewBag.Manageri = new SelectList(_context.Manageri, "ManagerId", "Nume");
            return View();
        }



        // POST: Proiects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProiectId,Nume,Descriere,DataIncepere,DataFinalizare,Status,ManagerId")] Proiect proiect)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proiect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var key in ModelState.Keys)
            {
                var modelStateEntry = ModelState[key];
                foreach (var error in modelStateEntry.Errors)
                {
                    // Log or print the error messages to diagnose the issue
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Folosește "Nume" și aici pentru a menține selecția corectă în caz de eroare
            ViewData["ManagerId"] = new SelectList(_context.Manageri, "ManagerId", "Nume", proiect.ManagerId);
            return View(proiect);
        }



        // GET: Proiects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Proiecte == null)
            {
                return NotFound();
            }

            var proiect = await _context.Proiecte
                .Include(p => p.ManagerProiect)
                .FirstOrDefaultAsync(m => m.ProiectId == id);
            if (proiect == null)
            {
                return NotFound();
            }

            // Aici încărcați numele managerilor în ViewBag
            ViewBag.Manageri = new SelectList(_context.Manageri, "ManagerId", "Nume");

            return View(proiect);
        }

        // POST: Proiects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProiectId,Nume,Descriere,DataIncepere,DataFinalizare,Status,ManagerId")] Proiect proiect)
        {
            if (id != proiect.ProiectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Atribuiți managerul corect modelului Proiect
                proiect.ManagerProiect = _context.Manageri.FirstOrDefault(m => m.ManagerId == proiect.ManagerId);

                try
                {
                    _context.Update(proiect);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProiectExists(proiect.ProiectId))
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

            // Log ModelState errors for debugging
            foreach (var key in ModelState.Keys)
            {
                var modelStateEntry = ModelState[key];
                foreach (var error in modelStateEntry.Errors)
                {
                    // Log or print the error messages to diagnose the issue
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Aici încărcați numele managerilor în ViewBag
            ViewBag.Manageri = new SelectList(_context.Manageri, "ManagerId", "Nume");

            return View(proiect);
        }



        // GET: Proiects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Proiecte == null)
            {
                return NotFound();
            }

            var proiect = await _context.Proiecte
                .Include(p => p.ManagerProiect)
                .FirstOrDefaultAsync(m => m.ProiectId == id);
            if (proiect == null)
            {
                return NotFound();
            }

            // Load the manager's name into ViewBag
            ViewBag.Manageri = new SelectList(_context.Manageri, "ManagerId", "Nume");

            return View(proiect);
        }

        // POST: Proiects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Proiecte == null)
            {
                return Problem("Entity set 'LibraryContext.Proiecte' is null.");
            }
            var proiect = await _context.Proiecte.FindAsync(id);
            if (proiect != null)
            {
                _context.Proiecte.Remove(proiect);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProiectExists(int id)
        {
          return (_context.Proiecte?.Any(e => e.ProiectId == id)).GetValueOrDefault();
        }
    }
}
