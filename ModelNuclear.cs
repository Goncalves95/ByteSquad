// =========================
// ModelNuclear.cs
// =========================
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ByteSquad
{
    namespace Model
    {
        
        //Representa o Model da aplicação.
        //Armazena, manipula e notifica sobre formas geométricas detectadas.
        public class ModelNuclear
        {
            private List<IForma> formas = new List<IForma>();
            private IDetectorDeFormas detector = new DetectorDeFormas();

            // Evento disparado quando uma nova forma é adicionada.
            public event Action<IForma> FormaAdicionada;

            // Evento disparado quando a lista de formas é alterada.
            public event Action ListaDeFormasAlteradas;

            // Adiciona uma nova forma à lista e notifica os ouvintes.
            public void AdicionarForma(IForma forma)
            {
                formas.Add(forma);
                FormaAdicionada?.Invoke(forma);
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
                AdicionarForma(resultado.FormaDetectada);
                return resultado;
            }
        }
    }
}
