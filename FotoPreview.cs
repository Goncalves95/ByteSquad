// =========================
// FotoPreview.cs 
// =========================
using System.Drawing;
using System.Windows.Forms;

namespace ByteSquad
{
    namespace View
    {
        // Janela auxiliar para exibir a imagem capturada da webcam em destaque.
        public class FotoPreview : Form
        {
            public FotoPreview(Bitmap imagem)
            {
                this.Text = "Foto Capturada";
                this.Width = 500;
                this.Height = 400;
                this.StartPosition = FormStartPosition.CenterScreen;

                // Cria um PictureBox para exibir a imagem capturada
                PictureBox picture = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = imagem,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle
                };

                this.Controls.Add(picture);
            }
        }
    }
}
