// =========================
// DetectorDeFormas.cs(parte do componente Model)
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
            Threshold thresholdFilter = new Threshold(120);
            thresholdFilter.ApplyInPlace(grayImage);

            // 3.1 Aplicar filtro de inversão para destacar os objetos
            Invert invertFilter = new Invert();
            invertFilter.ApplyInPlace(grayImage);

            // DEBUG: Salva a imagem binarizada para análise
            //Apenas DEBUG, para verificação do resultado da binarização
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

            FormasPossiveis tipoDetectado = FormasPossiveis.Desconhecida;
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            // 5. Desenhar contornos e detectar formas
            //DEBUG: Mostra o número de blobs encontrados           
            Console.WriteLine($"Total de blobs encontrados: {blobs.Length}");
            
            //Itera sobre os blobs encontrados
            //e tenta identificar a forma geométrica
            foreach (Blob blob in blobs)
            {
                //DEBUG: Mostra o ID e tamanho do blob
                Console.WriteLine($"Processando blob ID: {blob.ID} | Tamanho: {blob.Rectangle.Width}x{blob.Rectangle.Height}");
                
                //Desenha o retângulo delimitador do blob
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                
                //DEBUG: Mostra o número de pontos de contorno encontrados
                Console.WriteLine($"Pontos de contorno: {edgePoints.Count}");

                //Desenha o contorno do blob
                List<System.Drawing.Point> pontos = new List<System.Drawing.Point>();
                foreach (IntPoint p in edgePoints)
                pontos.Add(new System.Drawing.Point(p.X, p.Y));

                //Desenha o contorno do blob

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
        
        //Libertar os recursos da memória  
        g.Dispose();

        //Verificar se a forma é desconhecida
        var forma = new Forma
        {
            TipoForma = tipoDetectado,
            Altura = 100,
            Largura = 100,
            PontoBasilar = new Vector2(50, 50)
        };
        //Se a forma for desconhecida, regista o erro
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
