using System;
using System.Windows.Forms;

namespace ByteSquad
{
    static class Program
    {
        /*Necessário para o funcionamento do Windows Forms.*/
        [STAThread]
        static void Main()
        {
            /*Ativa o visual styles para o programa.*/
            /*Define o modo de renderização de texto compatível.*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*Cria uma nova instância do controlador e iniciar o programa.*/
            Controller controller = new Controller();
            controller.IniciarPrograma();
        }
    }
}