using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using System.Threading.Tasks;
using System.Threading;
using ByteSquad.Model;
using ByteSquad.View;
//using ByteSquad.Infra;

namespace ByteSquad
{
    namespace Controller
    {
        

        // Classe Controller: orquestra a interação entre View, Model e serviços técnicos (WebCam).
        // Escuta eventos da View e do Model e responde de forma centralizada.
        public class ControllerNuclear
        {
            private ModelNuclear model;
            private ViewNuclear view;
            private IWebCam webcam;
            private Bitmap imagemAtual; // Armazena a imagem atual da webcam

            public ControllerNuclear()
            {
                model = new ModelNuclear();
                webcam = new WebCam();
                view = new ViewNuclear();

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

                    // Espera 3 segundos para ver se frames chegam
                    // Isso é importante para garantir que a webcam está funcionando corretamente antes de continuar.
                    Thread.Sleep(3000);

                    if (!webcam.RecebeuFrame())
                        throw new Exception("Problemas com a webcam, pode estar desligada.");

                    view.AtivarInterface();
                    view.MostrarMensagem("Bem-vindo! Webcam iniciada.");
                    Application.Run(view);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao iniciar a WebCam: " + ex.Message);
                    view.MostrarMensagem("Erro ao iniciar a WebCam.");
                    Application.Exit();
                }
            }

            // Responde ao clique do botão "Nova Forma" da View.
            private void CriarNovaForma()
            {
                var formaGenerica = new FormaDesconhecida
                {
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
            public void TirarFoto()
            {
                if (imagemAtual == null)
                {
                    view.MostrarMensagem("Nenhuma imagem disponível.");
                    return;
                }

                try
                {
                    var resultado = model.AnalisarImagem(imagemAtual);
                    view.MostrarImagem(resultado.ImagemComContorno);
                    view.MostrarFiguraDetectada(resultado.FormaDetectada);

                    FotoPreview preview = new FotoPreview((Bitmap)resultado.ImagemComContorno.Clone());
                    preview.Show();
                }
                catch (Exception ex)
                {
                    view.MostrarMensagem("Erro ao processar a imagem: " + ex.Message);
                }
            }

            // Encerramento do programa e desligamento da webcam.
            private async void EncerrarPrograma()
            {
                view.MostrarMensagem("Programa encerrado.");
                await Task.Delay(2000); // Espera 2 segundos para mostrar a mensagem antes de fechar
                webcam.Cam_Off();
                Application.Exit();
            }
        }
    }
}