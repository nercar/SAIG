using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAIG.BD;
using SAP.Middleware.Connector;
using AForge.Video.DirectShow;
using AForge.Video;
using ZKFPEngXControl;
using AxZKFPEngXControl;

namespace SAIG
{
    public partial class CreaTrabajador : Form
    {
        Conexion cnx;
        String ruta = "";
        FilterInfoCollection dispositivos;
        VideoCaptureDevice fuente;
        private int fpcHandle;
        bool bgn = false;
        bool admin = false;
        bool reg = false, eliminar=false;
        bool band_fuente = false;
        private bool FAutoIdentify;
        int adm = 1;
        int calidad = 0;
        String CI = "";
        String template = "";
        String inicia = "", f_pausa = "", in_pausa = "", fin = "";
        String entrada = "", desc1 = "", desc2 = "", salida = "", IDAdm = "";
        int Iper = 0;
        Bitmap bimage;


        public CreaTrabajador()
        {
             try
            {
                 InitializeComponent();
                
                cnx = new Conexion();
                bgn = begain();                                                                       //Verifico si Hay algun trabajador creado sino el primero sera Administrador

                dispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);
           }
           catch (Exception e)
           {
               MessageBox.Show("Error Constructor clase CreaTrabajador " + e.ToString());
           }
        }
        public bool begain()
        {
            bool band = false;
            //cnx.OPEN();
            IDataReader rd = cnx.getAdms();

            int val = 0;
            using (rd)
            {
                while (rd.Read())
                {
                    val = Convert.ToInt32(rd["cantidad"]);
                }
            }
            if (val == 0)
                band = true;
            else
                band = false;
            //cnx.close();
            return band;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            buscar();
        }
        public RfcDestination ConectaSap()
        {
            try
            {
                cnx.OPEN();
                IDataReader rd = cnx.param_sap();
                String IP = "", USU = "", PSS = "";
                using (rd)
                {
                    while (rd.Read())
                    {
                        IP = Convert.ToString(rd["DirSAP"]);
                        USU = Convert.ToString(rd["UsuSAP"]);
                        PSS = Convert.ToString(rd["PasSAP"]);
                        ruta = Convert.ToString(rd["Ruta_foto"]);
                        cnx.OPEN();
                    }
                }
                RfcConfigParameters parms = new RfcConfigParameters();

                //parms.Add(RfcConfigParameters.AppServerHost, "192.168.125.120");
                parms.Add(RfcConfigParameters.AppServerHost, IP);
                parms.Add(RfcConfigParameters.SystemID, "00");
                parms.Add(RfcConfigParameters.Name, "ERP");
                parms.Add(RfcConfigParameters.User, USU);
                parms.Add(RfcConfigParameters.Password, PSS);
                parms.Add(RfcConfigParameters.Client, "300");
                parms.Add(RfcConfigParameters.Language, "ES");

                RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);

                return rfcDest;
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error en el metodo conexion ConectaSap " +e.ToString());
                textBox4.Text = "Error en conexion con servidor SAP, por favor contacte a soporte tecnico\n";
                return null;
            }

        }

        public void buscar()                                                                            // evento boton buscar
        {
            if (textBox1.Text.Length > 6)
            {  
                try
                {
                    String  hora="";                    
                    RfcDestination rfcDest = ConectaSap();                                               //Conecto con SAP
                    RfcRepository rfcRep = rfcDest.Repository;
                    IRfcFunction func = rfcRep.CreateFunction("Z_OBTENER_DATOS_BIO");                    //Me conecto con la funcion de ABAP
                    func.SetValue("CEDULA", textBox1.Text);                                              //Paso la cedula por parametro
                    CI = textBox1.Text;
                    func.Invoke(rfcDest);                                                                //Ejecuto funcion
                    Iper = func.GetInt("ID");                                                            //Traigo ID de personal
                    if (Iper != 0)                                                                       //Valido que hayan datos
                    {
                        label5.Text = func.GetString("NOMBRE");
                        label6.Text = func.GetString("CARGO");
                        label7.Text = func.GetString("DEPARTAMENTO");

                        hora = func.GetString("HORA");
                        inicia = func.GetString("INGRESO");
                        in_pausa = func.GetString("IPAUSA");
                        f_pausa = func.GetString("FPAUSA");
                        fin = func.GetString("SALIDA");
                        //if (inicia.CompareTo("00:00:00") == 0)
                        //    Regla(hora);
                        //else
                            label1.Text = inicia + " " + in_pausa + " - " + f_pausa + " " + fin;

                        label5.Visible = true;
                        label6.Visible = true;
                        label7.Visible = true;
                        label1.Visible = true;
                        if (ZKFPEngX1.InitEngine() == 0)                                                //Verifico que haya conexion con Biometrico
                        {

                            textBox4.Text = "Conexión Biometrico Verificada\r\n";
                            fpcHandle = ZKFPEngX1.CreateFPCacheDB();
                            ZKFPEngX1.BeginCapture();                                                   //permito que la hueya sea cargada en el picturebox
                            ZKFPEngX1.EnrollCount = 3;

                        
                            if (bgn)                                                                            //Verifico si no hay administrador
                            {
                            textBox4.AppendText("Presione 3 veces para registrar administrador...\r\n");
                            adm = 0;

                            }
                            else if (!admin)                                                                     ////Verifico si administrador no ha validado
                            {
                            textBox4.AppendText("Presione Administrador para validar\r\n");
                            }
                            else
                            {
                            textBox4.AppendText("Usuario presione 3 veces para validar\r\n");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error al conectar con biometrico");
                        }
                    }
                    else
                    {
                        textBox4.AppendText("Trabajador no encontrado en SAP, verifique cedula\r\n");
                        textBox1.Text = "";
                    }
                }
                catch (SAP.Middleware.Connector.RfcCommunicationException se)
                {
                    textBox4.Text = "Error en conexion con servidor SAP, contacte a soporte tecnico";
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error Boton2 " + ex.ToString());
                    textBox4.Text = "Error metodo buscar " + ex.ToString();
                }
            }
        }

        public void Regla(String hora)
        {
            
            bool b = false;
            String[] vec = hora.Split(' ');
            for (int i = 0; i < vec.Length; i++)
            {
                if (i == 1)
                    entrada = vec[i];
                else if (i == 2)
                {
                    if (vec[i].CompareTo("am") != 0)
                    {
                        String[] vec2 = vec[i].Split('-');
                        desc1 = vec2[0];
                        desc2 = vec2[1];
                    }
                    else
                    {
                        salida = vec[(i + 2)];
                        b = true;
                    }

                }
                else if (i == 3 && !b)
                    salida = vec[i];

            }
            if (!b)
            {
                entrada = entrada + "0";
                desc1 = desc1 + "0";
                desc2 = desc2 + "0";
                salida = salida + "0";

            }
            label1.Text = entrada + " " + desc1 + " - " + desc2 + " " + salida;
            //Upd_Horario(entrada, desc1, desc2, salida, ced);
            //Console.WriteLine("Entrada " +entrada+" desc1 "+desc1+" desc2 "+desc2+" salida "+salida);
            //Console.ReadLine();

        }

        public void Bgn_Registro()
        {
            ZKFPEngX1.CancelEnroll();
            ZKFPEngX1.EnrollCount = 3;
            ZKFPEngX1.BeginEnroll();
            //textBox4.AppendText("Comienza registro");
            reg = true;
        }

        /***********************************************************Metodos WebCam*********************************************************************************/

        void videosource(object sender, NewFrameEventArgs e)
        {
            Bitmap image = (Bitmap)e.Frame.Clone();
            pictureBox4.Image = image;

        }

        void IniciaWC()
        {
            band_fuente = true;
            fuente = new VideoCaptureDevice(dispositivos[0].MonikerString);
            fuente.NewFrame += new NewFrameEventHandler(videosource);
            fuente.Start();
        }
        /************************************************************************************************************************************************************/

        /******************************************************Verifica que no este registrado el trabajador*******************************************************************/
        public Boolean check()
        {
            //cnx.OPEN();
            IDataReader dr1 = cnx.Valida(CI);  //Obtiene todos los datos de trabajador

            using (dr1)
            {
                while (dr1.Read())
                {
                    int val = 1;
                    val = Convert.ToString(dr1["Cedula"]).Length;
                    //String Nombre = Convert.ToString(dr1["Nombre"]);
                    if (val != 0)
                    {
                        return true;

                    }//cierra if val

                }// cierra while

            }//cierra using
            return false;
        }



        public void Grabar()
        {
             try
            {                
                if (!check())                                                                       //llamada al metodo de validar
                {
                    try
                    {

                        //cnx.close();
                        //cnx.OPEN();
                        cnx.saveTemp(CI, label5.Text, label6.Text, template, ruta + CI + ".jpg", 1, calidad);  //Crea el trabajador en tabla trabajadores
                        //cnx.close();
                        //cnx.OPEN();
                        cnx.saveAuditoria(IDAdm, CI, DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss"), "Crear Trabajador");  //Crea el trabajador en tabla trabajadores
                        //cnx.close();
                        //////////////////////////
                        bimage.Save(ruta + CI + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        bimage.Dispose();
                        ////////////////////////////////////

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en crear trabajador " + ex.ToString());
                    }
                    try
                    {
                        //cnx.OPEN();
                        cnx.Save_Horario(Iper, CI, inicia, in_pausa, f_pausa, fin);                  //Crea el horario del trabajador en tabla trabajadores
                        //cnx.close();
                        MessageBox.Show("¡Usuario Registrado Exitosamente!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en registrar horario " + ex.ToString());
                    }
                    /**/
                }
                else
                {
                    MessageBox.Show("¡Error al ingresar, Usuario Ya Registrado!");

                }

                // MessageBox.Show("Guardado");
                //salir = true;                
                adm = 1;
                bgn = false;
                admin = true;
                limpia();
                button1.Enabled = false;
                button7.Enabled = false;
                pictureBox4.Image = null;
                button6.Enabled = false;
                button9.Enabled = false;

            }
            catch (Exception ex)
            {
                textBox4.Text = "Error Evento guardar " + ex.Message;
                return;
            }       
        
        }

        public void Grabar_Adm()
        {
            try
            {
               
                    try
                    {
                        cnx.saveTemp(CI, label5.Text, label6.Text, template,ruta + CI + ".jpg", 0,calidad);  //Crea el trabajador en tabla trabajadores         
                        bimage.Save(ruta + CI + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        bimage.Dispose();
                        ////////////////////////////////////

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en crear trabajador " + ex.ToString());
                    }
                    try
                    {
                        
                        cnx.Save_Horario(Iper, CI, inicia, in_pausa, f_pausa, fin);                  //Crea el horario del trabajador en tabla trabajadores
                        
                        textBox4.AppendText("\r\n¡Administrador Registrado Exitosamente! \r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en registrar horario " + ex.ToString());
                    }
                    /**/
                
                // MessageBox.Show("Guardado");
                //salir = true;                
                    IDAdm = CI;
                adm = 1;
                bgn = false;
                admin = true;
                limpia();
                button1.Enabled = false;
                button7.Enabled = false;
                pictureBox4.Image = null;
                button6.Enabled = false;
                button9.Enabled = false;

            }
            catch (Exception ex)
            {
                textBox4.Text = "Error Evento guardar Administrador" + ex.Message;
                return;
            }

        }
        public void limpia()
        {
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";
            label1.Text = "";
            pictureBox1.Controls.Clear();
            progressBar1.Value = 0;
            textBox1.Text = "";
            pictureBox1.Image = null;
            textBox4.Text = "Scanner Listo \r\n";
            textBox4.AppendText("Ingrese nueva cedula a buscar: \r\n");
            reg = false;
            ZKFPEngX1.CancelCapture();
        }

        public void buscar_eliminar()
        {
            String foto = "";
            eliminar = true;    
                IDataReader dr1 = cnx.GetTemplates(textBox2.Text);  //Obtiene todos los datos de trabajador
                using (dr1)
                {
                    while (dr1.Read())
                    {
                        label19.Text = Convert.ToString(dr1["Nombre"]);
                        label18.Text = Convert.ToString(dr1["Cargo"]);
                        label19.Visible = true;
                        label18.Visible = true;
                        foto = Convert.ToString(dr1["Foto"]);
                        try
                        {
                            if (foto.CompareTo("") != 0)
                            {
                                pictureBox6.Image = Image.FromFile(foto);  // Ubico la foto de la persona
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        textBox3.Text = "Presione administrador para validar \n";
                        if (ZKFPEngX1.InitEngine() == 0)                                                //Verifico que haya conexion con Biometrico
                        {

                            textBox4.Text = "Conexión Biometrico Verificada\r\n";
                            fpcHandle = ZKFPEngX1.CreateFPCacheDB();
                            ZKFPEngX1.BeginCapture();                                                   //permito que la huella sea cargada en el picturebox
                           
                        }
                        else
                        {
                            MessageBox.Show("Error al conectar con biometrico");
                        }
                        ZKFPEngX1.BeginCapture();
                    }
                }
        }
        public void limpia_eliminar()
        {
            textBox2.Text = "";
            label18.Text = "";
            label19.Text = "";
            pictureBox5.Image = null;
            progressBar2.Value = 0;
            textBox3.Text = "";
            pictureBox6.Image = null;
            button8.Enabled = false;
            admin = false;
            eliminar = false;
        }

        /***********************************************************Metodos libreria biometrico*********************************************************************************/

        private void ZKFPEngX1_OnImageReceived(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnImageReceivedEvent e)       //Cuando se recibe una imagen en el sensor
        {
            //ShowHintImage(0);
            if (!eliminar)
            {
                Graphics g = pictureBox1.CreateGraphics();
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(bmp);
                int dc = g.GetHdc().ToInt32();
                ZKFPEngX1.PrintImageAt(dc, 0, 0, bmp.Width, bmp.Height);
                g.Dispose();
                pictureBox1.Image = bmp;
                if (bgn && !reg)
                {
                    Bgn_Registro();
                }
                else if (admin && !reg)
                {
                    Bgn_Registro();
                }
                else if (!admin && !reg)
                {
                    if (ZKFPEngX1.IsRegister)
                    {
                        ZKFPEngX1.CancelEnroll();
                    }
                    FAutoIdentify = false;
                    ZKFPEngX1.SetAutoIdentifyPara(FAutoIdentify, fpcHandle, 8);
                    //textBox1.Text = "begin verification(1:N)";
                }
            }
            else
            {
                Graphics g = pictureBox5.CreateGraphics();
                Bitmap bmp = new Bitmap(pictureBox5.Width, pictureBox5.Height);
                g = Graphics.FromImage(bmp);
                int dc = g.GetHdc().ToInt32();
                ZKFPEngX1.PrintImageAt(dc, 0, 0, bmp.Width, bmp.Height);
                g.Dispose();
                pictureBox5.Image = bmp;
                if (ZKFPEngX1.IsRegister)
                {
                    ZKFPEngX1.CancelEnroll();
                }
                FAutoIdentify = false;
                ZKFPEngX1.SetAutoIdentifyPara(FAutoIdentify, fpcHandle, 8);
            }


        }

        private void ZKFPEngX1_OnEnroll(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnEnrollEvent e)                      //Cuando se va a registrar
        {
            if (e.actionResult)
            {
             
                template = ZKFPEngX1.GetTemplateAsString();
                calidad = ZKFPEngX1.LastQuality;
              

                IniciaWC();                     //Inicia metodo de la webcam********************************************
                button1.Enabled = true;
                button7.Enabled = true;
                button9.Enabled = true;
                button6.Enabled = true;
                
               
                ZKFPEngX1.CancelCapture();

                
                reg = false;
                ZKFPEngX1.CancelEnroll();
                ZKFPEngX1.EnrollCount = 3;
            }
            else
            {
                textBox4.Text = "Error en huellas, por favor intente nuevamente\r\n";
                textBox4.AppendText ("Presione 3 veces para registrar\r\n");
                ZKFPEngX1.CancelEnroll();
                ZKFPEngX1.EnrollCount = 3;
                ZKFPEngX1.BeginEnroll();
            }

        }

        private void ZKFPEngX1_OnFeatureInfo(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEvent e)
        {
            String strTemp = "Calidad de Huella";
            if (e.aQuality != 0)
            {
                strTemp = strTemp + " Mala";
            }
            else
            {
                strTemp = strTemp + " Buena";
            }
            if (ZKFPEngX1.EnrollIndex != 1)
            {
                if (ZKFPEngX1.IsRegister)
                {
                    if (ZKFPEngX1.EnrollIndex - 1 > 0 && (ZKFPEngX1.EnrollIndex-1)!=1)
                    {
                        strTemp = strTemp + '\n' + "... Presione: " + Convert.ToString(ZKFPEngX1.EnrollIndex - 1) + " veces más \r\n";
                    }
                    else if ((ZKFPEngX1.EnrollIndex - 1) == 1)
                    {
                        strTemp = strTemp + '\n' + "... Presione: " + Convert.ToString(ZKFPEngX1.EnrollIndex - 1) + " vez más \r\n";
                    }
                }
            }
            
            textBox4.AppendText(strTemp);
        }

        private void ZKFPEngX1_OnCapture(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnCaptureEvent e)
        {
           
            String huella = template;           
            bool RegChanged = new bool(), retorna = false, found = false;          
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


                        if (retorna)
                        {                            
                            String strTemp = "Administrador Validado:\r\n" + Convert.ToString(dr["Nombre"]+"\r\n");
                            IDAdm= Convert.ToString(dr["Cedula"]);
                            
                            if (!eliminar)
                            {
                                textBox4.Text = strTemp;
                                textBox4.AppendText("Trabajador \r\n" + label5.Text + "\r\n presione 3 veces para validar\r\n");
                            }
                            else
                            {
                                textBox3.Text = strTemp;
                                button8.Enabled = true;
                            }

                            found = true;
                            admin = true;
                            Bgn_Registro();
                        }
                    }
                    if (!found)
                    {
                        textBox4.AppendText("¡Administrador no Valido!");
                    }
                }
            }
        }




        /*****************************************************************************************************************************************************************/
        /*********************************************************************Evento de Botones***************************************************************************/

        private void button1_Click(object sender, EventArgs e)   //////////////////////////////////////////////Boton Grabar
        {
            if (bgn)
            {
                Grabar_Adm();
            }
            else
            {
                Grabar();                                
            }
        }

        private void button7_Click(object sender, EventArgs e)   //////////////////////////////////////////////Boton Actualizar
        {
            if (check())                                                                       //llamada al metodo de validar
            {
                try
                {
                    cnx.UpdateTmpl(CI);
                    cnx.saveTemp(CI, label5.Text, label6.Text, template, ruta + CI + ".jpg", 1, calidad);
                    cnx.saveAuditoria(IDAdm, CI, DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss"), "Actualizo Trabajador");
                    MessageBox.Show("¡Trabajador Actualizado!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en actualizar " + ex.ToString());
                }


                // adm = 1;
                // bgn = false;
                admin = true;
                limpia();
                button1.Enabled = false;
                pictureBox4.Image = null;
                button6.Enabled = false;
                button7.Enabled = false;
            }
            else
            {
                MessageBox.Show("No se puede Actualizar el trabajador, Debe estar registrado");
            }
            
              
        }

        private void button5_Click(object sender, EventArgs e)//boton buscar pestaña Eliminar
        {
            buscar_eliminar();
        }

        private void button8_Click(object sender, EventArgs e)// boton eliminar
        {
            try
            {
                
                cnx.Borrartmpl(textBox2.Text);  //Borra el trabajador en tabla trabajadores                
                cnx.saveAuditoria(IDAdm, textBox2.Text, DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss"), "Elimino Trabajador");//registra operacion en auditoria
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error evento eliminar trabajador " + ex.ToString());
            }
            MessageBox.Show("Usuario eliminado correctamente!");
            eliminar = false;
            ZKFPEngX1.CancelCapture();
            //bgn = false;
            limpia_eliminar();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                buscar();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                buscar_eliminar();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            bimage = (Bitmap)pictureBox4.Image.Clone();
            button1.Enabled = true;
            button7.Enabled = true;
            fuente.Stop();
            band_fuente = false;
        }


    }
}
