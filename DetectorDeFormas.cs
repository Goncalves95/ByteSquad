// =========================
// DetectorDeFormas.cs
// =========================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

namespace ByteSquad
{
    //Responsável por analisar uma imagem e detectar o tipo de forma geométrica presente.
    //Desenha os contornos automaticamente na imagem original.
    public class DetectorDeFormas
    {
       
        //Processa a imagem, desenha os contornos e retorna a forma detectada e a imagem modificada.
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
            Threshold thresholdFilter = new Threshold(100);
            thresholdFilter.ApplyInPlace(grayImage);

            // 4. Detectar blobs
            BlobCounter blobCounter = new BlobCounter
            {
                MinWidth = 20,
                MinHeight = 20,
                FilterBlobs = true,
                ObjectsOrder = ObjectsOrder.Size
            };
            blobCounter.ProcessImage(grayImage);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            Graphics g = Graphics.FromImage(imagem);
            Pen pen = new Pen(Color.Red, 2);

            FormasPossiveis tipoDetectado = FormasPossiveis.Desconhecida;
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            // 5. Desenhar contornos e detectar formas
            foreach (Blob blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<System.Drawing.Point> pontos = new List<System.Drawing.Point>();

                foreach (IntPoint p in edgePoints)
                    pontos.Add(new System.Drawing.Point(p.X, p.Y));

                if (pontos.Count > 2)
                    g.DrawPolygon(pen, pontos.ToArray());

                if (shapeChecker.IsCircle(edgePoints))
                {
                    tipoDetectado = FormasPossiveis.Circulo;
                    break;
                }
                if (shapeChecker.IsTriangle(edgePoints))
                {
                    tipoDetectado = FormasPossiveis.Triangulo;
                    break;
                }
                if (shapeChecker.IsQuadrilateral(edgePoints))
                {
                    tipoDetectado = FormasPossiveis.Quadrado;
                    break;
                }
            }

            g.Dispose();

            var forma = new Forma
            {
                TipoForma = tipoDetectado,
                Altura = 100,
                Largura = 100,
                PontoBasilar = new Vector2(50, 50)
            };

            return new ResultadoDeteccao
            {
                FormaDetectada = forma,
                ImagemComContorno = imagem
            };
        }
    }

    //Representa o resultado da detecção de formas.
    public class ResultadoDeteccao
    {
        public Forma FormaDetectada { get; set; }
        public Bitmap ImagemComContorno { get; set; }
    }
}
