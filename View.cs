// =========================
// View.cs 
// =========================
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace ByteSquad
{
   
    // Representa a interface gráfica da aplicação (camada View).
    // Dispara eventos que são tratados pelo Controller.
    public class View : Form
    {
        // Eventos expostos pela View
        //Disparado quando o utilizador clica no botão "Nova Forma".
        public event EventHandler BotaoNovaFormaClicado;

        // Disparado quando o utilizador clica no botão "Tirar Foto".
        public event EventHandler BotaoCapturaImagemClicado;

        public event EventHandler BotaoSairClicado;

        //Disparado quando o formulário principal é fechado.
        public event EventHandler FormularioFechado;

        //Componentes da interface
        private ListBox listaFormas;
        private Button btnNovaForma; // Botão para adicionar nova forma
        private Button btnCapturarImagem; // Botão para capturar imagem da webcam
        private PictureBox pictureBox; // PictureBox para mostrar a imagem capturada da webcam
        private Label lblMensagem; // Label para mostrar mensagens ao utilizador
        private Button btnSair; // Botão para sair da aplicação

        //Construtor da View. Inicializa os componentes gráficos.
        public View()
        {
            InicializarComponentes();
        }

        //Configura propriedades da janela principal e trata o evento de fechamento.
        public void AtivarInterface()
        {
            this.Text = "ByteSquad - Webcam Viewer";
            this.Width = 500;
            this.Height = 400;
            this.FormClosing += (s, e) => FormularioFechado?.Invoke(this, EventArgs.Empty);
        }

        //Inicializa os controles da interface e associa eventos locais.
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

            // Adiciona o PictureBox à interface
            btnNovaForma = new Button() { Text = "Nova Forma", Top = 290, Left = 10, Width = 150 };
            btnCapturarImagem = new Button() { Text = "Tirar Foto", Top = 290, Left = 170, Width = 150 };
            lblMensagem = new Label() { Top = 330, Left = 10, Width = 460 };

            // Eventos de clique → Eventos públicos
            btnNovaForma.Click += (s, e) => BotaoNovaFormaClicado?.Invoke(this, EventArgs.Empty);
            btnCapturarImagem.Click += (s, e) => BotaoCapturaImagemClicado?.Invoke(this, EventArgs.Empty);

            // Evento de clique do botão "Sair" → Evento público
            // Adiciona o botão "Sair" à interface
            btnSair = new Button() { Text = "Sair", Top = 290, Left = 330, Width = 100 };
            btnSair.Click += (s, e) => BotaoSairClicado?.Invoke(this, EventArgs.Empty);

            // Adiciona os controles à interface
            this.Controls.Add(listaFormas);
            this.Controls.Add(pictureBox);
            this.Controls.Add(btnNovaForma);
            this.Controls.Add(btnCapturarImagem);
            this.Controls.Add(lblMensagem);
            this.Controls.Add(btnSair);
        }

        //Atualiza a lista de formas mostradas no ListBox.
        public void AtualizarListaFormas(List<Forma> formas)
        {
            listaFormas.Items.Clear();
            foreach (var forma in formas)
            {
                listaFormas.Items.Add(forma.ToString());
            }
        }

        //Mostra uma imagem capturada pela webcam no PictureBox.
        public void MostrarImagem(Bitmap imagem)
        {
            pictureBox.Image = imagem;
        }

        //Adiciona uma forma detectada à lista com destaque.
        public void MostrarFiguraDetectada(Forma figura)
        {
            listaFormas.Items.Add("Figura detectada: " + figura);
        }
    
        //Mostra uma mensagem de status ao utilizador.
        public void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = mensagem;
        }

        //Trata o evento do Model que informa que uma forma foi adicionada.
        public void OnFormaAdicionada(Forma forma)
        {
            listaFormas.Items.Add("Nova forma detectada: " + forma);
        }
    }
}
