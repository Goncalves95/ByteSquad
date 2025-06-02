// =========================
// FormaFactory.cs 
// =========================

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Numerics;

// Classe estática responsável por converter uma linha de texto com informações
// de uma forma geométrica (exportada ou salva em ficheiro) numa instância concreta de Forma.
// Essa fábrica interpreta uma string formatada contendo:
// Tipo da forma (ex: Círculo, Quadrado, Retângulo...)
// Dimensões (largura x altura)
// Posição (x, y)
// Hora da deteção

namespace ByteSquad.Model
{
    public static class FormaFactory
    {
        // Método estático para criar uma forma a partir de uma string formatada
        public static IForma CriarFormaDeTexto(string linha)
        {
            try
            {
                var regex = new Regex(@"^(?<tipo>\w+)\s+\((?<w>\d+)x(?<h>\d+)\)\s+em\s+<(?<x>\d+)\s+(?<y>\d+)>\s+às\s+(?<hora>\d{2}:\d{2}:\d{2})$");
                var match = regex.Match(linha);
                if (!match.Success) return null;

                string tipoRaw = match.Groups["tipo"].Value;
                int largura = int.Parse(match.Groups["w"].Value);
                int altura = int.Parse(match.Groups["h"].Value);
                int x = int.Parse(match.Groups["x"].Value);
                int y = int.Parse(match.Groups["y"].Value);
                string hora = match.Groups["hora"].Value;

                DateTime timestamp = DateTime.Today.Add(TimeSpan.Parse(hora, CultureInfo.InvariantCulture));
                Vector2 ponto = new Vector2(x, y);

                string tipo = tipoRaw.ToLowerInvariant();

                return tipo switch
                {
                    "círculo" or "circulo" => new FormaCirculo(largura, altura, ponto) { DataDeteccao = timestamp },
                    "quadrado" => new FormaQuadrado(largura, altura, ponto) { DataDeteccao = timestamp },
                    "retângulo" or "retangulo" => new FormaRetangulo(largura, altura, ponto) { DataDeteccao = timestamp },
                    "triângulo" or "triangulo" => new FormaTriangulo(largura, altura, ponto) { DataDeteccao = timestamp },
                    "desconhecida" => new FormaDesconhecida(largura, altura, ponto) { DataDeteccao = timestamp },
                    _ => null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar forma de texto: {ex.Message}");
                return null;
            }
        }

    }
}
