using System.Collections.Generic;

namespace ByteSquad
{
    public class Model
    {
        private List<string> formas = new List<string>();
        
        public void AdicionarForma(string forma)
        {
            formas.Add(forma);
        }

        public List<string> ObterFormas()
        {
            return formas;
        }
    }
}
