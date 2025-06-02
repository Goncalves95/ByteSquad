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
            public event Action ListaDeFormasAlteradas;

            private List<IForma> formasDetectadas = new List<IForma>();
            private List<IForma> formasConfirmadas = new List<IForma>();

            private const string CaminhoDetectadas = "formas_detectadas.txt";
            private const string CaminhoConfirmadas = "formas_confirmadas.txt";

            // Adiciona uma nova forma à lista de formas.
            public void AdicionarFormaConfirmada(IForma forma)
            {
                formasConfirmadas.Add(forma);
                GuardarFormaEmFicheiro(CaminhoConfirmadas,forma); // Guarda a forma confirmada em ficheiro
                ListaDeFormasAlteradas?.Invoke(); 

            }

            // Retorna uma cópia da lista atual de formas.
            public List<IForma> ObterFormas()
            {
                return new List<IForma>(formas);
            }

            // Usa o DetectorDeFormas para identificar o tipo de forma na imagem.
            public ResultadoDeteccao AnalisarImagem(Bitmap imagem)
            {

                var resultado = detector.Detectar(imagem);
                formasDetectadas.Add(resultado.FormaDetectada); // Só adiciona à lista de detetadas

                GuardarFormaEmFicheiro(CaminhoDetectadas,resultado.FormaDetectada);

                ListaDeFormasAlteradas?.Invoke(); // Atualiza a view se quiser mostrar as detetadas
                return resultado;
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
