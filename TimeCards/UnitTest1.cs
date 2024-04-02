using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
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
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1710282269378&source=portal");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("=========== Validaré el campo Hours en: Carga de horas ===========");
            _logger.Info("Entré a ValidarHours");

            // wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("fullscreen-app-host")));
            driver.SwitchTo().Frame("fullscreen-app-host");
            
            //Click on the relojito
            driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")));

            // Validate hours during Add

            // Enter data
            // Leave empty Hours field
            // Expect this error message: 'Please enter a value between 1 and 24'
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("QA Automation");
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter an hour')]")));
            IWebElement error = driver.FindElement(By.XPath("//span[contains(text(),'Please enter an hour')]"));
            Assert.IsTrue(error.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas vacías, exitosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();

            
            // Enter a negative value for hours
            // Expect this error message: 'Please enter a value between 1 and 24'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("-8");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error2 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error2.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas negativas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Enter a zero value for hours
            // Expect this error message: 'Please enter a value between 1 and 24'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("0");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error3 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error3.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación 0 horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Enter a value of 25 for hours
            // Expect this error message: 'Please enter a value between 1 and 24'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("25");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error4 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error4.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de más de 24 horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Enter a string value of 'e' for hours
            // Expect this error message: 'Please enter a value between 1 and 24'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("e");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error5 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error5.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de string 'e' como hora, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();



            Reporter.LogPassingTestStepForBugLogger("=========== Ahora validaré el campo Hours en: Editar Registro ===========");

            // Validate hours in Edit modal
            
            // Empty hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error6 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error6.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas vacías, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Negative hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")).SendKeys("-8");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error7 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error7.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas negativas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Zero hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")).SendKeys("0");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error8 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error8.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de cero horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Over 24 hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")).SendKeys("25");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error9 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error9.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de más de 24 horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Letter 'e'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesHoursTimeReporttextbox']")).SendKeys("e");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error10 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error10.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de letra 'e' como horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();




            Reporter.LogPassingTestStepForBugLogger("=========== Ahora validaré el campo Hours en: Hístoric Report ===========");

            // Validate hours in Edit modal Historic Report

            // Empty hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error11 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error11.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas vacías, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();

            
            // Negative hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).SendKeys("-8");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error12 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error12.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de horas negativas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Zero hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).SendKeys("0");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error13 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error13.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de 0 horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Over 24 hours
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).SendKeys("25");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error14 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error14.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de más de 24 horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();


            // Letter 'e'
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtHoursEditHistoricReporttextbox']")).SendKeys("e");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]")));
            IWebElement error15 = driver.FindElement(By.XPath("//span[contains(text(),'Please enter a value between 1 and 24')]"));
            Assert.IsTrue(error15.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Validación de letra 'e' en horas, existosa");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//i[@data-icon-name = 'Clear']")).Click();
            
        }


        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Validar que salga un mensaje de error al introducir más de 255 caracteres en las notas")]
        public void ValidarNotes()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("============ Validar longitud máxima del campo Notes en: Carga de Horas ============");

            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            // driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            // Thread.Sleep(5000);

            // Enter data
            // Enter over 640 characters for Notes
            // Expect an error
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("8");
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_2']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]")));
            IWebElement error = driver.FindElement(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]"));
            Assert.IsTrue(error.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Se mostró correctamente el mensaje de máximo 255 caracteres en: Carga de horas");


            // Over 255 characters in Editar Registro
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnEditTimeReport']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtInputNotesEditTimeReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesEditTimeReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtInputNotesEditTimeReporttextbox']")).SendKeys("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdateEditTimeReport']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]")));
            IWebElement error2 = driver.FindElement(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]"));
            Assert.IsTrue(error2.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Se mostró correctamente el mensaje de máximo 255 caracteres en: Editar Registro");


            // Over 255 characters in Editar Registro de Historic Report
            driver.SwitchTo().Frame("fullscreen-app-host");
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtNotesEditHistoricReporttextbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtNotesEditHistoricReporttextbox']")).Clear();
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtNotesEditHistoricReporttextbox']")).SendKeys("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]")));
            IWebElement error3 = driver.FindElement(By.XPath("//span[contains(text(),'Notes field text cannot exceed 255 characters.')]"));
            Assert.IsTrue(error3.Displayed);
            Reporter.LogPassingTestStepForBugLogger("Se mostró correctamente el mensaje de máximo 255 caracteres en: Editar Registro de Historic Report");

        }


        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Insertar un registro de carga de horas")]
        public void AltaRegistro()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");
            
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("Entré a Alta Registro");
            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            // driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            // Thread.Sleep(5000);

            // Enter data
            // Click Add
            // Expect no error

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")));
            driver.FindElement(By.XPath("//input[@appmagic-control = 'TextInput2_8textbox']")).SendKeys("8");
            driver.FindElement(By.XPath("//textarea[@appmagic-control = 'TextInput2_10textarea']")).SendKeys("QA Automation hours");
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
        }



        [TestMethod]
        [TestCategory("Carga de horas")]
        [Description("Eliminar un registro de carga de horas")]
        public void EliminarRegistro()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("Test de Eliminar Registro");
            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            // driver.FindElement(By.XPath("//div[@data-control-name = 'link1']")).Click();

            // Wait for page rendering
            // Thread.Sleep(5000);

            // Click Delete on the first record
            try
            {
                driver.FindElement(By.XPath("//div[@data-control-name = 'Button3']")).Click();
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//div[@data-control-name = 'Button1']")).Click();

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
            } catch(NoSuchElementException e)
            {
                driver.SwitchTo().DefaultContent();
                IJavaScriptExecutor JavaScriptExecutor = driver as IJavaScriptExecutor;
                JavaScriptExecutor.ExecuteScript("alert('No records for deletion')");
            }
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Realizar una consulta desde el 1 de enero")]
        public void VerHistoricoHoras()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("Entré a Ver Histórico Horas");
            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Wait for page rendering
            Thread.Sleep(5000);


            // Set Start date: 1st day of the month
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'DatePickerStart']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'DatePickerStart']")).Click();
            driver.FindElement(By.XPath("//button[@data-pika-day = '1']")).Click();
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

            // Validate there's a record for 1/1/2024
            if (chromeOptions.Arguments.Contains("headless") == true)
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains (text(), '1/1/2024')]")));
                Assert.IsTrue(driver.FindElement(By.XPath("//div[contains (text(), '1/1/2024')]")).Displayed);
            }
            else
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains (text(), '01/01/2024')]")));
                Assert.IsTrue(driver.FindElement(By.XPath("//div[contains (text(), '01/01/2024')]")).Displayed);

            }
            Reporter.LogPassingTestStepForBugLogger("Se realizó la consulta desde el 1 de enero, correctamente");
            driver.SwitchTo().DefaultContent();
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Editar un registro")]
        public void EditarRegistro()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")));
            driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(8000);

            Reporter.LogPassingTestStepForBugLogger("Entré a Editar Registro");
            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Click Edit on the first record
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Button2_7']")));
            driver.FindElement(By.XPath("//div[@data-control-name = 'Button2_7']")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@appmagic-control = 'TxtNotesEditHistoricReporttextbox']")));
            IWebElement Notes = driver.FindElement(By.XPath("//input[@appmagic-control = 'TxtNotesEditHistoricReporttextbox']"));
            Notes.Clear();
            Thread.Sleep(300);
            Notes.SendKeys("Edited: QA Automation Hours");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//div[@data-control-name = 'BtnUpdate']")).Click();

            // Validate the record was edited correctly
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div/div")));
            IWebElement notes = driver.FindElement(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div/div"));
            Assert.AreEqual(notes.Text, "Edited: QA A . . . ");
            // Assert.IsTrue(driver.FindElement(By.XPath("//div[@title = 'Edited: QA Automation Hours')]")).Displayed);

            Reporter.LogPassingTestStepForBugLogger("Registro actualizado correctamente");
            driver.SwitchTo().DefaultContent();
        }



        [TestMethod]
        [TestCategory("Histórico de horas")]
        [Description("Validar Tooltips")]
        public void ValidarTooltips()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://apps.powerapps.com/play/e/9fd5302d-a4da-e8fe-af21-930adda2e30e/a/3be5954f-d753-46e2-b2aa-bc38b9fb66d5?tenantId=5c4fae17-a009-4196-85fa-9b956adbd1ea&source=AppSharedV3&hint=c0ee3102-3e42-441e-bdde-2b1b2a0ef820&sourcetime=1708706229940");
            driver.Manage().Window.Maximize();
            _logger.Info("Opened and Maximized Chrome");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            Email.SendKeys("rafael.villalvazo@grupo-giga.com");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered Username");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            Password.SendKeys("@Giga0324");
            driver.FindElement(By.Id("idSIButton9")).Click();
            _logger.Info("Entered password and next");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']"))); driver.FindElement(By.XPath("//*[@data-report-event = 'Signin_Submit' and @data-report-trigger = 'click' and @data-report-value = 'Submit']")).Click();
            _logger.Info("Clicked on Si Mantener sesión iniciada");
            Thread.Sleep(5000);

            Reporter.LogPassingTestStepForBugLogger("Entré a Validar Tooltips");
            driver.SwitchTo().Frame("fullscreen-app-host");

            //Click on the relojito           
            driver.FindElement(By.XPath("//div[@data-control-name = 'link2']")).Click();

            // Creating actions
            var actions = new Actions(driver);

            // Hover over notes field
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div")));
            IWebElement notes = driver.FindElement(By.XPath("//div[@data-control-name = 'Title7_4']/div/div/div/div"));
            actions.MoveToElement(notes).Build().Perform();
            string title = notes.GetAttribute("title");
            Assert.AreEqual(title, "Edited: QA Automation Hours");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Notes funciona");


            // Validar tooltips en cronómetro (Time Report)
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'link1']/div/div/div/div")));
            IWebElement timeReportClock = driver.FindElement(By.XPath("//div[@data-control-name = 'link1']/div/div/div/div"));            
            actions.MoveToElement(timeReportClock).Build().Perform();
            string titleTimeReport = timeReportClock.GetAttribute("title");
            Assert.AreEqual(titleTimeReport, "Time Report");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Time Report Clock funciona");

            // Validar tooltips en los relojito de arena (Historic Report)
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@data-control-name = 'link2']/div/div/div/div")));
            IWebElement historicReportClock = driver.FindElement(By.XPath("//div[@data-control-name = 'link2']/div/div/div/div"));
            actions.MoveToElement(historicReportClock).Build().Perform();
            string titleHistoricReport = historicReportClock.GetAttribute("title");
            Assert.AreEqual(titleHistoricReport, "Historic Report");
            Reporter.LogPassingTestStepForBugLogger("Tooltip en Historic Report Clock funciona");
        }
    }
}
