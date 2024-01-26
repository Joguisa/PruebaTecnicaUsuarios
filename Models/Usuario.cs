using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISGUILLEN.Models
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public int Edad { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = null!;
        public int Estado { get; set; }
        [Required(ErrorMessage = "Seleccione un perfil")]
        public int? PerfilId { get; set; }
        [Required(ErrorMessage = "Seleccione un cargo")]
        public int? CargoId { get; set; }

        public virtual Cargo? Cargo { get; set; }
        public virtual Perfile? Perfil { get; set; }
    }
}
