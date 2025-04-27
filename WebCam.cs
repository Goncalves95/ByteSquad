// =========================
// WebCam.cs
// =========================
using AForge.Video;
using AForge.Video.DirectShow;
using System;

namespace ByteSquad
{
    // Classe responsável por encapsular o acesso à webcam utilizando AForge.NET.
    // Atua como um serviço técnico independente, comunicando-se via eventos.
    // Permite que a View e o Controller se concentrem em suas responsabilidades sem se preocupar com detalhes de implementação da webcam.
    // Utiliza eventos para notificar quando um novo frame é capturado, permitindo que outros componentes se inscrevam e respondam a esses eventos.
    public class WebCam
    {
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videoDevices;

        // Evento disparado sempre que um novo frame é capturado pela webcam.
        // Outros componentes (como o Controller) podem se inscrever neste evento.
        // O evento é do tipo NewFrameEventHandler, que é um delegado fornecido pela biblioteca AForge.NET.
        public event NewFrameEventHandler FrameAtualizado;

        // Inicializa e ativa a webcam. Se encontrar um dispositivo válido, começa a capturar frames.
        // Caso contrário, lança uma exceção informando que não há webcams disponíveis.
        public void Cam_On()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                // Inscreve-se no evento de novo frame e repassa para os ouvintes externos
                videoSource.NewFrame += (s, e) => FrameAtualizado?.Invoke(s, e);
                videoSource.Start();
            }
            else
            {
                throw new Exception("Nenhuma webcam encontrada.");
            }
        }

        // Desativa a webcam com segurança, parando a captura de frames.
        public void Cam_Off()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        // Verifica se a webcam está ativa e capturando.
        // Retorna true se a webcam estiver em execução, caso contrário, false.
        public bool Cam_Status()
        {
            return videoSource != null && videoSource.IsRunning;
        }
    }
} 
