using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SISGUILLEN.Models;

namespace SISGUILLEN.Controllers
{
    public class CargoController : Controller
    {
        private readonly BD_GUILLENContext _context;

        public CargoController(BD_GUILLENContext context)
        {
            _context = context;
        }
        // GET: CargoController
        public async Task<IActionResult> Index()
        {
            if (_context.Cargos == null)
            {
                return Problem("Entity set 'BD_GUILLENContext.Cargos' is null.");
            }
            var cargos = await _context.Cargos.ToListAsync();
            return View(cargos);
        }

        // GET: CargoController/Create
        public ActionResult Create()
        {
            ViewData["CargoId"] = GetSelectListForCargo();
            return View();
        }

        private SelectList GetSelectListForCargo()
        {
            return new SelectList(_context.Cargos, "CargoId", "Nombre");
        }

        // POST: CargoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CargoId,Nombre,Descripcion")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Cargos.Any(p => p.Nombre == cargo.Nombre && p.Descripcion == cargo.Descripcion))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un cargo con el mismo nombre y descripción");
                        return View(cargo);
                    }

                    _context.Add(cargo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, cargo);
                    return View(cargo);
                }
            }
            return View(cargo);
        }

        // GET: CargoController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
            .FindAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: CargoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("CargoId,Nombre,Descripcion")] Cargo cargo)
        {
            if (id != cargo.CargoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cargoExistente = await _context.Cargos.FindAsync(id);

                    if (cargoExistente == null)
                    {
                        return NotFound();
                    }

                    // Verificar si el perfil está asignado a algún usuario
                    var perfilAsignado = await _context.Usuarios.AnyAsync(u => u.CargoId == id);

                    if (perfilAsignado)
                    {
                        // Mostrar mensaje de error o manejar según tus requisitos
                        ModelState.AddModelError(string.Empty, "El cargo está asignado a uno o más usuarios y no puede ser editado.");
                        ConfigureViewDataForEdit(cargo);
                        return View(cargo);
                    }

                    cargoExistente.Nombre = cargo.Nombre;
                    cargoExistente.Descripcion = cargo.Descripcion;

                    _context.Update(cargoExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, cargo);
                    return View(cargo);
                }
            }
            ConfigureViewDataForEdit(cargo);
            return View(cargo);
        }

        private SelectList GetSelectListForCargo(int? selectedValue = null)
        {
            return new SelectList(_context.Cargos, "CargoId", "Nombre", selectedValue);
        }
        private void ConfigureViewDataForEdit(Cargo cargo)
        {
            ViewData["CargoId"] = GetSelectListForCargo(cargo.CargoId);
        }
        private bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex?.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        private void HandleUniqueConstraintViolation(DbUpdateException ex, Cargo cargo)
        {
            var uniqueColumns = new[] { "Nombre", "Descripcion" };

            foreach (var columnName in uniqueColumns)
            {
                ModelState.AddModelError(columnName, "Ya existe un cargo con los mismos detalles.");
            }

            ConfigureViewDataForEdit(cargo);
        }

        // GET: CargoController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cargos == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(p => p.CargoId == id);

            return cargo == null ? NotFound() : View(cargo);
        }

        // POST: CargoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargo = await _context.Cargos.FindAsync(id);

            if (cargo == null)
            {
                return Problem("El registro no fue encontrado.");
            }

            // Verificar si hay usuarios vinculados a este cargo
            var usuariosConEsteCargo = await _context.Usuarios.AnyAsync(u => u.CargoId == id);

            if (usuariosConEsteCargo)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar el cargo porque está vinculado a uno o más usuarios.");
                return View(cargo);
            }

            _context.Cargos.Remove(cargo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
