using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluralsightCrawler
{
    class Program
    {
        static IWebDriver driver { get; set; }

        static void Main(string[] args)
        {
            ExtractCourse();
        }


        static void ExtractCourse()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://app.pluralsight.com/player?course=c-sharp-code-more-object-oriented&author=zoran-horvat&name=c-sharp-code-more-object-oriented-m0&clip=0&mode=live");
            //Listar todos os vídeos
            List<Curso> cursos = new List<Curso>();

            List<IWebElement> lstModules = new List<IWebElement>();
            //Captura os modulos
            int i = 1;
            while (i<900)
            {
                try
                {
                    IWebElement elem3 = driver.FindElement(By.XPath("//*[@id='tab-table-of-contents']/section[" + i + "]"));
                    lstModules.Add(elem3);

                    Curso curso = new Curso();
                    curso.TituloCurso = "Making Your C# Code More Object-oriented";

                    string titulomodulo = elem3.Text;//tratar
                    curso.TituloModulo = titulomodulo;
                    cursos.Add(curso);
                }
                catch
                {
                    break;
                }


                i++;
            }

            foreach(IWebElement element in lstModules)
            {
                element.Click();
            }




            List<IWebElement> lst2 = new List<IWebElement>();
            int modulo = 1;
            int item = 1;
            foreach (IWebElement element in lstModules)
            {
                while(item < 900)
                {
                    try
                    {
                        IWebElement elem3 = driver.FindElement(By.XPath($"//*[@id='tab-table-of-contents']/section[{modulo}]/ul/li[{item}]/h3"));
                        lst2.Add(elem3);
                    }
                    catch
                    {
                        item = 1;
                        break;
                    }
                    item++;
                }

                modulo++;

            }

            List<IWebElement> lstvideos = new List<IWebElement>();


            foreach(IWebElement e in lst2)
            {
                e.Click();
                Thread.Sleep(5000);
                IWebElement getVideo = driver.FindElement(By.TagName("video"));

                string link = getVideo.GetAttribute("src").ToString();

                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                driver.Navigate().GoToUrl(link);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.close();");
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                //Esperar o fim do download
                string[] filePaths = Directory.GetFiles(@"C:\Users\Fernando\Downloads", "*.*",
                                             SearchOption.TopDirectoryOnly);

                FileInfo videofile = new FileInfo(filePaths[0]);

                var teste = IsFileLocked(videofile);


            }


            //Separar os nomes e criar as pastas

        }



        static bool IsFileLocked(FileInfo file)
        {
            int attempts = 0;

            FileStream stream = null;
             again:
            try
            {
                
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                Thread.Sleep(1000);

                if (attempts < 120)
                {
                    goto again;
                }

                attempts++;
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
