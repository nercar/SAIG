using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using SAIG.BD;

namespace SAIG
{
    public partial class SAIG : Form
    {
        Captahuellas ch;
        public Label[,] mtz = new Label[8, 5];
        public String[,] horas = new String[8, 5];
        public int duration = 0;
        public int wait = 0;
        public int minutos = 2;
        public string scs = "";
        public System.Timers.Timer timer;
        delegate void ctimer();
        String template = "";
        private int fpcHandle;
        private bool FAutoIdentify;
        Conexion cnx;


        public SAIG(int espera, int t_mrj, String suc)
        {
            InitializeComponent();
            ch = new Captahuellas(this);
            cnx = new Conexion();
            cnx.Open();
            wait = espera;
            duration = wait;
            minutos = t_mrj;
            timer2.Enabled = true;
            scs = suc;
            
            timer = new System.Timers.Timer(duration); // fire every 1 second
            timer.Elapsed += HandleTimerElapsed;
        }

        private void ZKFPEngX1_OnCapture(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnCaptureEvent e)
        {
            ch.Y = -1;
            String huella = template;
            bool RegChanged = new bool(), retorna = false, found = false;
            //textBox1.Text = "Acquired fingerprint template:";
            if (!FAutoIdentify)
            {
                ZKFPEngX1.FPEngineVersion = "9";
                IDataReader dr = cnx.GetTemplates();
                using (dr)
                {
                    while (dr.Read())
                    {

                        string tmp1 = ZKFPEngX1.GetTemplateAsString();
                        byte[] buff = (byte[])dr["Huella"];
                        string tmp2 = Encoding.ASCII.GetString(buff);

                        retorna = ZKFPEngX1.VerFingerFromStr(ref tmp2, tmp1, false, ref RegChanged);


                        if (retorna)                                                                     ///la huella fue encontrada
                        {
                            //textBox1.Text = "Identification Failed! Score = " + Convert.ToString(Score);
                           // String strTemp = "Identification success!\n" + " Nombre =" + Convert.ToString(dr["Nombre"]);
                            found = true;
                            int tmpID = Convert.ToInt32(dr["Id"]);
                            int Cedula = Convert.ToInt32(dr["Cedula"]);
                            String Nombre = Convert.ToString(dr["Nombre"]);
                            String cargo1 = Convert.ToString(dr["cargo"]);
                            String foto = Convert.ToString(dr["Foto"]);
                            String rol = Convert.ToString(dr["Rol"]);

                            int cx = 0;
                            String x = Nombre;
                            String[] words = x.Split(',');
                            foreach (var word in words)
                            {
                                if (cx == 0)
                                    nomb.Text = word;
                                else
                                    nom2.Text = word.Substring(1);
                                cx++;
                            }
                            cargo.Text = cargo1;
                            nomb.Visible = true;
                            nom2.Visible = true;
                            //dsn.ci.Visible = true;
                            cargo.Visible = true;
                            try
                            {
                                if (foto.CompareTo("") != 0)
                                    pictureBox1.Image = Image.FromFile(foto);
                                //   pictureBox1.Image = Image.FromFile("../Pics/image1.jpg");
                            }
                            catch (Exception exp)
                            {
                                textBox1.Text = "Error 301: Comunicación con archivo fotos";
                                //Exec("comunicacion con archivo fotos", "Identify");
                            }
                            dr.Close();
                            IDataReader dr1 = cnx.GetHora("" + Cedula);
                            String IN = "", bin = "", bout = "", OUT = "";                           
                            using (dr1)
                            {
                                while (dr1.Read())
                                {
                                    IN = Convert.ToString(dr1["Entrada"]);
                                    bin = Convert.ToString(dr1["Inicia_Desc"]);
                                    bout = Convert.ToString(dr1["Fin_Desc"]);
                                    OUT = Convert.ToString(dr1["Salida"]);
                                }
                            }
                            horario.Text = IN + " " + bin + " - " + bout + " " + OUT;                                    //Se carga horario de trabajador en label
                            horario.Visible = true;

                            ch.horario(Cedula);
                            return;
                            
                        }
                    }
                    if (!found)
                    {
                        //textBox1.Text = "Identification Failed!";  //huella no registrada
                        textBox1.ForeColor = System.Drawing.Color.Red;
                        textBox1.Text = "Huella No Registrada";
                        LimpiaMtr();
                        //limpialabels();
                        if (ch.timelo)
                        {
                            /*dsn.timer1.Stop();
                            dsn.duration = dsn.wait;
                            dsn.timer1.Start();*/
                            timer.Stop();
                            timer.Start();
                        }
                        else
                        {
                            /*//dsn.timer1.Enabled = true;
                            dsn.timer1.Start();
                            timelo = true;*/
                            timer.Start();
                            ch.timelo = true;
                        }

                    }
                }
            }
        }
        public class DoubleBufferedPanel : TableLayoutPanel
        {
            [DefaultValue(true)]
            public new bool DoubleBuffered
            {
                get
                {
                    return base.DoubleBuffered;
                }
                set
                {
                    base.DoubleBuffered = value;
                }
            }
        }

        public void LimpiaMtr()
        {
            Marcaje.SuspendLayout();
            Marcaje.Controls.Clear();
            Marcaje.ResumeLayout();
            //textBox1.Text = "";
            nomb.Text = "";
            nom2.Text = "";
            //ci.Text = "";
            cargo.Text = "";
            horario.Text = "";
            ch.Y = 0;
            pictureBox1.Image = global::SAIG.Properties.Resources.Sin_Foto;
        }
        public void LimpiaMtr1()
        {
            Marcaje.Controls.Clear();
            //pictureBox1.Image = global::CaptaHuellas.Properties.Resources.Sin_Foto;
        }

        public void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            metodo1();
        }

        public void metodo1()
        {

            if (this.InvokeRequired)
            {
                ctimer CB = new ctimer(metodo1);//Creamos una instancia del delegate
                //pasandole el metodo que estamos ejecutando como parametro
                this.Invoke(CB);  //invocamos el Form pasando el delegate void que recien creamos
            }
            else
            {
                LimpiaMtr();
                textBox1.Text = "";
                ch.timelo = false;
                duration = wait;
                pictureBox1.Image = global::SAIG.Properties.Resources.Sin_Foto;
                timer.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label6.Text = DateTime.Now.ToString();
        }



    }
}
