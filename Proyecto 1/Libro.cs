using System;

namespace Proyecto_1
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public int Anio { get; set; }
        public string Genero { get; set; } = "";

        public override string ToString()
        {
            return Id + " | " + Titulo + " | " + Autor + " | " + Anio + " | " + Genero;
        }
    }
}