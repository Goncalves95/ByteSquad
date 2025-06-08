// =========================
// IForma.cs (Interaface do componente Model)
// =========================
using System;
using System.Numerics;
using System.Drawing;

namespace ByteSquad.Model
{
    public interface IForma
    {
        Vector2 PontoBasilar { get; set; }
        int Largura { get; set; }
        int Altura { get; set; }
        DateTime DataDeteccao { get; set; }

        FormasPossiveis TipoForma { get; }

        Color Cor { get; set; } 

        IForma Clone();
        string ToString();
    }

    public enum FormasPossiveis
    {
        Circulo, Quadrado, Retangulo, Triangulo, Desconhecida
    }
}