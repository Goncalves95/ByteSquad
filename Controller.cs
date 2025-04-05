using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;

namespace ByteSquad
{
    public class Controller
    {
        private Model model;
        private View view;
        private WebCam webcam;
        private Bitmap imagemAtual;

        public Controller()
        {
            model = new Model();
            webcam = new WebCam();
            view = new View(this, model);
            /*
             * Assinando eventos para atualizar a interface gráfica
             * quando uma nova forma for adicionada ou uma nova imagem for capturada.
             */
            webcam.FrameAtualizado += AtualizarImagem;
            model.FormaAdicionada += view.OnFormaAdicionada;
        }
        /*
         * Método para iniciar o programa.
         * Liga a webcam, ativa a interface gráfica e exibe uma mensagem de boas-vindas.
         */
        public void IniciarPrograma()
        {
            try
            {
                webcam.Cam_On();
                view.AtivarInterface();
                view.MostrarMensagem("Bem-vindo! Webcam iniciada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao iniciar a WebCam: " + ex.Message);
            }

            Application.Run(view);
        }
        /*
         * Método para criar uma nova forma.
         * Adiciona uma nova forma genérica à lista de formas e atualiza a interface gráfica.
         */
        public void CriarNovaForma()
        {
            model.AdicionarForma("Forma Genérica");
            view.AtualizarListaFormas();
        }
        /*
         * Método para atualizar a imagem capturada pela webcam.
         * Esse método é chamado sempre que um novo frame é capturado pela webcam.
         */
        private void AtualizarImagem(object sender, NewFrameEventArgs eventArgs)
        {
            imagemAtual = (Bitmap)eventArgs.Frame.Clone();
            view.MostrarImagem(imagemAtual);
        }
        /*
         * Método para capturar uma foto da webcam.
         * Esse método é chamado quando o botão "Tirar Foto" é pressionado.
         */
        public void TirarFoto()
        {
            if (imagemAtual != null)
            {
                view.MostrarImagem(imagemAtual);
                var figura = model.AnalisarImagem(imagemAtual);
                view.MostrarFiguraDetectada(figura);
            }
        }

        public void EncerrarPrograma()
        {
            webcam.Cam_Off();
            view.MostrarMensagem("Programa encerrado.");
        }
    }
}
