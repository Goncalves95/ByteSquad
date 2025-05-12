// =========================
// interface IWebCam.cs
// =========================

using AForge.Video;
public interface IWebCam
{
    event NewFrameEventHandler FrameAtualizado;

    void Cam_On();
    void Cam_Off();
    bool Cam_Status();
    bool RecebeuFrame();
}
