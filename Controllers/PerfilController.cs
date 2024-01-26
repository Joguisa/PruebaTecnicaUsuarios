using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SISGUILLEN.Models;

namespace SISGUILLEN.Controllers
{
    public class PerfilController : Controller
    {
        private readonly BD_GUILLENContext _context;

        public PerfilController(BD_GUILLENContext context)
        {
            _context = context;
        }

        // GET: PerfilController
        public async Task<IActionResult> Index()
        {
            if (_context.Perfiles == null)
            {
                return Problem("Entity set 'BD_GUILLENContext.Perfiles' is null.");
            }
            var perfiles = await _context.Perfiles.ToListAsync();
            return View(perfiles);
        }

        // GET: PerfilController/Create
        public ActionResult Create()
        {
            ViewData["PerfilId"] = GetSelectListForPerfil();
            return View();
        }

        private SelectList GetSelectListForPerfil()
        {
            return new SelectList(_context.Perfiles, "PerfilId", "Nombre");
        }

        // POST: PerfilController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PerfilId,Nombre,Descripcion")] Perfile perfil)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Perfiles.Any(p => p.Nombre == perfil.Nombre && p.Descripcion == perfil.Descripcion))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un perfil con el mismo nombre y descripción");
                        return View(perfil);
                    }

                    _context.Add(perfil);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, perfil);
                    return View(perfil);
                }
            }
            return View(perfil);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfiles
            .FindAsync(id);

            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }


        // POST: PerfilController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int perfilId, [Bind("PerfilId,Nombre,Descripcion")] Perfile perfil)
        {
            if (perfilId != perfil.PerfilId)
            {
                return NotFound();                
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var perfilExistente = await _context.Perfiles.FindAsync(perfilId);

                    if (perfilExistente == null)
                    {
                        return NotFound();
                    }

                    // Verificar si el perfil está asignado a algún usuario
                    var perfilAsignado = await _context.Usuarios.AnyAsync(u => u.PerfilId == perfilId);

                    if (perfilAsignado)
                    {
                        // Mostrar mensaje de error o manejar según tus requisitos
                        ModelState.AddModelError(string.Empty, "El perfil está asignado a uno o más usuarios y no puede ser editado.");
                        ConfigureViewDataForEdit(perfil);
                        return View(perfil);
                    }

                    perfilExistente.Nombre = perfil.Nombre;
                    perfilExistente.Descripcion = perfil.Descripcion;

                    _context.Update(perfilExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, perfil);
                    return View(perfil);
                }
            }
            ConfigureViewDataForEdit(perfil);
            return View(perfil);
        }

        private SelectList GetSelectListForPerfil(int? selectedValue = null)
        {
            return new SelectList(_context.Perfiles, "PerfilId", "Nombre", selectedValue);
        }
        private void ConfigureViewDataForEdit(Perfile perfil)
        {
            ViewData["PerfilId"] = GetSelectListForPerfil(perfil.PerfilId);
        }
        private bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex?.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        private void HandleUniqueConstraintViolation(DbUpdateException ex, Perfile perfil)
        {
            var uniqueColumns = new[] { "Nombre", "Descripcion"};

            foreach (var columnName in uniqueColumns)
            {
                ModelState.AddModelError(columnName, "Ya existe un perfil con los mismos detalles.");
            }

            ConfigureViewDataForEdit(perfil);
        }

        // GET: PerfilController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Perfiles == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(p => p.PerfilId == id);

            return perfil == null ? NotFound() : View(perfil);
        }

        // POST: PerfilController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);

            if (perfil == null)
            {
                return Problem("El registro no fue encontrado.");
            }

            // Verificar si hay usuarios vinculados a este perfil
            var usuariosConEstePerfil = await _context.Usuarios.AnyAsync(u => u.PerfilId == id);

            if (usuariosConEstePerfil)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar el perfil porque está vinculado a uno o más usuarios.");
                return View(perfil);
            }

            _context.Perfiles.Remove(perfil);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
