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
        }

        public class ResultadoDeteccao
        {
            public IForma FormaDetectada { get; set; }
            public Bitmap ImagemComContorno { get; set; }
        }
    }
}
