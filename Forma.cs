using System;
using System.Numerics;

namespace ByteSquad
{   

    //Esta classe pertence ao componente Model
    public enum FormasPossiveis
    {
        Circulo, Quadrado, Retangulo, Triangulo, Estrela, Desconhecida
    }

    //Esta classe pertence ao componente Model
    public class Forma
    {
        public Vector2 PontoBasilar { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public FormasPossiveis TipoForma { get; set; }
        public DateTime DataDeteccao { get; set; }

        public Forma()
        {
            DataDeteccao = DateTime.Now;
        }
        // Construtor com parâmetros
        public override string ToString()
        {
            return $"{TipoForma} ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
        }

        //Método Clone para criar uma cópia da forma
        //Isso é útil para evitar referências partilhadas entre objetos
        public Forma Clone()
        {
            return new Forma
            {
                Altura = this.Altura,
                Largura = this.Largura,
                TipoForma = this.TipoForma,
                PontoBasilar = new Vector2(this.PontoBasilar.X, this.PontoBasilar.Y),
                DataDeteccao = this.DataDeteccao
            };
        }
    }
}
