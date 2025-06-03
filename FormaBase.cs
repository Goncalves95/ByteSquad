// =========================
// FormaBase.cs
// =========================
using System;
using System.Numerics;

namespace ByteSquad.Model
{
    public abstract class FormaBase : IForma
    {
        public Vector2 PontoBasilar { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public DateTime DataDeteccao { get; set; }

        public abstract FormasPossiveis TipoForma { get; }

        public abstract IForma Clone();

        public override string ToString()
        {
            return $"{TipoForma} ({Largura}x{Altura}) em {PontoBasilar} às {DataDeteccao:HH:mm:ss}";
        }
    }
}