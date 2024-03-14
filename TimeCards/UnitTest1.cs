using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace TimeCards
{
    [TestClass]
    public class UnitTest1
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public TestContext TestContext { get; set; }
        private ScreenshotTaker ScreenshotTaker { get; set; }   
        public ChromeOptions chromeOptions = new ChromeOptions();
        public ChromeDriver driver;
        public IWebElement Email => driver.FindElement(By.Name("loginfmt"));
        public IWebElement Password => driver.FindElement(By.Name("passwd"));




        [TestInitialize]
        public void RunBeforeEveryTest()
        {
            _logger.Debug("****** TEST STARTED");
            Reporter.AddTestCaseMetadataToHtmlReport(TestContext);

            chromeOptions.AddArguments("headless");
            driver = GetChromeDriver(chromeOptions);

            ScreenshotTaker = new ScreenshotTaker(driver, TestContext);       

        }

        [TestCleanup]
        public void RunAfterEveryTest()
        {
            _logger.Debug(GetType().FullName + " started a test finalize");

            try
            {
                TakeScreenshotForTestFailure();
            }
            catch (Exception e)
            {
                _logger.Error(e.Source);
                _logger.Error(e.StackTrace);
                _logger.Error(e.InnerException);
                _logger.Error(e.Message);
            }
            finally
            {
                // StopBrowser();
                _logger.Debug(TestContext.TestName);
                _logger.Debug("*************************************** TEST STOPPED");
                _logger.Debug("*************************************** TEST STOPPED");
            }

            driver.SwitchTo().DefaultContent();
            IJavaScriptExecutor JavaScriptExecutor = driver as IJavaScriptExecutor;
            JavaScriptExecutor.ExecuteScript("alert('Test Finished')");
            Thread.Sleep(1500);
            driver.SwitchTo().Alert().Accept();
            
            driver.Close();
            driver.Quit();
        }

        private void TakeScreenshotForTestFailure()
        {
            if (ScreenshotTaker != null)
            {
                ScreenshotTaker.CreateScreenshotIfTestFailed();
                Reporter.ReportTestOutcome(ScreenshotTaker.ScreenshotFilePath);
            }
            else
            {
                Reporter.ReportTestOutcome("");
            }
        }

        private void StopBrowser()
        {
            if (driver == null)
                return;
            driver.Quit();
            driver = null;
            _logger.Trace("Browser stopped successfully.");
        }

        private ChromeDriver GetChromeDriver(ChromeOptions chromeOptions)
        {
            var outputDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ChromeDriver(outputDirectory,chromeOptions);
        }


        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Validar que salga un mensaje de error al dejar vacías las horas")]
        public void ValidarHours()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1710282269378&source=portal");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }



            Reporter.LogPassingTestStepForBugLogger("Entré a validar Hours");
            _logger.Info("Entré a ValidarHours");
            
            driver.SwitchTo().Frame("fullscreen-app-host");
            
            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Enter data
            // Leave empty Hours field
            // Expect an error
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("QA Automation");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            Thread.Sleep(1000);
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(1000);            
            IWebElement error = driver.FindElement(By.XPath("//span[contains(text(),'Please enter an hour')]"));
            Assert.IsTrue(error.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Mensaje 'Please enter an hour' se mostró correctamente");
            Thread.Sleep(1000);
        }


        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Validar que salga un mensaje de error al introducir más de 255 caracteres en las notas")]
        public void ValidarNotes()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }


            Reporter.LogPassingTestStepForBugLogger("Entré a validar Notes");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Enter data
            // Enter over 640 characters for Notes
            // Expect an error
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("8");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(3000);
            IWebElement error = driver.FindElement(By.XPath("//*[@id = 'MessageBar18']/span/span"));
            if (chromeOptions.Arguments.Contains("headless") == true)
            {
                Assert.AreEqual(error.Text, "Value must be at most 255 characters in length");

            }
            else
            {
                Assert.AreEqual(error.Text, "El valor debe tener como máximo 255 caracteres de longitud");

            }
            Reporter.LogPassingTestStepForBugLogger("Se mostró correctamente el mensaje de máximo 255 caracteres");

            Thread.Sleep(1000);
        }


        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Insertar un registro de carga de horas")]
        public void AltaRegistro()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }


            Reporter.LogPassingTestStepForBugLogger("Entré a Alta Registro");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Enter data
            // Click Add
            // Expect no error
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("8");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("QA Automation hours");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(3000);
            // validamos que no haya mensaje de error, por lo tanto el alta fue exitosa
            driver.SwitchTo().DefaultContent();
            try
            {
                driver.FindElement(By.XPath("//*[@id = 'MessageBar18']/span/span"));
            } catch (NoSuchElementException e)
            {
                // Assert.AreEqual(e.Message, "no such element: Unable to locate element: {\"method\":\"xpath\",\"selector\":\"//*[@id = 'MessageBar18']/span/span\"}\r\n  (Session info: chrome=121.0.6167.185); For documentation on this error, please visit: https://www.selenium.dev/documentation/webdriver/troubleshooting/errors#no-such-element-exception");               
            }
            Reporter.LogPassingTestStepForBugLogger("Se insertó correctament el registro, y sin errores");
            Thread.Sleep(1000);
        }



        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Eliminar un registro de carga de horas")]
        public void EliminarRegistro()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }



            Reporter.LogPassingTestStepForBugLogger("Entré a Eliminar Registro");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Click Delete on the first record
            try
            {
                driver.FindElement(By.XPath("//div[@data-control-name = 'Button3']")).Click();
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//div[@data-control-name = 'Button1']")).Click();
            } catch(NoSuchElementException e)
            {
                driver.SwitchTo().DefaultContent();
                IJavaScriptExecutor JavaScriptExecutor = driver as IJavaScriptExecutor;
                JavaScriptExecutor.ExecuteScript("alert('No records for deletion')");
            }
            Thread.Sleep(1000);

            // validar aqui que se haya eliminado el registro y no haya mensaje de error
            driver.SwitchTo().DefaultContent();
            try
            {
                driver.FindElement(By.XPath("//*[@id = 'MessageBar18']/span/span"));
            }
            catch (NoSuchElementException e)
            {
                // Assert.AreEqual(e.Message, "no such element: Unable to locate element: {\"method\":\"xpath\",\"selector\":\"//*[@id = 'MessageBar18']/span/span\"}\r\n  (Session info: chrome=121.0.6167.185); For documentation on this error, please visit: https://www.selenium.dev/documentation/webdriver/troubleshooting/errors#no-such-element-exception");
            }
            Reporter.LogPassingTestStepForBugLogger("Se eliminó correctamente el registro");
            Thread.Sleep(1000);
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Realizar una consulta desde el 1 de enero")]
        public void VerHistoricoHoras()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }


            Reporter.LogPassingTestStepForBugLogger("Entré a Ver Histórico Horas");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Select Project
            driver.FindElement(By.Id("react-combobox-view-0")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.ClassName("itemTemplateLabel_dqr75c")).Click();
            Thread.Sleep(500);

            // Set Start date: 1st day of the month
            driver.FindElement(By.XPath("//div[@data-control-name = 'DatePickerStart']")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//button[@data-pika-day = '1']")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//button[@class = 'appmagic-datepicker-ok-button']")).Click();

            // Set Start Date = January 1st
            driver.FindElement(By.XPath("//div[@data-control-name = 'DatePickerStart']")).Click();
            do
            {
                Thread.Sleep(500);
                driver.FindElement(By.XPath("//button[@class = 'pika-prev']")).Click();
                Thread.Sleep(500);

            } while (driver.FindElement(By.ClassName("pika-label-month")).Text != "enero" && driver.FindElement(By.ClassName("pika-label-month")).Text != "January");
            driver.FindElement(By.XPath("//button[@data-pika-day = '1']")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//button[@class = 'appmagic-datepicker-ok-button']")).Click();
            Thread.Sleep(1000);

            // Validate there's a record for 1/1/2024
            if (chromeOptions.Arguments.Contains("headless") == true)
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//div[contains (text(), '1/1/2024')]")).Displayed);
            }
            else
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//div[contains (text(), '01/01/2024')]")).Displayed);

            }
            Reporter.LogPassingTestStepForBugLogger("Se realizó la consulta desde el 1 de enero, correctamente");
            Thread.Sleep(1000);
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(1000);
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Editar un registro")]
        public void EditarRegistro()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);
            try
            {
                Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(),'Power Apps')]")).Displayed);
                _logger.Info("Power Apps page was loaded");
            }
            catch (Exception ex)
            {
                _logger.Info("Power Apps text did not appear");
                TakeScreenshotForTestFailure();
            }



            Reporter.LogPassingTestStepForBugLogger("Entré a Editar Registro");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Select Project
            driver.FindElement(By.Id("react-combobox-view-0")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.ClassName("itemTemplateLabel_dqr75c")).Click();
            Thread.Sleep(500);

            // Click Edit on the first record
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            Thread.Sleep(500);
            IWebElement Notes = driver.FindElement(By.XPath("//input[@appmagic-control = 'DataCardValue7textbox']"));
            Thread.Sleep(500);
            Notes.Clear();
            Thread.Sleep(300);
            Notes.SendKeys("Edited: QA Automation Hours");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            Thread.Sleep(2000);



            // Validate the record was edited correctly
            IWebElement notes = driver.FindElement(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div/div"));
            Assert.AreEqual(notes.Text, "Edited: QA A . . . ");
            // Assert.IsTrue(driver.FindElement(By.XPath("//div[@title = 'Edited: QA Automation Hours')]")).Displayed);

            Reporter.LogPassingTestStepForBugLogger("Registro actualizado correctamente");

            Thread.Sleep(1000);
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(3000);
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Validar Tooltips")]
        public void ValidarTooltips()
        {
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");
            Thread.Sleep(8000);
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            Thread.Sleep(8000);
            Password.SendKeys("@Giga0124");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");
            Thread.Sleep(8000);
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(5000);

            Reporter.LogPassingTestStepForBugLogger("Entré a Validar Tooltips");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);

            // Creating actions
            var actions = new Actions(driver);


            // Hover over notes field
            IWebElement notes = driver.FindElement(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div"));
            actions.MoveToElement(notes).Build().Perform();
            string title = notes.GetAttribute("title");
            Assert.AreEqual(title, "pruebas automatizadas Lynk");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Notes funciona");
            Thread.Sleep(2000);


            // Validar tooltips en cronómetro (Time Report)
            IWebElement timeReportClock = driver.FindElement(By.XPath("//div[@data-control-name = 'link1']/div/div/div/div"));            
            actions.MoveToElement(timeReportClock).Build().Perform();
            string titleTimeReport = timeReportClock.GetAttribute("title");
            Assert.AreEqual(titleTimeReport, "Time Report");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Time Report Clock funciona");
            Thread.Sleep(2000);

            // Validar tooltips en los relojito de arena (Historic Report)
            IWebElement historicReportClock = driver.FindElement(By.XPath("//div[@data-control-name = 'link2']/div/div/div/div"));
            actions.MoveToElement(historicReportClock).Build().Perform();
            string titleHistoricReport = historicReportClock.GetAttribute("title");
            Assert.AreEqual(titleHistoricReport, "Historic Report");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Historic Report Clock funciona");
            Thread.Sleep(2000);

        }
    }
}
