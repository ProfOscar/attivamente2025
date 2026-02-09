using System.ComponentModel.DataAnnotations;

namespace AttivaMente.Core.Models
{
    public class Ruolo
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50)] 
        public string Nome { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Id}: {Nome}";
        }
    }
}
