using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using SAIG.BD; 
using System.Runtime.InteropServices;
using ZKFPEngXControl;
using AxZKFPEngXControl;


namespace SAIG
{
    class Captahuellas
    {
        
         [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
         private int fpcHandle;
        bool b1 = false, b2 = false, b3 = false, b4 = false;
        String IN = "", OUT = "", bin = "", bout = "", F_aux="", F_Aux="";
        SAIG dsn;
        public int Y = 0;
        //int Y = -1;
        //Listener lis;
        Conexion cnx;
        public bool timelo = false;

        public Captahuellas(SAIG tmp)
        {
            

            dsn = tmp;
            cnx = new Conexion();
            if (dsn.ZKFPEngX1.InitEngine() == 0)
            {
                fpcHandle = dsn.ZKFPEngX1.CreateFPCacheDB();
                dsn.ZKFPEngX1.BeginCapture();
            }
            //dsn.inicia_mtr();
        }
      ////////////////////

        public static void alzheimer()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
        
        /////////////////////////////////////////
     
       /* public void Identify()
        {
            bool aparece = false;
            //GriauleFingerprintLibrary.DataTypes.FingerprintTemplate TestTemp = null;
            try
            {  
                    IDataReader dr = cnx.GetTemplates();

                    using (dr)
                    {

                        while (dr.Read())
                        {                            
                            byte[] buff = (byte[])dr["Huella"];
                            int quality = Convert.ToInt32(dr["Quality"]);
                         
                            TestTemp = new GriauleFingerprintLibrary.DataTypes.FingerprintTemplate();
                            TestTemp.Size = buff.Length;
                            TestTemp.Buffer = buff;
                            TestTemp.Quality = quality;
                          
                            int score;
                          
                            if (Identify(TestTemp, out score))
                            {
                                int tmpID = Convert.ToInt32(dr["Id"]);
                                int Cedula = Convert.ToInt32(dr["Cedula"]);
                                String Nombre = Convert.ToString(dr["Nombre"]);
                                String cargo = Convert.ToString(dr["cargo"]);
                                String foto = Convert.ToString(dr["Foto"]);
                                String rol = Convert.ToString(dr["Rol"]);


                                aparece = true;
                                int cx = 0;
                                String x = Nombre; 
                                String[] words = x.Split(',');
                                foreach (var word in words)
                                {
                                    if (cx == 0)
                                        dsn.nomb.Text = word;
                                    else
                                        dsn.nom2.Text = word.Substring(1);
                                    cx++;
                                }


                                
                                //dsn.ci.Text = ""+Cedula;
                                dsn.cargo.Text = cargo;
                                dsn.nomb.Visible = true;
                                dsn.nom2.Visible = true;
                                //dsn.ci.Visible = true;
                                dsn.cargo.Visible = true;
                                try
                                {
                                    if (foto.CompareTo("") != 0)
                                        dsn.pictureBox1.Image = Image.FromFile(foto);
                                    //   pictureBox1.Image = Image.FromFile("../Pics/image1.jpg");
                                }
                                catch (Exception e)
                                {
                                    dsn.textBox1.Text = "Error 301: Comunicación con archivo fotos";
                                    //Exec("comunicacion con archivo fotos", "Identify");
                                }
                                IDataReader dr1 = cnx.GetHora(""+Cedula);
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
                                dsn.horario.Text=IN+" "+bin+ " - "+bout+" "+OUT;                                    //Se carga horario de trabajador en label
                                dsn.horario.Visible = true;


                                horario(Cedula);
                                
                                return;
                            }
                            
                        }
                        if (!aparece)
                        {
                            dsn.textBox1.ForeColor = System.Drawing.Color.Red;
                            dsn.textBox1.Text = "Huella No Registrada";
                            dsn.LimpiaMtr();
                            //limpialabels();
                            if (timelo)
                            {
                                /*dsn.timer1.Stop();
                                dsn.duration = dsn.wait;
                                dsn.timer1.Start();
                                dsn.timer.Stop();
                                dsn.timer.Start();
                            }
                            else
                            {
                                //dsn.timer1.Enabled = true;
                                dsn.timer1.Start()
                                timelo = true;
                                dsn.timer.Start();
                                timelo = true;
                            }
                        }
                        else
                        {
                            aparece = false;
                        }
                    }

                
            }
            catch (Exception e)
            {
               //algo va
            }

        }// cierra identify

       */
        public void horario(int Persona)
        {
            bool band = false;
            try
            {
                //DateTime hoy = Dia();
                if (!cnx.Verify("" + Persona, dsn.minutos))
                {
                    //dsn.MatrizMarc.Controls.Clear();
                    //dsn.MatrizMarc.RowStyles.Clear();
                    limpialabels();
                    DateTime hoy = Dia();                   ///////////////////////Aqui se retorna el Lunes inmediatamente anterior al marcaje
                    //cnx.close();
                    //cnx.OPEN();
                    //cnx.SaveHrs(DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"), "" + Persona);
                    band=cnx.SaveHrs(dsn.scs,DateTime.Now.ToString("yyyy/MM/dd"), DateTime.Now.ToString("HH:mm:ss"), "" + Persona);
                    if (band)
                    {
                        dsn.textBox1.ForeColor = System.Drawing.Color.Green;
                        dsn.textBox1.Text = "¡Marcaje registrado exitosamente! ";
                    }
                    else
                    {
                        dsn.textBox1.ForeColor = System.Drawing.Color.Red;
                        dsn.textBox1.Text = "¡Error en registrar Marcaje! ";
                    }
                    //cnx.close();
                    //cnx.OPEN();
                    //Formato("" + Persona, hoy.ToString("ddMMyyyy"), DateTime.Now.ToString("ddMMyyyy"));
                    Formato("" + Persona, hoy.ToString("yyyy/MM/dd"), DateTime.Now.ToString("yyyy/MM/dd"));
                    
                }
                else
                {
                    dsn.textBox1.ForeColor = System.Drawing.Color.Red;
                    dsn.textBox1.Text = "Marcaje ya registrado "+cnx.lstmarcaje;
                    dsn.LimpiaMtr1();
                    //limpialabels();
                }
                if (timelo)
                {
                    /*dsn.timer1.Stop();
                    dsn.duration = dsn.wait;
                    dsn.timer1.Start();*/
                    dsn.timer.Stop();
                    dsn.timer.Start();
                }
                else
                {
                    //dsn.timer1.Enabled = true;
                   /* dsn.timer1.Start();
                    timelo = true;*/
                    dsn.timer.Start();
                    timelo = true;
                }

            }
            catch(Exception e)
            {
                //MessageBox.Show("Error Metodo Horario " + e.Message);
                Exec(e.Message, "Horario");
            }       
        }
       

        public DateTime Dia()
        {
            DateTime hoy=DateTime.Today;
            String Fecha =DateTime.Now.ToString("ddMMyyyy");
            int resta = 0;
            int dia = Int32.Parse(Fecha.Substring(0, 2));
            int mes = Int32.Parse(Fecha.Substring(2, 2));
            int ano = Int32.Parse(Fecha.Substring(4, 4));
            DateTime dateValue = new DateTime(ano, mes, dia);            
            String desc=dateValue.ToString("dddd", new CultureInfo("es-ES"));
            switch (desc.ToLower())
            {
                case "lunes":
                    {
                        resta = 0;
                    } break;
                case "martes":
                    {
                        resta = -1;
                    } break;
                case "miércoles":
                    {
                        resta = -2;
                    } break;
                case "jueves":
                    {
                        resta = -3;
                    } break;
                case "viernes":
                    {
                        resta = -4;
                    } break;
                case "sábado":
                    {
                        resta = -5;
                    } break;
                case "domingo":
                    {
                        resta = -6;
                    } break;
            }

            DateTime lunes = hoy.AddDays(resta);
            return lunes;
           
        }
        //Metodo formato carga los horarios de la persona en el tablalayoutpanel
        public void Formato(String Persona, String lunes, String hoy)
        {
            int mrk = 0;
            IDataReader dr = cnx.GetHrs("" + Persona, lunes,hoy);//se trae las horas marcada por persona desde el lunes anterior
            using (dr)
            {
                dsn.Marcaje.SuspendLayout();
                while (dr.Read())
                {
                    String Fecha = Convert.ToString(dr["Fecha"]);
                    String Hora = Convert.ToString(dr["Hora"]);

                    String dia = Fecha.Substring(0, 2);
                    String mes = Fecha.Substring(3, 2);
                    String ano = Fecha.Substring(6, 4);

                    /*String dia = Fecha.Substring(8, 2);
                    String mes = Fecha.Substring(5, 2);
                    String ano = Fecha.Substring(0, 4);

                    int dia = Int32.Parse(Fecha.Substring(0, 2));
                    int mes = Int32.Parse(Fecha.Substring(2, 2));
                    int ano = Int32.Parse(Fecha.Substring(4, 4));
                    */

                    String Fecha1 = dia + "/" + mes + "/" + ano;
                    if(F_aux.CompareTo(Fecha1)!=0){
                        b1 = false;
                        b2 = false;
                        b3 = false;
                        b4 = false;
                    }
                   
                    runmtz( Fecha1, Hora);
                }
                dsn.Marcaje.ResumeLayout();
            }
        }

        public void runmtz(String Fecha, String Hora)//carga matriz de labels
        {
           
            int x = -1, y=-1;
           
            int dia = Int32.Parse(Fecha.Substring(0, 2));
            int mes = Int32.Parse(Fecha.Substring(3, 2));
            int ano = Int32.Parse(Fecha.Substring(6, 4));

            DateTime dateValue = new DateTime(ano, mes, dia);
            x=((int)dateValue.DayOfWeek)-1;
            if (x == -1)
                x = 6;                                                  //valido que no sea domingo
            
            //y = diferencia(Hora);
            String fec = "" + dia + "" + mes + "" + ano;
            if (fec.CompareTo(F_Aux) == 0 && Y<=4)
            {
                Y++;               
            }
            else
            {
                Y = 0;
                F_Aux = fec;
                
            }
            if (Y < 4 )
            {
                cargalabels(x, Y, Hora);               
            }

        }

       


        public void cargalabels(int x, int y, String Hora){
            

            dsn.mtz[x, y] = new Label();            
            dsn.mtz[x, y].Text = Hora;
            dsn.mtz[x, y].Size = new Size(50, 13);
            dsn.mtz[x, y].Invalidate();
            dsn.mtz[x, y].Update();
            dsn.mtz[x, y].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            dsn.Marcaje.Controls.Add(dsn.mtz[x, y], x, y); 
        }
 

        public void limpialabels() {
            dsn.Marcaje.SuspendLayout();
            dsn.Marcaje.Controls.Clear();
            dsn.Marcaje.ResumeLayout();
            
        }

        public void Exec(String exec, String Metodo)
        {
            String fecha = DateTime.Now.ToString("ddMMyyyy");
            String hora = DateTime.Now.ToString("HH:mm:ss");
            cnx.Sav_LOG(exec,Metodo,fecha,hora);
            String r4 = Application.StartupPath;
            //System.Diagnostics.Process.Start("C:\\Users\\garzon07\\Desktop\\CaptaHuellas_prog\\Tareas\\cierra.bat");
            System.Diagnostics.Process.Start(r4+"\\Tareas\\cierra.bat");
        }

    }
}
