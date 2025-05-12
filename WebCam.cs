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
    public class WebCam : IWebCam
    {
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videoDevices;

        private bool recebeuFrame = false;

        // Evento disparado sempre que um novo frame é capturado pela webcam.
        // Outros componentes (como o Controller) podem se inscrever neste evento.
        // O evento é do tipo NewFrameEventHandler, que é um delegado fornecido pela biblioteca AForge.NET.
        public event NewFrameEventHandler FrameAtualizado;

        // Inicializa e ativa a webcam. Se encontrar um dispositivo válido, começa a capturar frames.
        // Caso contrário, lança uma exceção informando que não há webcams disponíveis.
        // Inicia a webcam com tratamento de exceções.
        public void Cam_On()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new Exception("Nenhuma webcam encontrada.");

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                videoSource.NewFrame += (s, e) =>
                {
                    recebeuFrame = true; //Marca que recebeu pelo menos 1 frame
                    FrameAtualizado?.Invoke(s, e);
                };

                videoSource.Start();
            }
            catch (Exception ex)
            {
                // Lança uma exceção mais clara para ser capturada no Controller
                throw new Exception("Erro ao iniciar a webcam: " + ex.Message, ex);
            }
        }

        // Para e desliga a webcam com segurança.
        public void Cam_Off()
        {
            try
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
            }
            catch (Exception ex)
            {
                // Ignorar falha no desligamento silenciosamente ou logar se necessário
                Console.WriteLine("Erro ao desligar a webcam: " + ex.Message);
            }
        }

        // Verifica se a webcam está ativa.
        // Retorna true se a webcam estiver em execução, caso contrário, false.
        public bool Cam_Status()
        {
            return videoSource != null && videoSource.IsRunning;
        }

        // Verifica se a webcam recebeu pelo menos um frame.
        public bool RecebeuFrame()
        {
            return recebeuFrame;
        }
    }
} 
