using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SISGUILLEN.Models;
using System.Diagnostics;

namespace SISGUILLEN.Controllers
{
    public class HomeController : Controller
    {
        private readonly BD_GUILLENContext _context;

        public HomeController(BD_GUILLENContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuariosActivos = await _context.Usuarios
                .Where(u => u.Estado == 1)
                .Include(u => u.Perfil)
                .Include(u => u.Cargo)
                .ToListAsync();

            return View(usuariosActivos);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Create()
        {
            ViewData["PerfilID"] = GetSelectListForPerfil();
            ViewData["CargoID"] = GetSelectListForCargo();
            return View();
        }

        private SelectList GetSelectListForPerfil()
        {
            return new SelectList(_context.Perfiles, "PerfilId", "Nombre");
        }

        private SelectList GetSelectListForCargo()
        {
            return new SelectList(_context.Cargos, "CargoId", "Nombre");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nombre,Apellido,Edad,FechaNacimiento,Direccion,PerfilId,CargoId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el usuario ya existe
                    if (_context.Usuarios.Any(u => u.Nombre == usuario.Nombre && u.Apellido == usuario.Apellido && u.Edad == usuario.Edad && u.Direccion == usuario.Direccion && u.FechaNacimiento == usuario.FechaNacimiento))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un usuario con los mismos detalles.");
                        return View(usuario);
                    }

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, usuario);
                }
            }
            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .Include(x => x.Perfil)
                .Include(x => x.Cargo)
                .FirstOrDefaultAsync(x => x.UsuarioId == id);

            if (usuarios == null)
            {
                return NotFound();
            }

            ConfigureViewDataForEdit(usuarios);
            return View(usuarios);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Apellido,Edad,FechaNacimiento,Direccion,PerfilId,CargoId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);

                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Apellido = usuario.Apellido;
                    usuarioExistente.Edad = usuario.Edad;
                    usuarioExistente.FechaNacimiento = usuario.FechaNacimiento;
                    usuarioExistente.Direccion = usuario.Direccion;
                    usuarioExistente.PerfilId = usuario.PerfilId;
                    usuarioExistente.CargoId = usuario.CargoId;

                    _context.Update(usuarioExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    HandleUniqueConstraintViolation(ex, usuario);
                    return View(usuario);
                }
            }

            ConfigureViewDataForEdit(usuario);
            return View(usuario);
        }

        private SelectList GetSelectListForPerfil(int? selectedValue = null)
        {
            return new SelectList(_context.Perfiles, "PerfilId", "Nombre", selectedValue);
        }

        private SelectList GetSelectListForCargo(int? selectedValue = null)
        {
            return new SelectList(_context.Cargos, "CargoId", "Nombre", selectedValue);
        }

        private void ConfigureViewDataForEdit(Usuario usuarios)
        {
            ViewData["PerfilId"] = GetSelectListForPerfil(usuarios.PerfilId);
            ViewData["CargoId"] = GetSelectListForCargo(usuarios.CargoId);
        }

        private bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex?.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        private void HandleUniqueConstraintViolation(DbUpdateException ex, Usuario usuario)
        {
            var uniqueColumns = new[] { "Nombre", "Apellido", "Edad", "Direccion", "FechaNacimiento" };

            // Agregar un error personalizado al ModelState
            foreach (var columnName in uniqueColumns)
            {
                ModelState.AddModelError(columnName, "Ya existe un usuario con los mismos detalles.");
            }

            ConfigureViewDataForEdit(usuario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .Include(x => x.Perfil)
                .Include(x => x.Cargo)
                .FirstOrDefaultAsync(x => x.UsuarioId == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        // POST: UsuariosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return Problem("El registro no fue encontrado.");
            }

            // Cambiar el estado en lugar de eliminar físicamente
            if (usuario.Estado == 1)
            {
                usuario.Estado = 0;
            } else
            {
                usuario.Estado = 1;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
