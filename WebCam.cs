using AForge.Video;
using AForge.Video.DirectShow;
using System;

namespace ByteSquad
{
    public class WebCam
    {
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videoDevices;
        /*
         * Evento para notificar quando um novo frame é capturado.
         * Esse evento é disparado quando um novo frame é capturado pela webcam.
         */
        public event NewFrameEventHandler FrameAtualizado;
        /*
         * Método para iniciar a webcam.
         * Verifica se há dispositivos de vídeo disponíveis e inicia o primeiro encontrado.
         */
        public void Cam_On()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += (s, e) => FrameAtualizado?.Invoke(s, e);
                videoSource.Start();
            }
            else
            {
                throw new Exception("Nenhuma webcam encontrada.");
            }
        }

        public void Cam_Off()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }
        /*
         * Método para verificar se a webcam está ligada.
         * Retorna true se a webcam estiver ligada, caso contrário, retorna false.
         */
        public bool Cam_Status()
        {
            return videoSource != null && videoSource.IsRunning;
        }
    }
}