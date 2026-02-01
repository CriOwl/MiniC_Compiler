using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniC
{
    public partial class Form1 : Form
    {
        string Archivo;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OpcCompilar_Click(object sender, EventArgs e)
        {
            AnalizadorLexico AL = new AnalizadorLexico(); // creamos un objeto de nuestro analizador lexico
            List<Tokens> LstTokens = AL.AnalisisLexico(rtbEditor.Text); // Le pasamos el archivo para crear una lista de tokens

            rtbEditor.Text += "\n--------------------------------------------------------------------\n";
            for (int i = 0; i < LstTokens.Count; i++)
                rtbEditor.Text += "Linea: " + LstTokens[i].Linea.ToString() + "    " +
                    "Lexema: " + LstTokens[i].Lexema + "    " +
                    "Token: " + LstTokens[i].Token + "\n";

            // PARA EL ANALIZADOR SINTÁCTICO
            AnalizadorSintactico AS = new AnalizadorSintactico();
            AS.AnalisisSintactico(LstTokens);

            rtbEditor.Text +=
                "\n\n---------------------------------------\n";
            rtbEditor.Text += "Análisis sintáctico completado con éxito.";
        }

        private void OpcNuevo_Click(object sender, EventArgs e)
        {
            rtbEditor.Clear();
            Archivo = null;
            Form1.ActiveForm.Text = "MiniC";
        }

        private void OpcAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog AbrirArchivo = new OpenFileDialog();
            AbrirArchivo.Filter = "MiniC | *.c";
            if (AbrirArchivo.ShowDialog() == DialogResult.OK)
            {
                Archivo = AbrirArchivo.FileName;
                using (StreamReader sr = new StreamReader(Archivo))
                {
                    rtbEditor.Text = sr.ReadToEnd();
                }
                Form1.ActiveForm.Text = "MiniC | " + Archivo;
            }
        }

        private void OpcGuardar_Click(object sender, EventArgs e)
        {
            SaveFileDialog GuardarArchivo = new SaveFileDialog(); // creamos un objeto de cuador de dialogo
            GuardarArchivo.Filter = "MiniC | *.c"; // aplicamos filtro solo muestre archivos .c
            if (Archivo != null) // si la variable archivo tiene un valor, previamente he abierto (sobreescribir)
            {
                using (StreamWriter sw = new StreamWriter(Archivo))
                {
                    sw.Write(rtbEditor.Text); // si hay nombre de archivo entonces guardar en ese mismo
                }
            }
            else
            {
                if (GuardarArchivo.ShowDialog() == DialogResult.OK)
                {
                    Archivo = GuardarArchivo.FileName; // recibimos el nombre desde el usuario para guardar
                    using (StreamWriter sw = new StreamWriter(GuardarArchivo.FileName))
                    {
                        sw.Write(rtbEditor.Text); // guardamos con ese nombre asignado por el usuario, lo que está en rtbEditor
                    }
                }
            }
        }

        private void OpcGuardarComo_Click(object sender, EventArgs e)
        {
            SaveFileDialog GuardarComo = new SaveFileDialog()
            {
                Title = "Guadar como:",
                Filter = "MiniC | *.c",
                AddExtension = true
            }; // generoel objeto guardar como y asigno sus propiedades
            GuardarComo.ShowDialog();// mostrar el cuadro de dialogo
            if (Archivo != null && GuardarComo.FileName != String.Empty) // tiene que haber nombre de archivo y tiene estar asignado por el usuario
            {
                Archivo = GuardarComo.FileName;// obtenemos el nombre de archivo asignado por el usuario
                using (StreamWriter sw = new StreamWriter(GuardarComo.FileName))
                {
                    sw.Write(rtbEditor.Text); // escribir el archivo mediante sw, se escribe lo que está en rtbEditor
                    Form1.ActiveForm.Text = "MiniC | " + Archivo; // cambiar el nombrede la cabecera de la ventana
                    sw.Close();
                }
            }
        }

        private void OpcSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();// destruir la ventana de dialogo
        }
    }
}
