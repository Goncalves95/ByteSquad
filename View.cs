using System.Windows.Forms;
using System.Drawing;

namespace ByteSquad
{
    public class View : Form
    {
        private Controller controller;
        private Model model;

        private ListBox listaFormas;
        private Button btnNovaForma;
        private Button btnCapturarImagem;
        private PictureBox pictureBox;
        private Label lblMensagem;
        
        /*
         * Construtor da classe View.
         * Recebe o controlador e o modelo como parâmetros.
         */
        public View(Controller c, Model m)
        {
            controller = c;
            model = m;
            InicializarComponentes();
        }
        /*
         * Método para ativar a interface gráfica do programa.
         * Define o título, tamanho da janela e o evento de fechamento.
         */
        public void AtivarInterface()
        {
            this.Text = "ByteSquad - Webcam Viewer";
            this.Width = 500;
            this.Height = 400;
            this.FormClosing += (s, e) => controller.EncerrarPrograma();
        }
        
         /* Método para inicializar os componentes da interface gráfica.*/
        private void InicializarComponentes()
        {
            listaFormas = new ListBox() { Top = 10, Left = 10, Width = 460, Height = 80 };

            pictureBox = new PictureBox()
            {
                Top = 100,
                Left = 10,
                Width = 460,
                Height = 180,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnNovaForma = new Button() { Text = "Nova Forma", Top = 290, Left = 10, Width = 150 };
            btnCapturarImagem = new Button() { Text = "Tirar Foto", Top = 290, Left = 170, Width = 150 };
            lblMensagem = new Label() { Top = 330, Left = 10, Width = 460 };

            btnNovaForma.Click += (s, e) => controller.CriarNovaForma();
            btnCapturarImagem.Click += (s, e) => controller.TirarFoto();

            this.Controls.Add(listaFormas);
            this.Controls.Add(pictureBox);
            this.Controls.Add(btnNovaForma);
            this.Controls.Add(btnCapturarImagem);
            this.Controls.Add(lblMensagem);
        }

        public void AtualizarListaFormas()
        {
        /*Limpa a lista de formas e adiciona as novas formas do modelo*/
            listaFormas.Items.Clear();
            foreach (var forma in model.ObterFormas())
            {
                listaFormas.Items.Add(forma);
            }
        }

        /*Método para mostrar a imagem capturada na interface gráfica*/

        public void MostrarImagem(Bitmap imagem)
        {
            pictureBox.Image = imagem;
        }

        public void MostrarFiguraDetectada(string figura)
        {
            listaFormas.Items.Add("Figura detectada: " + figura);
        }

        public void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = mensagem;
        }
        /* Método para mostrar mensagens ao adicionar uma nova forma*/
        public void OnFormaAdicionada(string forma)
        {
            listaFormas.Items.Add("Nova forma detectada: " + forma);
        }
    }
} 

