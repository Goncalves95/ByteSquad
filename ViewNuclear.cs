// =========================
// ViewNuclear.cs
// =========================
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using ByteSquad.Model;

namespace ByteSquad
{
    namespace View
    {
    
        // Representa a interface gráfica da aplicação (camada View).
        // Dispara eventos que são tratados pelo Controller.
        public class ViewNuclear : Form
        {
            // Eventos expostos pela View
            // Disparado quando o utilizador clica no botão "Nova Forma".
            public event EventHandler BotaoNovaFormaClicado;

            // Disparado quando o utilizador clica no botão "Tirar Foto".
            public event EventHandler BotaoCapturaImagemClicado;

            // Disparado quando o utilizador clica no botão "Sair".
            public event EventHandler BotaoSairClicado;

            // Disparado quando o formulário principal é fechado.
            public event EventHandler FormularioFechado;
            
            // Disparado quando o utilizador clica no botão "Ver Formas Guardadas".
            public event EventHandler BotaoVerFormasConfirmadasClicado;
            // Disparado quando o utilizador clica no botão "Carregar Imagem".
            public event EventHandler BotaoCarregarImagemClicado;


            // Componentes da interface
            private ListBox listaFormas;
            private Button btnNovaForma;
            private Button btnCapturarImagem;
            private PictureBox pictureBox;
            private Label lblMensagem;
            private Button btnSair;
            private Button btnCarregarImagem; // Botão para carregar imagem de ficheiro

     
            // Construtor da View. Inicializa os componentes gráficos.
            public ViewNuclear()
            {
                InicializarComponentes();
            }

    
            // Configura propriedades da janela principal e trata o evento de fechamento.
            public void AtivarInterface()
            {
                this.Text = "ByteSquad - Webcam Viewer";
                this.Width = 500;
                this.Height = 450;
                this.FormClosing += (s, e) => FormularioFechado?.Invoke(this, EventArgs.Empty);
            }

     
            // Inicializa os controles da interface e associa eventos locais.
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

                btnNovaForma = new Button() { Text = "Guardar Forma", Top = 290, Left = 10, Width = 150 };
                btnCapturarImagem = new Button() { Text = "Tirar Foto", Top = 290, Left = 170, Width = 150 };
                lblMensagem = new Label() { Top = 330, Left = 10, Width = 460 };

                // Eventos de clique → Eventos públicos
                btnNovaForma.Click += (s, e) => BotaoNovaFormaClicado?.Invoke(this, EventArgs.Empty);
                btnCapturarImagem.Click += (s, e) => BotaoCapturaImagemClicado?.Invoke(this, EventArgs.Empty);


                // Botão para ver formas confirmadas/histórico
                Button btnVerFormasConfirmadas = new Button() { Text = "Histórico", Top = 290, Left = 330, Width = 140 };
                btnVerFormasConfirmadas.Click += (s, e) => BotaoVerFormasConfirmadasClicado?.Invoke(this, EventArgs.Empty);
                this.Controls.Add(btnVerFormasConfirmadas);

                // Botão para carregar imagem de ficheiro
                btnCarregarImagem = new Button() { Text = "Carregar Imagem", Top = 360, Left = 170, Width = 150 };
                btnCarregarImagem.Click += (s, e) => BotaoCarregarImagemClicado?.Invoke(this, EventArgs.Empty);
                this.Controls.Add(btnCarregarImagem);

                // Botão para sair da aplicação
                btnSair = new Button() { Text = "Sair", Top = 360, Left = 330, Width = 140, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false  };
                btnSair.FlatAppearance.BorderColor = Color.Red;
                btnSair.FlatAppearance.BorderSize = 2;
                btnSair.Click += (s, e) => BotaoSairClicado?.Invoke(this, EventArgs.Empty);


                // Adiciona os controles à interface
                this.Controls.Add(listaFormas);
                this.Controls.Add(pictureBox);
                this.Controls.Add(btnNovaForma);
                this.Controls.Add(btnCapturarImagem);
                this.Controls.Add(lblMensagem);
                this.Controls.Add(btnSair);

                // Definir foco inicial no botão "Tirar Foto"
                this.ActiveControl = btnCapturarImagem;
            }


            // Atualiza a lista de formas mostradas no ListBox.
            public void AtualizarListaFormas(List<IForma> formas)
            {
                listaFormas.Items.Clear();
                foreach (var forma in formas)
                    listaFormas.Items.Add(forma.ToString());
            }
        
            // Mostra uma imagem capturada pela webcam no PictureBox.
            public void MostrarImagem(Bitmap imagem)
            {
                pictureBox.Image = imagem;
            }

            // Adiciona uma forma detectada à lista com destaque.
            public void MostrarFiguraDetectada(IForma figura)
            {
                listaFormas.Items.Add("Figura detectada: " + figura);
            }

            // Mostra uma mensagem de status ao utilizador.
            public void MostrarMensagem(string mensagem)
            {
                lblMensagem.Text = mensagem;
            }

        }
    }
}
