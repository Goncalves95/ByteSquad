using System.Windows.Forms;

namespace ByteSquad
{
    public class Controller
    {
        private Model model;
        private View view;

        public Controller()
        {
            model = new Model();
            view = new View(this, model);
        }
        
        /*Método para iniciar o programa.*/
        /*Necessário para o funcionamento do Windows Forms.*/
        public void IniciarPrograma()
        {
            Application.Run(view);
        }

        /*Método para adicionar uma nova forma ao modelo e atualizar a view.*/
        public void CriarNovaForma()
        {
            model.AdicionarForma("Forma Genérica");
            view.AtualizarListaFormas();
        }
    }
}
