using System.ComponentModel.DataAnnotations;

namespace Vets.Models
{
    /// <summary>
    /// modelo que interage com os dados dos veterinários
    /// </summary>
    public class Veterinarios
    {
        public Veterinarios()
        {
            ListaConsultas = new HashSet<Consultas>();
        }
        /// <summary>
        /// PK para cada um dos registos da tabela
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome do veterinario
        /// </summary>
        public string Nome { get; set; }
        [Display(Name = "Nº de cédulas profissional")]

        /// <summary>
        /// Nº da cedula profissional
        /// </summary>
        public string NumCedulaProf { get; set; }
        /// <summary>
        /// Nome do ficheiro que contém a fotografia do veterinario
        /// </summary>
        public String Fotografia { get; set; }
        /// <summary>
        /// Lista de consultas feitas pelo veterinario
        /// </summary>
        public ICollection<Consultas> ListaConsultas { get; set; }
    }
}
