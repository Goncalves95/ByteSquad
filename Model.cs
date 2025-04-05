using System.Collections.Generic;
using System.Drawing;

namespace ByteSquad
{
    public class Model
    {
        private List<string> formas = new List<string>();

        /*Evento para notificar quando uma nova forma é adicionada*/
        /*Esse evento é disparado quando uma nova forma é adicionada à lista de formas.*/
        public delegate void FormaAdicionadaHandler(string forma);
        public event FormaAdicionadaHandler FormaAdicionada;


        /*Método para adicionar uma nova forma à lista de formas*/
        public void AdicionarForma(string forma)
        {
            formas.Add(forma);
            FormaAdicionada?.Invoke(forma); // Disparar evento
        }

        public List<string> ObterFormas()
        {
            return formas;
        }

        /*Método de análise de imagem simulação*/
        public string AnalisarImagem(Bitmap imagem)
        {
            /*Simulação de análise de imagem, falta implementar*/
            string formaDetectada = "Circulo, que feio :p"; /*Simulação de forma detectada*/

            AdicionarForma(formaDetectada);
            return formaDetectada;
        }
    }
}
