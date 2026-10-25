using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingresa el nombre.")]
        [StringLength(100, ErrorMessage = "El nombre admite hasta 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el apellido.")]
        [StringLength(100, ErrorMessage = "El apellido admite hasta 100 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el teléfono.")]
        [RegularExpression(@"^[67]\d{7}$",
            ErrorMessage = "Ingresa 8 dígitos, comenzando con 6 o 7.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el correo.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [StringLength(150, ErrorMessage = "El correo admite hasta 150 caracteres.")]
        public string Correo { get; set; } = string.Empty;
    }
}