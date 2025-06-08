// =========================
// FormaBase.cs (parte do componente Model)
// =========================
using System;
using System.Numerics;
using System.Drawing;

namespace ByteSquad.Model
{
    // Classe base abstrata que define a estrutura comum para todas as formas geométricas.
    public abstract class FormaBase : IForma
    {
        public Vector2 PontoBasilar { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public DateTime DataDeteccao { get; set; }

        public abstract FormasPossiveis TipoForma { get; }

        public Color Cor { get; set; }

        public abstract IForma Clone();

        public override string ToString()
        {
            return $"{TipoForma} ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss} (Cor: {Cor})";
        }
    }
}