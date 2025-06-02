using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;


namespace ByteSquad.Model
{
    public class DetectorDeFormas2 : IDetectorDeFormas
    {
        public ResultadoDeteccao Detectar(Bitmap imagemOriginal)
        {
            Bitmap imagem = (Bitmap)imagemOriginal.Clone();

            // 1. Tons de cinza + filtros
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(imagem);
            new Median().ApplyInPlace(grayImage);
            new Threshold(120).ApplyInPlace(grayImage);
            new Invert().ApplyInPlace(grayImage);

            // 2. Detectar blobs
            BlobCounter blobCounter = new BlobCounter()
            {
                FilterBlobs = true,
                MinWidth = 30,
                MinHeight = 30,
                ObjectsOrder = ObjectsOrder.Size
            };

            blobCounter.ProcessImage(grayImage);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            Graphics g = Graphics.FromImage(imagem);
            Pen pen = new Pen(Color.Beige, 2);
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            IForma melhorForma = new FormaDesconhecida(0, 0, new Vector2(0, 0));
            int maxArea = 0;

            foreach (Blob blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<IntPoint> corners;

                FormasPossiveis tipoDetectado = FormasPossiveis.Desconhecida;

                if (shapeChecker.IsCircle(edgePoints))
                {
                    tipoDetectado = FormasPossiveis.Circulo;
                }
                else if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                {
                    if (corners.Count == 3)
                        tipoDetectado = FormasPossiveis.Triangulo;
                    else if (corners.Count == 4)
                        tipoDetectado = FormasPossiveis.Quadrado;
                }

                // Criar forma com base no bounding box
                if (tipoDetectado != FormasPossiveis.Desconhecida)
                {
                    Rectangle rect = blob.Rectangle;
                    int largura = rect.Width;
                    int altura = rect.Height;
                    Vector2 centro = new Vector2(rect.X + largura / 2, rect.Y + altura / 2);

                    IForma forma = tipoDetectado switch
                    {
                        FormasPossiveis.Circulo => new FormaCirculo(largura, altura, centro),
                        FormasPossiveis.Triangulo => new FormaTriangulo(largura, altura, centro),
                        FormasPossiveis.Quadrado => new FormaQuadrado(largura, altura, centro),
                        _ => null
                    };

                    if (forma != null && largura * altura > maxArea)
                    {
                        maxArea = largura * altura;
                        melhorForma = forma;
                    }

                    // Desenha contorno da forma
                    List<System.Drawing.Point> pontosDesenho = new List<System.Drawing.Point>();
                    foreach (AForge.IntPoint p in edgePoints)
                        pontosDesenho.Add(new System.Drawing.Point(p.X, p.Y));

                    if (pontosDesenho.Count >= 3)
                    {
                        g.DrawPolygon(pen, pontosDesenho.ToArray());
                    }

                }
            }

            g.Dispose();

            // Se nada foi encontrado, define forma como desconhecida (padrão)
            if (melhorForma is FormaDesconhecida)
            {
                melhorForma = new FormaDesconhecida(100, 100, new Vector2(50, 50));
            }

            melhorForma.DataDeteccao = DateTime.Now;

            return new ResultadoDeteccao
            {
                FormaDetectada = melhorForma,
                ImagemComContorno = imagem
            };
        }
    }
}
