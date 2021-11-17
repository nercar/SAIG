using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using SAIG.BD;


namespace SAIG
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
           /* Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CreaTrabajador());*/
            bool btipo = false;
            bool besp = false;
            bool bmrj = false;
            bool bsuc = false;

            int tipo = -1;
            int t_esp = 0;
            int t_mrj = 0;

            String sucursal = "";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            foreach (string line in File.ReadLines(@"c:\temp\setup.ini", Encoding.UTF8))
            {
                string[] words = line.Split(' ');
                foreach (string word in words)
                {
                    //Console.WriteLine(word);
                    word.ToLower();
                    if (btipo)
                    {
                        tipo = Int32.Parse(word);
                        btipo = false;
                        //break;
                    }
                    if (besp)
                    {
                        besp = false;
                        t_esp = Int32.Parse(word);
                    }
                    if (bmrj)
                    {
                        bmrj = false;
                        t_mrj = Int32.Parse(word);
                    }
                    if (bsuc)
                    {
                        bsuc = false;
                        sucursal = word;
                    }
                    if (word.CompareTo("tipo") == 0)
                    {
                        btipo = true;
                    }
                    else if (word.CompareTo("espera") == 0)
                    {
                        besp = true;
                    }
                    else if (word.CompareTo("marcaje") == 0)
                    {
                        bmrj = true;
                    }
                    else if (word.CompareTo("sucursal") == 0)
                    {
                        bsuc = true;
                    }
                }
            }
            if (tipo == 1)
                Application.Run(new SAIG(t_esp, t_mrj, sucursal));
            else if (tipo == 2)
                Application.Run(new CreaTrabajador());
            //Application.ExitThread();
            //GC.Collect();
            Application.Exit();
           


        }
    }
}
