// =========================
// Forma.cs (parte do componente Model)
// =========================

using System;
using System.Numerics;

namespace ByteSquad
{
    namespace Model
    {
        // Interface que representa uma forma genérica
        public interface IForma
        {
            Vector2 PontoBasilar { get; set; }
            int Largura { get; set; }
            int Altura { get; set; }
            FormasPossiveis TipoForma { get; }
            DateTime DataDeteccao { get; set; }

            string ToString();
            IForma Clone();
        }

        // Enum que representa os tipos possíveis de formas
        public enum FormasPossiveis
        {
            Circulo, Quadrado, Retangulo, Triangulo, Desconhecida
        }

        // -------------------------
        // Implementações concretas
        // -------------------------

        public class FormaCirculo : IForma
        {
            // Propriedades da forma
            // Ponto basilar, largura e altura da forma
            public Vector2 PontoBasilar { get; set; }
            public int Largura { get; set; }
            public int Altura { get; set; }
            public DateTime DataDeteccao { get; set; }

            public FormasPossiveis TipoForma => FormasPossiveis.Circulo;

            public FormaCirculo() { }

            public FormaCirculo(int largura, int altura, Vector2 pontoBasilar)
            {
                Largura = largura;
                Altura = altura;
                PontoBasilar = pontoBasilar;
                DataDeteccao = DateTime.Now;
            }

            public IForma Clone() => new FormaCirculo(Largura, Altura, PontoBasilar)
            {
                DataDeteccao = this.DataDeteccao
            };

            public override string ToString()
            {
                return $"Círculo ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
            }
        }

        public class FormaQuadrado : IForma
        {
            public Vector2 PontoBasilar { get; set; }
            public int Largura { get; set; }
            public int Altura { get; set; }
            public DateTime DataDeteccao { get; set; }

            public FormasPossiveis TipoForma => FormasPossiveis.Quadrado;

            public FormaQuadrado() { }

            public FormaQuadrado(int largura, int altura, Vector2 pontoBasilar)
            {
                Largura = largura;
                Altura = altura;
                PontoBasilar = pontoBasilar;
                DataDeteccao = DateTime.Now;
            }

            public IForma Clone() => new FormaQuadrado(Largura, Altura, PontoBasilar)
            {
                DataDeteccao = this.DataDeteccao
            };

            public override string ToString()
            {
                return $"Quadrado ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
            }
        }

        public class FormaRetangulo : IForma
        {
            public Vector2 PontoBasilar { get; set; }
            public int Largura { get; set; }
            public int Altura { get; set; }
            public DateTime DataDeteccao { get; set; }

            public FormasPossiveis TipoForma => FormasPossiveis.Retangulo;

            public FormaRetangulo() { }

            public FormaRetangulo(int largura, int altura, Vector2 pontoBasilar)
            {
                Largura = largura;
                Altura = altura;
                PontoBasilar = pontoBasilar;
                DataDeteccao = DateTime.Now;
            }

            public IForma Clone() => new FormaRetangulo(Largura, Altura, PontoBasilar)
            {
                DataDeteccao = this.DataDeteccao
            };

            public override string ToString()
            {
                return $"Retângulo ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
            }
        }

        public class FormaTriangulo : IForma
        {
            public Vector2 PontoBasilar { get; set; }
            public int Largura { get; set; }
            public int Altura { get; set; }
            public DateTime DataDeteccao { get; set; }

            public FormasPossiveis TipoForma => FormasPossiveis.Triangulo;

            public FormaTriangulo() { }

            public FormaTriangulo(int largura, int altura, Vector2 pontoBasilar)
            {
                Largura = largura;
                Altura = altura;
                PontoBasilar = pontoBasilar;
                DataDeteccao = DateTime.Now;
            }

            public IForma Clone() => new FormaTriangulo(Largura, Altura, PontoBasilar)
            {
                DataDeteccao = this.DataDeteccao
            };

            public override string ToString()
            {
                return $"Triângulo ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
            }
        }

        public class FormaDesconhecida : IForma
        {
            public Vector2 PontoBasilar { get; set; }
            public int Largura { get; set; }
            public int Altura { get; set; }
            public DateTime DataDeteccao { get; set; }

            public FormasPossiveis TipoForma => FormasPossiveis.Desconhecida;

            public FormaDesconhecida() { }

            public FormaDesconhecida(int largura, int altura, Vector2 pontoBasilar)
            {
                Largura = largura;
                Altura = altura;
                PontoBasilar = pontoBasilar;
                DataDeteccao = DateTime.Now;
            }

            public IForma Clone() => new FormaDesconhecida(Largura, Altura, PontoBasilar)
            {
                DataDeteccao = this.DataDeteccao
            };

            public override string ToString()
            {
                return $"Forma Desconhecida em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
            }
        }
    }
}
