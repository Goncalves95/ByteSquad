// =========================
// Model.cs
// =========================
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ByteSquad
{

    // Representa o Model aplicação.
    // Armazena, manipula e notifica sobre formas geométricas detectadas.
    public class Model
    {
        private List<Forma> formas = new List<Forma>();
        private DetectorDeFormas detector = new DetectorDeFormas();

        public event Action<Forma> FormaAdicionada;
        public event Action ListaDeFormasAlteradas;

        //Inicializa a lista de formas e o detector de formas.
        public void AdicionarForma(Forma forma)
        {
            formas.Add(forma);
            FormaAdicionada?.Invoke(forma);
            ListaDeFormasAlteradas?.Invoke();
        }

        public List<Forma> ObterFormas()
        {
            return new List<Forma>(formas);
        }

        //Usa o DetectorDeFormas para identificar o tipo de forma na imagem.
        public ResultadoDeteccao AnalisarImagem(Bitmap imagem)
        {   
            var resultado = detector.Detectar(imagem);
            AdicionarForma(resultado.FormaDetectada);
            return resultado;
        }
    }
}