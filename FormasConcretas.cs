// =========================
// FormasConcretas.cs (parte do componente Model)
// =========================
using System.Numerics;
using System;

namespace ByteSquad.Model
{
    public class FormaCirculo : FormaBase
    {
        public override FormasPossiveis TipoForma => FormasPossiveis.Circulo;

        public FormaCirculo() { }

        public FormaCirculo(int largura, int altura, Vector2 pontoBasilar)
        {
            Largura = largura;
            Altura = altura;
            PontoBasilar = pontoBasilar;
            DataDeteccao = DateTime.Now;
        }

        public override IForma Clone() => new FormaCirculo(Largura, Altura, PontoBasilar)
        {
            DataDeteccao = this.DataDeteccao
        };
    }
    public class FormaQuadrado : FormaBase
    {
        public override FormasPossiveis TipoForma => FormasPossiveis.Quadrado;

        public FormaQuadrado() { }

        public FormaQuadrado(int largura, int altura, Vector2 pontoBasilar)
        {
            Largura = largura;
            Altura = altura;
            PontoBasilar = pontoBasilar;
            DataDeteccao = DateTime.Now;
        }

        public override IForma Clone() => new FormaQuadrado(Largura, Altura, PontoBasilar)
        {
            DataDeteccao = this.DataDeteccao
        };
    }

    public class FormaRetangulo : FormaBase
    {
        public override FormasPossiveis TipoForma => FormasPossiveis.Retangulo;

        public FormaRetangulo() { }

        public FormaRetangulo(int largura, int altura, Vector2 pontoBasilar)
        {
            Largura = largura;
            Altura = altura;
            PontoBasilar = pontoBasilar;
            DataDeteccao = DateTime.Now;
        }

        public override IForma Clone() => new FormaRetangulo(Largura, Altura, PontoBasilar)
        {
            DataDeteccao = this.DataDeteccao
        };
    }

    public class FormaTriangulo : FormaBase
    {
        public override FormasPossiveis TipoForma => FormasPossiveis.Triangulo;

        public FormaTriangulo() { }

        public FormaTriangulo(int largura, int altura, Vector2 pontoBasilar)
        {
            Largura = largura;
            Altura = altura;
            PontoBasilar = pontoBasilar;
            DataDeteccao = DateTime.Now;
        }

        public override IForma Clone() => new FormaTriangulo(Largura, Altura, PontoBasilar)
        {
            DataDeteccao = this.DataDeteccao
        };
    }

    public class FormaDesconhecida : FormaBase
    {
        public override FormasPossiveis TipoForma => FormasPossiveis.Desconhecida;

        public FormaDesconhecida() { }

        public FormaDesconhecida(int largura, int altura, Vector2 pontoBasilar)
        {
            Largura = largura;
            Altura = altura;
            PontoBasilar = pontoBasilar;
            DataDeteccao = DateTime.Now;
        }

        public override IForma Clone() => new FormaDesconhecida(Largura, Altura, PontoBasilar)
        {
            DataDeteccao = this.DataDeteccao
        };
    }
}
