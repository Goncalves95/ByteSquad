// =========================
// DetectorDeFormas.cs (parte do componente Model)
// =========================

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;

namespace ByteSquad
{
    namespace Model
    {
        // Responsável por analisar uma imagem e detectar a melhor forma geométrica presente.
        public class DetectorDeFormas : IDetectorDeFormas
        {
            public ResultadoDeteccao Detectar(Bitmap imagemOriginal)
            {
                Bitmap imagem = (Bitmap)imagemOriginal.Clone();

                // 1. Converter para tons de cinza
                Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayImage = grayFilter.Apply(imagem);

                // 2. Suavizar imagem
                new Median().ApplyInPlace(grayImage);

                // 3. Binarizar imagem
                new Threshold(120).ApplyInPlace(grayImage);
                new Invert().ApplyInPlace(grayImage);

                grayImage.Save("debug_binarizada.bmp");

                // 4. Detectar blobs
                BlobCounter blobCounter = new BlobCounter
                {
                    MinWidth = 50,
                    MinHeight = 50,
                    FilterBlobs = true,
                    ObjectsOrder = ObjectsOrder.Size
                };

                blobCounter.ProcessImage(grayImage);
                Blob[] blobs = blobCounter.GetObjectsInformation();

                Graphics g = Graphics.FromImage(imagem);
                Pen pen = new Pen(Color.Beige, 2);
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                IForma melhorForma = new FormaDesconhecida(0, 0, new Vector2(0, 0));
                int maiorArea = 0;

                // 5. Analisar cada blob detectado e identificar a forma geométrica
                foreach (Blob blob in blobs)
                {
                    List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                    List<System.Drawing.Point> pontos = new List<System.Drawing.Point>();
                    foreach (IntPoint p in edgePoints)
                        pontos.Add(new System.Drawing.Point(p.X, p.Y));

                    List<IntPoint> corners;
                    FormasPossiveis tipoDetectado = FormasPossiveis.Desconhecida;

                    if (shapeChecker.IsCircle(edgePoints))
                    {
                        tipoDetectado = FormasPossiveis.Circulo;
                    }
                    else if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                    {
                        if (corners.Count == 3)
                        {
                            tipoDetectado = FormasPossiveis.Triangulo;
                        }
                        else if (corners.Count == 4)
                        {
                            var subtipo = shapeChecker.CheckPolygonSubType(corners);
                            if (subtipo == PolygonSubType.Square)
                                tipoDetectado = FormasPossiveis.Quadrado;
                            else if (subtipo == PolygonSubType.Rectangle)
                                tipoDetectado = FormasPossiveis.Retangulo;
                        }
                    }

                    if (tipoDetectado != FormasPossiveis.Desconhecida)
                    {
                        Rectangle rect = blob.Rectangle;
                        int largura = rect.Width;
                        int altura = rect.Height;
                        int area = largura * altura;
                        Vector2 centro = new Vector2(rect.X + largura / 2, rect.Y + altura / 2);

                        IForma forma = tipoDetectado switch
                        {
                            FormasPossiveis.Circulo => new FormaCirculo(largura, altura, centro),
                            FormasPossiveis.Triangulo => new FormaTriangulo(largura, altura, centro),
                            FormasPossiveis.Quadrado => new FormaQuadrado(largura, altura, centro),
                            FormasPossiveis.Retangulo => new FormaRetangulo(largura, altura, centro),
                            _ => null
                        };

                        if (forma != null)
                        {
                            // Extrair região da imagem original (com cor)

                            Bitmap roi = imagemOriginal.Clone(rect, imagemOriginal.PixelFormat);

                            // Calcular e aplicar a cor média
                            Color corMedia = CalcularCorMedia(roi);
                            forma.Cor = corMedia;

                            string nomeCor = ObterNomeCorAproximada(corMedia);
                            Console.WriteLine($"Cor detetada: R={corMedia.R}, G={corMedia.G}, B={corMedia.B} → {nomeCor}");
                        }

                        if (forma != null && area > maiorArea)
                        {
                            maiorArea = area;
                            melhorForma = forma;
                        }

                        if (pontos.Count >= 3)
                            g.DrawPolygon(pen, pontos.ToArray());
                    }
                }

                g.Dispose();

                // Se não foi encontrada nenhuma forma válida, retorna uma forma desconhecida
                if (melhorForma is FormaDesconhecida)
                    melhorForma = new FormaDesconhecida(100, 100, new Vector2(50, 50));

                // Define a data de deteção da forma
                melhorForma.DataDeteccao = DateTime.Now;

                // Desenha o contorno da melhor forma detectada
                return new ResultadoDeteccao
                {
                    FormaDetectada = melhorForma,
                    ImagemComContorno = imagem
                };
            }

            private Color CalcularCorMedia(Bitmap imagem)
            {
                long totalR = 0, totalG = 0, totalB = 0;
                int count = 0;

                for (int y = 0; y < imagem.Height; y++)
                {
                    for (int x = 0; x < imagem.Width; x++)
                    {
                        Color pixel = imagem.GetPixel(x, y);
                        totalR += pixel.R;
                        totalG += pixel.G;
                        totalB += pixel.B;
                        count++;
                    }
                }

                if (count == 0) return Color.Black;

                return Color.FromArgb((int)(totalR / count), (int)(totalG / count), (int)(totalB / count));
            }
            
            private string ObterNomeCorAproximada(Color cor)
            {
                // Lista de cores base com nome e valores RGB
                var coresBase = new Dictionary<string, Color>
                {
                    { "Vermelho", Color.FromArgb(255, 0, 0) },
                    { "Verde", Color.FromArgb(0, 255, 0) },
                    { "Azul", Color.FromArgb(0, 0, 255) },
                    { "Amarelo", Color.FromArgb(255, 255, 0) },
                    { "Ciano", Color.FromArgb(0, 255, 255) },
                    { "Roxo", Color.FromArgb(128, 0, 128) },
                    { "Laranja", Color.FromArgb(255, 165, 0) },
                    { "Rosa", Color.FromArgb(255, 192, 203) },
                    { "Branco", Color.FromArgb(255, 255, 255) },
                    { "Preto", Color.FromArgb(0, 0, 0) },
                    { "Cinza", Color.FromArgb(128, 128, 128) },
                    { "Azul escuro", Color.FromArgb(0, 33, 81) }
                };

                string corMaisProxima = "Cor indefinida";
                double menorDistancia = double.MaxValue;

                foreach (var par in coresBase)
                {
                    
                    Color baseColor = par.Value;

                    double distancia = Math.Sqrt(
                    Math.Pow(cor.R - baseColor.R, 2) +
                    Math.Pow(cor.G - baseColor.G, 2) +
                    Math.Pow(cor.B - baseColor.B, 2)
                );

                    if (distancia < menorDistancia)
                    {
                        menorDistancia = distancia;
                        corMaisProxima = par.Key;
                    }
                }

                return corMaisProxima;
            }


        }

        // Resultado da detecção de formas, contendo a forma detectada e a imagem com o contorno desenhado
        public class ResultadoDeteccao
        {
            public IForma FormaDetectada { get; set; }
            public Bitmap ImagemComContorno { get; set; }
        }
    }
}
