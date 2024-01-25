using System;
using System.Collections.Generic;

namespace SISGUILLEN.Models
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public int Edad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = null!;
        public int Estado { get; set; }
        public int? PerfilId { get; set; }
        public int? CargoId { get; set; }

        public virtual Cargo? Cargo { get; set; }
        public virtual Perfile? Perfil { get; set; }
    }
}
