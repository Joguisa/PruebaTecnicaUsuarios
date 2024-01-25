using System;
using System.Collections.Generic;

namespace SISGUILLEN.Models
{
    public partial class Cargo
    {
        public Cargo()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int CargoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
