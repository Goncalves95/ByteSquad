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
        // Responsável por analisar uma imagem e detectar o tipo de forma geométrica presente.
        // Desenha os contornos automaticamente na imagem original.
        public class DetectorDeFormas : IDetectorDeFormas
        {
            // Processa a imagem, desenha os contornos e retorna a forma detectada e a imagem modificada.
            public ResultadoDeteccao Detectar(Bitmap imagemOriginal)
            {
                Bitmap imagem = (Bitmap)imagemOriginal.Clone();

                // 1. Converter para tons de cinza
                Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayImage = grayFilter.Apply(imagem);

                // 2. Suavizar imagem
                Median medianFilter = new Median();
                medianFilter.ApplyInPlace(grayImage);

                // 3. Binarizar imagem
                Threshold thresholdFilter = new Threshold(120);
                thresholdFilter.ApplyInPlace(grayImage);

                // 3.1 Aplicar filtro de inversão para destacar os objetos
                Invert invertFilter = new Invert();
                invertFilter.ApplyInPlace(grayImage);

                // DEBUG: Salva a imagem binarizada para análise
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

                // Analisar cada blob encontrado
                FormasPossiveis tipoDetectado = FormasPossiveis.Desconhecida;
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                //Debug: Desenha os blobs encontrados
                Console.WriteLine($"Total de blobs encontrados: {blobs.Length}");

                foreach (Blob blob in blobs)
                {
                    Console.WriteLine($"Processando blob ID: {blob.ID} | Tamanho: {blob.Rectangle.Width}x{blob.Rectangle.Height}");

                    List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                    Console.WriteLine($"Pontos de contorno: {edgePoints.Count}");

                    List<System.Drawing.Point> pontos = new List<System.Drawing.Point>();
                    foreach (IntPoint p in edgePoints)
                        pontos.Add(new System.Drawing.Point(p.X, p.Y));

                    List<IntPoint> corners;
                    if (shapeChecker.IsCircle(edgePoints))
                    {
                        Console.WriteLine("Circulo detectado!");
                        g.DrawPolygon(pen, pontos.ToArray());
                        tipoDetectado = FormasPossiveis.Circulo;
                        break;
                    }
                    else if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                    {
                        Console.WriteLine($"Poligono Convexo com {corners.Count} vertices");

                        g.DrawPolygon(pen, pontos.ToArray());

                        if (corners.Count == 3)
                        {
                            Console.WriteLine(" Triangulo detectado!");
                            tipoDetectado = FormasPossiveis.Triangulo;
                            break;
                        }
                        else if (corners.Count == 4)
                        {
                            Console.WriteLine(" Quadrado ou Retangulo detectado!");
                            tipoDetectado = FormasPossiveis.Quadrado;
                            break;
                        }
                        else
                        {
                            Console.WriteLine(" Polígono com mais de 4 lados detectado");
                        }
                    }
                }

                g.Dispose();

                // Cria uma instância de forma correspondente à forma detectada
                IForma forma;

                switch (tipoDetectado)
                {
                    case FormasPossiveis.Circulo:
                        forma = new FormaCirculo(100, 100, new Vector2(50, 50));
                        break;
                    case FormasPossiveis.Quadrado:
                        forma = new FormaQuadrado(100, 100, new Vector2(50, 50));
                        break;
                    case FormasPossiveis.Triangulo:
                        forma = new FormaTriangulo(100, 100, new Vector2(50, 50));
                        break;
                    default:
                        forma = new FormaDesconhecida(100, 100, new Vector2(50, 50));
                        break;
                }

                return new ResultadoDeteccao
                {
                    FormaDetectada = forma,
                    ImagemComContorno = imagem
                };
            }
        }

        // Representa o resultado da detecção de formas.
        public class ResultadoDeteccao
        {
            public IForma FormaDetectada { get; set; }
            public Bitmap ImagemComContorno { get; set; }
        }
    }
}
