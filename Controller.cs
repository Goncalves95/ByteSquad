// =========================
// Controller.cs
// =========================
using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;

namespace ByteSquad
{
    
    // Classe Controller: orquestra a interação entre View, Model e serviços técnicos (WebCam).
    // Escuta eventos da View e do Model e responde de forma centralizada.
    public class Controller
    {
        private Model model; 
        private View view;   
        private WebCam webcam;
        private Bitmap imagemAtual; // Armazena a imagem atual da webcam

        public Controller()
        {
            model = new Model(); 
            webcam = new WebCam();
            view = new View();

            // Conecta eventos do WebCam ao Controller
            webcam.FrameAtualizado += AtualizarImagem;

            // Eventos do Model → View
            model.ListaDeFormasAlteradas += () =>
            {
                var formas = model.ObterFormas();
                view.AtualizarListaFormas(formas);
            };

            // Evento disparado quando uma nova forma é adicionada
            model.FormaAdicionada += view.OnFormaAdicionada;

            // Eventos da View → Controller
            view.BotaoNovaFormaClicado += (s, e) => CriarNovaForma();
            view.BotaoCapturaImagemClicado += (s, e) => TirarFoto();
            view.FormularioFechado += (s, e) => EncerrarPrograma();
            view.BotaoSairClicado += (s, e) => EncerrarPrograma();
        }

        // Inicializa a interface gráfica e a webcam.
        public void IniciarPrograma()
        {
            try
            {
                webcam.Cam_On();
                view.AtivarInterface();
                view.MostrarMensagem("Bem-vindo! Webcam iniciada.");
                Application.Run(view);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao iniciar a WebCam: " + ex.Message);
            }
        }

        // Responde ao clique do botão "Nova Forma" da View.
        private void CriarNovaForma()
        {
            var formaGenerica = new Forma
            {
                TipoForma = FormasPossiveis.Desconhecida,
                Largura = 100,
                Altura = 100,
                PontoBasilar = new System.Numerics.Vector2(0, 0)
            };

            model.AdicionarForma(formaGenerica);
        }

        // Evento de frame novo da webcam — atualiza a imagem no PictureBox.
        // Este evento é disparado sempre que um novo frame é capturado pela webcam.
        private void AtualizarImagem(object sender, NewFrameEventArgs eventArgs)
        {
            imagemAtual = (Bitmap)eventArgs.Frame.Clone();
            view.MostrarImagem(imagemAtual);
        }

        // Captura e processa a imagem atual, disparando a detecção de figura.
        // Mostra a imagem com contorno e a figura detectada na View.
        // Também abre uma nova janela de preview com a imagem processada.
        private void TirarFoto()
        {
            if (imagemAtual != null)
            {
                var resultado = model.AnalisarImagem(imagemAtual); // retorna ResultadoDeteccao
                view.MostrarImagem(resultado.ImagemComContorno);
                view.MostrarFiguraDetectada(resultado.FormaDetectada);

                FotoPreview preview = new FotoPreview((Bitmap)resultado.ImagemComContorno.Clone());
                preview.Show();
            }
        }


        // Encerramento do programa e desligamento da webcam.
        private void EncerrarPrograma()
        {
            webcam.Cam_Off();
            view.MostrarMensagem("Programa encerrado.");
        }
    }
}
