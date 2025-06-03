// =========================
// ModelNuclear.cs
// =========================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ByteSquad
{
    namespace Model
    {

        //Representa o Model da aplicação.
        //Armazena, manipula e notifica sobre formas geométricas detectadas.
        public class ModelNuclear
        {
            private List<IForma> formas = new List<IForma>();

            // Instância do detector de formas, responsável por identificar formas na imagem.
            // Pode ser substituído por uma implementação mockada para TESTES.
            private IDetectorDeFormas detector = new DetectorDeFormas();

            // Evento disparado quando a lista de formas é alterada.
            //public event Action ListaDeFormasAlteradas;
            public delegate void NotificarListaDeFormasAlteradas(object sender, FormaEventArgs e);
            public event NotificarListaDeFormasAlteradas ListaDeFormasAlteradas;

            private List<IForma> formasDetectadas = new List<IForma>();
            private List<IForma> formasConfirmadas = new List<IForma>();

            private const string CaminhoDetectadas = "formas_detectadas.txt";
            private const string CaminhoConfirmadas = "formas_confirmadas.txt";

            // Adiciona uma nova forma à lista de formas.
            public void AdicionarFormaConfirmada(IForma forma)
            {
                formasConfirmadas.Add(forma);
                GuardarFormaEmFicheiro(CaminhoConfirmadas,forma); // Guarda a forma confirmada em ficheiro
                ListaDeFormasAlteradas?.Invoke(this, new FormaEventArgs("confirmada", forma));

            }

            // Retorna uma cópia da lista atual de formas.
            public List<IForma> ObterFormas()
            {
                return new List<IForma>(formasConfirmadas);
            }
            
            public List<IForma> ObterFormasDetectadas()
            {
                return new List<IForma>(formasDetectadas);
            }

            // Usa o DetectorDeFormas para identificar o tipo de forma na imagem.
            public ResultadoDeteccao AnalisarImagem(Bitmap imagem)
            {

                var resultado = detector.Detectar(imagem);
                formasDetectadas.Add(resultado.FormaDetectada); // Só adiciona à lista de detetadas

                GuardarFormaEmFicheiro(CaminhoDetectadas, resultado.FormaDetectada);
                // Dispara o evento com a forma detectada
                ListaDeFormasAlteradas?.Invoke(this, new FormaEventArgs("detectada", resultado.FormaDetectada));

                return resultado;
            }

            public class FormaEventArgs : EventArgs
            {
                public string TipoAcao { get; }
                public IForma Forma { get; }

                public FormaEventArgs(string tipoAcao, IForma forma)
                {
                TipoAcao = tipoAcao;
                Forma = forma;
                }
            }

        
            // Carrega formas detectadas de um ficheiro de texto.
            public void CarregarFormasDetectadasDeFicheiro(string caminhoFicheiro)
            {
                if (!File.Exists(caminhoFicheiro)) return;

                foreach (var linha in File.ReadAllLines(caminhoFicheiro))
                {
                    var forma = FormaFactory.CriarFormaDeTexto(linha);
                    if (forma != null)
                        formasDetectadas.Add(forma);
                }
            }

            // Carrega formas guardadas de um ficheiro de texto.
            public void CarregarFormasConfirmadasDeFicheiro(string caminhoFicheiro)
            {
                if (!File.Exists(caminhoFicheiro)) return;

                foreach (var linha in File.ReadAllLines(caminhoFicheiro))
                {
                    var forma = FormaFactory.CriarFormaDeTexto(linha);
                    if (forma != null)
                        formasConfirmadas.Add(forma);
                }
            }
            // Guarda uma forma em ficheiro de texto.
            private void GuardarFormaEmFicheiro(string caminho, IForma forma)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(caminho, append: true))
                    {
                        writer.WriteLine(forma.ToString());
                    }
                }   
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao guardar forma: {ex.Message}");
                }
            }


        }
    }
}
