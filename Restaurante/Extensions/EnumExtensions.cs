using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Restaurante.Extensions
{
    public static class EnumExtensions
    {
        // Devuelve el texto de [Display(Name = "...")] o el nombre del valor
        public static string NombreVisible(this Enum valor)
        {
            var miembro = valor.GetType().GetMember(valor.ToString()).FirstOrDefault();
            return miembro?.GetCustomAttribute<DisplayAttribute>()?.GetName()
                   ?? valor.ToString();
        }
    }
}
