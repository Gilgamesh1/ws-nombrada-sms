using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using APM.SENDSMS.TEST.WS_ServiceSMS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace APM.SENDSMS.TEST
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {

            ServiceSMSClient cliente = new ServiceSMSClient();
            ServiceSMSRequest request = new ServiceSMSRequest();
            ContactoDTO[] lista = { new ContactoDTO { NroBoleta = "NOM-047321", FechaNombrada = DateTime.Now, Turno = "15:00-23:00", CodTrabajador = 100, NombreTrabajador = "Luis Soto", Celular = "969772149", Mensaje = "Prueba SMS" } }; 
                                      //new ContactoDTO { NroBoleta = "NOM-047321", FechaNombrada = DateTime.Now, Turno = "15:00-23:00", CodTrabajador = 100, NombreTrabajador="Jonathan Ascencio", Celular = "956747351", Mensaje = "Mensajin" } };
            request.Aplicacion = "Nombrada";
            request.Usuario = "user_admin";
            request.ListaEnvio = lista;
            ServiceSMSResponse response = cliente.EnviarMensajeBatch(request);
            Console.WriteLine(response.Codigo);
            Console.WriteLine(response.Mensaje);
        }
    }
}
