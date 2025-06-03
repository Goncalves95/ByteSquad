using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ByteSquad.Model;
using ByteSquad.View;

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

            private ResultadoDeteccao ultimaFormaDetectada;

            public ControllerNuclear()
            {
                model = new ModelNuclear();
                webcam = new WebCam();
                view = new ViewNuclear();

                model.CarregarFormasDetectadasDeFicheiro("formas_detectadas.txt");
                model.CarregarFormasConfirmadasDeFicheiro("formas_confirmadas.txt");


                // Conecta eventos do WebCam ao Controller
                webcam.FrameAtualizado += AtualizarImagem;

                // Subscreve-se ao evento ListaDeFormasAlteradas do Model.
                // Esse evento é disparado sempre que uma forma é detectada ou confirmada.
                model.ListaDeFormasAlteradas += (sender, e) =>
                {
                    var formas = model.ObterFormas();
                    view.AtualizarListaFormas(formas);

                    if (e.TipoAcao == "detectada")
                        view.MostrarMensagem("Nova forma detectada: " + e.Forma);
                    else if (e.TipoAcao == "confirmada")
                        view.MostrarMensagem("Forma confirmada e guardada: " + e.Forma);
                };


                // Eventos da View → Controller
                view.BotaoNovaFormaClicado += (s, e) => CriarNovaForma();
                view.BotaoCapturaImagemClicado += (s, e) => TirarFoto();
                view.FormularioFechado += (s, e) => EncerrarPrograma();
                view.BotaoSairClicado += (s, e) => EncerrarPrograma();
                view.BotaoVerFormasConfirmadasClicado += (s, e) => AbrirFicheiroFormasConfirmadas();
                view.BotaoCarregarImagemClicado += (s, e) => CarregarImagemManual();

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
                // Verifica se há uma forma detectada na última foto tirada
                if (ultimaFormaDetectada?.FormaDetectada == null)
                {
                    MessageBox.Show("Tire uma foto antes de adicionar uma forma.");
                    return;
                } 

                var forma = ultimaFormaDetectada.FormaDetectada;

                 // Verifica se é forma desconhecida
                if (forma.GetType().Name == "FormaDesconhecida")
                {
                    view.MostrarMensagem("Não é possível guardar uma forma desconhecida.");
                    return;
                }  

                // Adiciona a forma ao modelo → isto dispara o evento FormaAdicionada
                model.AdicionarFormaConfirmada(forma);
                view.MostrarMensagem($"Forma guardada com sucesso: {forma}");


                // Limpar o estado após adicionar
                ultimaFormaDetectada = null;
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
                    ultimaFormaDetectada = resultado; // Armazena o resultado para uso posterior
                }
                catch (Exception ex)
                {
                    view.MostrarMensagem("Erro ao processar a imagem: " + ex.Message);
                }
            }

            // Abre o ficheiro de formas confirmadas/guardadas, se existir.
            private void AbrirFicheiroFormasConfirmadas()
            {
                string caminho = "formas_confirmadas.txt";

                if (File.Exists(caminho))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = caminho,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        view.MostrarMensagem("Não foi possível abrir o ficheiro: " + ex.Message);
                    }
                }
                else
                {
                    view.MostrarMensagem("Ficheiro 'formas_confirmadas.txt' não encontrado.");
                }
            }

            private void CarregarImagemManual()
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Imagens (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                        using (var original = new Bitmap(dialog.FileName))
                        {
                            // Converte para 24bppRgb (compatível com filtros da AForge)
                            Bitmap imagemCarregada = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            using (Graphics g = Graphics.FromImage(imagemCarregada))
                            {
                                g.DrawImage(original, 0, 0, original.Width, original.Height);
                            }

                        imagemAtual = imagemCarregada;

                    var resultado = model.AnalisarImagem(imagemCarregada);
                    view.MostrarImagem(resultado.ImagemComContorno);
                    view.MostrarFiguraDetectada(resultado.FormaDetectada);

                    FotoPreview preview = new FotoPreview((Bitmap)resultado.ImagemComContorno.Clone());
                    preview.Show();
                    ultimaFormaDetectada = resultado;
                }
            }
            catch (Exception ex)
            {
                view.MostrarMensagem("Erro ao carregar a imagem: " + ex.Message);
            }
        }
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