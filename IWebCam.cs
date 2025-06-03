// =========================
// interface IWebCam.cs
// =========================

using AForge.Video;

public interface IWebCam
{
    // Evento para notificar quando um novo frame é capturado
    event NewFrameEventHandler FrameAtualizado;

    void Cam_On();
    void Cam_Off();
    bool Cam_Status();
    bool RecebeuFrame();
}
