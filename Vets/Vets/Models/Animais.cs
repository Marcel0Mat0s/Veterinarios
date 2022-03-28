namespace Vets.Models
{
    public class Animais
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Raca { get; set; }

        public string Especie { get; set; }

        public DateTime DataNascimento { get; set; }

        public double Peso { get; set; }

        public string Fotografia { get; set; }


        public Donos Dono { get; set; }

    }
}
