// =========================
// IDetectorDeFormas.cs (interface do componente Model)
// =========================

using System.Drawing;

namespace ByteSquad
{
    namespace Model
    {
        public interface IDetectorDeFormas
        {
            ResultadoDeteccao Detectar(Bitmap imagemOriginal);
        }
    }
}
