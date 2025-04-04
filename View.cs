using System.Windows.Forms;

namespace ByteSquad
{
    public class View : Form
    {
        private Controller controller;
        private Model model;

        private ListBox listaFormas;
        private Button btnNovaForma;

        public View(Controller c, Model m)
        {
            controller = c;
            model = m;
            InicializarComponentes();
        }
        /*Método para inicializar os componentes da view.*/
        private void InicializarComponentes()
        {
            this.Text = "ByteSquad - MVC Básico";
            this.Width = 400;
            this.Height = 300;

            listaFormas = new ListBox() { Dock = DockStyle.Top, Height = 200 };
            btnNovaForma = new Button() { Text = "Nova Forma", Dock = DockStyle.Bottom };

            btnNovaForma.Click += (s, e) => controller.CriarNovaForma();

            this.Controls.Add(listaFormas);
            this.Controls.Add(btnNovaForma);
        }
        /*Método para atualizar a lista de formas na view.*/
        public void AtualizarListaFormas()
        {
            listaFormas.Items.Clear();
            foreach (var forma in model.ObterFormas())
            {
                listaFormas.Items.Add(forma);
            }
        }
    }
}
