using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Data;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace SAIG.BD
{
    class Conexion
    {
        public OleDbConnection db;
        public String lstmarcaje = "";
        String bd = "", usu = "", pass = "", ip = "";
        bool bbd = false;
        bool busu = false;
        bool bpss = false;
        bool bip = false;
        //public readonly string CONX = "Provider=SQLOLEDB;Data Source=.;Initial Catalog=Empleados; User id=sa; Password=15990317";
        public readonly string CONX = "", CONX2 = "";
        SqlConnection conn;
        public Conexion()
        {
            try
            {
                foreach (string line in File.ReadLines(@"c:\temp\setup.ini", Encoding.UTF8))
                {
                    string[] words = line.Split(' ');
                    foreach (string word in words)
                    {
                        //Console.WriteLine(word);
                        word.ToLower();
                        if (bbd)
                        {
                            bbd = false;
                            bd = word;
                        }
                        if (busu)
                        {
                            busu = false;
                            usu = word;
                        }
                        if (bpss)
                        {
                            bpss = false;
                            pass = word;
                        }
                        if (bip)
                        {
                            bip = false;
                            ip = word;
                        }
                        else if (word.CompareTo("bd") == 0)
                        {
                            bbd = true;
                        }
                        else if (word.CompareTo("usu") == 0)
                        {
                            busu = true;
                        }
                        else if (word.CompareTo("pass") == 0)
                        {
                            bpss = true;
                        }
                        else if (word.CompareTo("ip") == 0)
                        {
                            bip = true;
                        }
                    }

                }
                //CONX = "Provider=SQLOLEDB;Data Source=.;Initial Catalog=" + bd + "; User id=" + usu + "; Password=" + pass + "";
                CONX = "Provider=SQLOLEDB;Data Source=" + ip + ";Initial Catalog=" + bd + "; User id=" + usu + "; Password=" + pass + "";
                CONX2 = "Data Source=" + ip + "; Initial Catalog=" + bd + ";Integrated Security=false;uid=" + usu + "; pwd=" + pass + "";
                OPEN();
                Open();
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Conexion SQL" + e.Message);
                //save_log(e.Message, "Constructor_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
            }
        }
        public void Open()
        {
            try
            {
                conn = new SqlConnection(CONX2);
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error metodo Open "+e);
            }

        }

        public void OPEN()
        {
            try
            {
                db = new OleDbConnection(CONX);
                db.Open();
            }
            catch (Exception e)
            {
                //MessageBox.Show("Problemas apertura de conexion " + e.Message);
                save_log(e.Message, "Metodo Open conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
            }
        }

        public void saveTemp(String CI, String nom, String cargo, String huella, String foto, int rol, int QL)
        {
            String txt = "INSERT INTO Trabajadores(Cedula, Nombre, Cargo, Huella, Foto, Rol, Quality) Values (@Cedula, @Nombre, @cargo, @Huella, @Foto, @Rol, @Quality)";
            byte[] x = Encoding.ASCII.GetBytes(huella);
            try
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = txt;
                    cmd.Parameters.AddWithValue("@Cedula", CI);
                    cmd.Parameters.AddWithValue("@Nombre", nom);
                    cmd.Parameters.AddWithValue("@cargo", cargo);
                    cmd.Parameters.Add(new SqlParameter("@huella", x));
                    cmd.Parameters.AddWithValue("@Foto", foto);
                    cmd.Parameters.AddWithValue("@Rol", rol);
                    cmd.Parameters.AddWithValue("@Quality", QL);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo saveTemp " + e.ToString());
            }

        }

        public void Save_Horario(int I_per, String persona, String entrada, String ipausa, String fpausa, String salida)
        {
            String free1 = "Sabado", free2 = "Domingo";
            string str = "INSERT INTO Horario(ID, Cedula, Entrada, Inicia_Desc, Fin_Desc, Salida, Dia_lib1, Dia_lib2) Values (@ID,@Cedula,@Entrada,@Inicia_desc,@Fin_Desc,@Salida,@Dia_lib1,@Dia_lib2)";
                try
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = str;
                        cmd.Parameters.AddWithValue("@ID", I_per);
                        cmd.Parameters.AddWithValue("@Cedula", persona);
                        cmd.Parameters.AddWithValue("@Entrada", entrada);
                        cmd.Parameters.AddWithValue("@Inicia_Desc", ipausa);
                        cmd.Parameters.AddWithValue("@Fin_Desc", fpausa);
                        cmd.Parameters.AddWithValue("@Salida", salida);
                        cmd.Parameters.AddWithValue("@Dia_lib1", free1);
                        cmd.Parameters.AddWithValue("@Dia_lib2", free2);
                        cmd.ExecuteNonQuery();
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show("Error en Metodo Save_Horario" + e.Message);
                    //save_log(e.Message, "SaveHorario_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                }
        }

        public void saveAuditoria(String CI_admin, String CI_trbj, String Fecha, String evento)
        {
            string str = "INSERT INTO Auditoria(CI_admin, CI_Empleado, Fecha, Evento) Values (@CI_admin,@CI_Empleado,@Fecha,@Evento)";
            try
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = str;
                    cmd.Parameters.AddWithValue("@CI_admin", CI_admin);
                    cmd.Parameters.AddWithValue("@CI_Empleado", CI_trbj);
                    cmd.Parameters.AddWithValue("@Fecha", Fecha);
                    cmd.Parameters.AddWithValue("@Evento", evento);
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error en Metodo Save_Horario" + e.Message);
                //save_log(e.Message, "SaveHorario_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
            }

        }

        public IDataReader param_sap()
        {
            try
            {
                //db.Open();
                string str = "Select * from configuracion";
                OleDbCommand ole = new OleDbCommand(str, db);
                IDataReader ID = ole.ExecuteReader();

                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo getTemplates " + e.Message);
                //save_log(e.Message, "Param_sap_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }

        }
        public IDataReader getAdms()
        {
            try
            {
                //db.Open();
                string str = "SELECT  COUNT(cedula) as cantidad FROM trabajadores";
                OleDbCommand ole = new OleDbCommand(str, db);
                IDataReader ID = ole.ExecuteReader();
                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo getHrs " + e.Message);
                //save_log(e.Message, "getAdms_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }

        }
        public IDataReader Valida(String ced)
        {
            //string str = "SELECT Nombre, cedula from Trabajadores where Cedula = '" + ced + "'  ";
            //OleDbCommand ole = new OleDbCommand(str, db);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Nombre, cedula from Trabajadores where Cedula = '" + ced + "'  ";
            cmd.CommandType = CommandType.Text;
            IDataReader ID = cmd.ExecuteReader();
            //return ole.ExecuteReader();
            //db.Close();
            return ID;
        }

        public IDataReader GetTemplates()
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                //db.Open();
               
                //OleDbCommand ole = new OleDbCommand(str, db);
                cmd.CommandText = "SELECT * FROM trabajadores";
                cmd.CommandType = CommandType.Text;
                IDataReader ID = cmd.ExecuteReader();

                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo getTemplates " + e.Message);
                //save_log(e.Message, "getTmplates_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }
        }
     
        public void UpdateTmpl(String CI)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                //db.Open();

                //OleDbCommand ole = new OleDbCommand(str, db);
                cmd.CommandText = "delete from trabajadores where cedula ='" + CI + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //return ole.ExecuteReader();
                //db.Close();
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo UpdateTemp " + e.Message);
                //save_log(e.Message, "getTmplates_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
              
            }
          
        }
        public IDataReader GetTemplates(String CI)
        {

            SqlCommand cmd = conn.CreateCommand();
            try
            {
                //db.Open();

                //OleDbCommand ole = new OleDbCommand(str, db);
                cmd.CommandText = "Select * from trabajadores where Cedula = '" + CI + "'";
                cmd.CommandType = CommandType.Text;               
                IDataReader ID = cmd.ExecuteReader();
                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo UpdateTemp " + e.Message);
                //save_log(e.Message, "getTmplates_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }
          
        }

        public void Borrartmpl(String CI)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = "UPDATE trabajadores SET rol = 3 where Cedula = '" + CI + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                //return ole.ExecuteReader();
                //db.Close();
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo BorrarTmpl " + e.Message);
                //save_log(e.Message, "getTmplates_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));   
            }

        }

   /***************************************************************************************************************************************************************************/
   /***********************************************************************Metodos Sistema Marcaje*****************************************************************************/
   /***************************************************************************************************************************************************************************/
        
       
        
        public bool Verify(String persona, int minutos)
        {
            bool b = false;
            SqlCommand cmd = conn.CreateCommand();
            using (db)
            {
                try
                {
                    //string str = "SELECT Hora FROM Marcaje WHERE clave=(SELECT MAX(CLAVE) FROM Marcaje) AND Persona LIKE '"+persona+"'";
                    //string str = "SELECT Fecha, Hora FROM Marcaje WHERE clave=(SELECT MAX(CLAVE) FROM Marcaje where Persona =" + persona + ") AND Persona = " + persona;
                    cmd.CommandText = "SELECT Fecha, Hora FROM Marcaje WHERE clave=(SELECT MAX(CLAVE) FROM Marcaje where Persona =" + persona + ") AND Persona = " + persona;
                    
                    //OleDbCommand ole = new OleDbCommand(str, db);
                    //IDataReader ID = ole.ExecuteReader();
                    IDataReader ID = cmd.ExecuteReader();
                   
                    using (ID)
                    {
                        //MessageBox.Show("paso using id");
                        if (ID.Read())
                        {
                            
                            String tmpID = Convert.ToString(ID["Hora"]);
                            
                            lstmarcaje = tmpID;
                            String Fech = Convert.ToString(ID["Fecha"]);
                            
                            DateTime dia1 = FormDate(Fech);
                            
                            DateTime hoy = DateTime.Today;
                            int result = DateTime.Compare(dia1, hoy);
                            if (result == 0)
                            {
                                DateTime hora = Convert.ToDateTime(tmpID);
                                DateTime ahora = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));

                                //DateTime oldDate = new DateTime(2002, 7, 15);
                                //DateTime newDate = DateTime.Now;

                                // Difference in days, hours, and minutes.
                                TimeSpan ts = ahora - hora;

                               
                                if ((ts.Hours < 1) && ts.Minutes <= minutos)//Valido que no acepte marcajes cada 29 minutos!
                                {
                                    b = true;
                                }
                            }

                        }
                    }

                    ID.Close();
                    return b;

                }
                catch (Exception e)
                {
                    //MessageBox.Show("Error en Metodo Verify" + e.Message);
                    save_log(e.Message, "Verify_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                    return b;
                }
            }
        }
        
        
        public DateTime FormDate(String Fecha)
        {
            /*int dia = Int32.Parse(Fecha.Substring(8, 2));
            int mes = Int32.Parse(Fecha.Substring(5, 2));
            int ano = Int32.Parse(Fecha.Substring(0, 4));*/

            int dia = Int32.Parse(Fecha.Substring(0, 2));
            int mes = Int32.Parse(Fecha.Substring(3, 2));
            int ano = Int32.Parse(Fecha.Substring(6, 4));
            DateTime date1 = new DateTime(ano, mes, dia);
            return date1;
        }

        
        public bool SaveHrs(String sucursal, String Fecha, String hora, String CI)
        {
            string str = "INSERT INTO DBO.MARCAJE(Id, Fecha, Hora, Persona, Verificador) Values (@Id,@Fecha,@Hora,@Persona,@Verificador)";
            try
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = str;
                    cmd.Parameters.AddWithValue("@Id", sucursal);
                    cmd.Parameters.AddWithValue("@Fecha", Fecha);                    
                    cmd.Parameters.AddWithValue("@Hora", hora);
                    cmd.Parameters.AddWithValue("@Persona", CI);
                    cmd.Parameters.AddWithValue("@Verificador", "0");
                    cmd.ExecuteNonQuery();
                    return true;
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show("Error en Metodo Save_Horario" + e.Message);
                //save_log(e.Message, "SaveHorario_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                save_log(e.Message, "SaveHrs_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return false;
            }

        }
       
       public IDataReader GetHora(String Cedula)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT *FROM Horario where Cedula =" + Cedula;
                cmd.CommandType = CommandType.Text;
                IDataReader ID = cmd.ExecuteReader();
                return ID;
            }
            catch (Exception e)
            {
                save_log(e.Message, "GetHora_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }
        }
        /*
       

        public void close()
        {
            //db.Close();
            try
            {
                db.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problemas clausura de conexion " + e.Message);
                // save_log(e.Message, "Close_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
            }
        }

        
        /*
        public IDataReader GetMarks(String persona, String Fecha)
        {
            try
            {
                //db.Open();
                string str = "SELECT  COUNT(Hora) as marcajes FROM marcaje WHERE persona like '" + persona + "' and Fecha like '" + Fecha + "'' ";
                OleDbCommand ole = new OleDbCommand(str, db);
                IDataReader ID = ole.ExecuteReader();
                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo getHrs " + e.Message);
                //save_log(e.Message, "getMarks_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }
        }*/
        
        public IDataReader GetHrs(String persona, String lunes, String hoy)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
               
                //db.Open();
                //string str = "Select * from Marcaje where (persona = '" + persona + "') and (fecha between '" + lunes + "' and '" + hoy + "') order by Fecha, Hora asc ";
                string str = "select T.Id, T.Fecha, T.Hora " +
                        "from (select T.Id, T.Fecha, T.Hora, T.persona, row_number() over(partition by T.fecha order by T.hora asc) as rn " +
                        "from Marcaje as T  where  T.Persona = '" + persona + "' and (T.fecha between '" + lunes + "' and '" + hoy + "') ) as T " +
                        "where T.rn <= 4 order by T.Fecha, T.Hora asc";

                cmd.CommandText = str;
                IDataReader ID = cmd.ExecuteReader();

                //OleDbCommand ole = new OleDbCommand(str, db);
                //IDataReader ID = ole.ExecuteReader();
                //return ole.ExecuteReader();
                //db.Close();
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Metodo getHrs " + e.Message);
                //save_log(e.Message, "getHrs_conexion", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"));
                return null;
            }
        }
        
        public void save_log(String exp, String metodo, String fecha, String hora)
        {
            using (db)
            {
                try
                {

                    string str = "INSERT INTO Log_errores(Excepcion, Metodo, Fecha, Hora) Values (?,?,?,?)";

                    OleDbCommand ole = new OleDbCommand(str, db);
                    ole.Parameters.Add(new OleDbParameter("@Excepcion", exp));
                    ole.Parameters.Add(new OleDbParameter("@Metodo", metodo));
                    ole.Parameters.Add(new OleDbParameter("@Fecha", fecha));
                    ole.Parameters.Add(new OleDbParameter("@Hora", hora));
                    ole.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    //MessageBox.Show("Error en funcion INSERT " + e);
                }
            }
        }
        
        public void Sav_LOG(String exp, String metodo, String fecha, String hora)
        {
            //String txt = "Insert into Log_errores values @Excepcion, @Metodo, @Fecha, @Hora";            
            String txt = "INSERT INTO Log_errores(Excepcion, Metodo, Fecha, Hora) Values (@Excepcion, @Metodo, @Fecha, @Hora)";
            try
            {

                using (SqlConnection conn = new SqlConnection(CONX2))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = txt;
                        cmd.Parameters.AddWithValue("@Excepcion", exp);
                        cmd.Parameters.AddWithValue("@Metodo", metodo);
                        cmd.Parameters.AddWithValue("@Fecha", fecha);
                        cmd.Parameters.AddWithValue("@Hora", hora);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error metodo Update ");
            }

        }

    }
}
