using SIRA_Notificaciones.el;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using SIRA_Notificaciones.wsSira;
using Microsoft.Web.Services3.Security.Tokens;
namespace SIRA_Notificaciones.dal
{
    class siraPeticiones
    {
        Int32 estadoPeticion;
        //Int32 idOperacion;
        String xmlRequest;
        String xmlResponse;
        

        static OperacionEntradaService ws = new OperacionEntradaService();

        public siraPeticiones() {
            ws.Timeout = configuracionNotificador.timerOutWS;
            ws.Url = configuracionNotificador.urlWs;
            wsPolicy();
        }

        public void wsPolicy() {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ICredentials credentials = new NetworkCredential("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", "https://*");
            ws.Credentials = credentials;
            UsernameToken token = new UsernameToken("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", PasswordOption.SendPlainText);
            ws.SetClientCredential(token);
            ws.SetPolicy("ClientPolicy");
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        public void pruebaSimple()
        {
            wsSira.InformacionDeIngresoSimple objEntradaSimple = new wsSira.InformacionDeIngresoSimple();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            try
            {
                objEntradaSimple.informacionGeneral = new InformacionOperacionGeneral();
                objEntradaSimple.informacionGeneral.consecutivo             = "0001";
                objEntradaSimple.informacionGeneral.idAsociado              = "000000001";
                objEntradaSimple.informacionGeneral.fechaRegistro = Convert.ToDateTime("15/04/2021 07:00:00");
                objEntradaSimple.informacionGeneral.tipoMovimiento          = 1; //Ingreso simple
                objEntradaSimple.informacionGeneral.detalleMovimiento       = 3;//Maritimo
                objEntradaSimple.informacionGeneral.tipoOperacion           = 1;//Importacion
                objEntradaSimple.informacionGeneral.cveRecintoFiscalizado   = "0067";
                objEntradaSimple.informacionIngreso = new InformacionIngreso();
                objEntradaSimple.informacionIngreso.tipoMercancia           = 1;
                objEntradaSimple.informacionIngreso.fechaFinDescarga = Convert.ToDateTime("15/04/2021 21:14:00");
                objEntradaSimple.informacionIngreso.fechaInicioDescarga = Convert.ToDateTime("15/04/2021 08:30:00");
                objEntradaSimple.informacionIngreso.peso                    = 400;
                objEntradaSimple.informacionIngreso.condicionCarga          = 1;
                objEntradaSimple.informacionIngreso.observaciones           = "Prueba de envio WS al 15/04/2021";

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(objEntradaSimple);
             

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoSimple(objEntradaSimple);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

                estadoPeticion = 1;
            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
                estadoPeticion = 0;
            }

            Inserta_Notificador(Int32.Parse(objEntradaSimple.informacionGeneral.idAsociado), "ingresoSimple", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void IngresoSinID()
        {
            wsSira.InformacionDeIngresoSinIdAsociado objIngresoSinId = new wsSira.InformacionDeIngresoSinIdAsociado ();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ICredentials credentials = new NetworkCredential("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", "https://*");
            ws.Credentials = credentials;
            UsernameToken token = new UsernameToken("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", PasswordOption.SendPlainText);
            ws.SetClientCredential(token);
            ws.SetPolicy("ClientPolicy");
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************


                //----------Informacion general
                wsSira.InformacionOperacionGeneral objInfoGeneral = new wsSira.InformacionOperacionGeneral();
                objInfoGeneral.consecutivo      = "01";
                objInfoGeneral.idAsociado       = "0";
                objInfoGeneral.fechaRegistro    = Convert.ToDateTime("10/04/2021 10:00:00");
                objInfoGeneral.tipoIngreso      = "01";
                objInfoGeneral.tipoMovimiento   = 4; // Ingreso sin Id
                objInfoGeneral.detalleMovimiento = 3; //Maritimo
                objInfoGeneral.tipoOperacion    = 1; // Importacion
                objInfoGeneral.cveRecintoFiscalizado = "0067";                
                objIngresoSinId.informacionGeneral = objInfoGeneral;

                //----------Informacion ingreso

                objIngresoSinId.informacionIngreso = new InformacionIngreso();
                objIngresoSinId.informacionIngreso.tipoMercancia = 1;
                objIngresoSinId.informacionIngreso.fechaFinDescarga         = Convert.ToDateTime("10/04/2021 18:30:00");
                objIngresoSinId.informacionIngreso.fechaInicioDescarga  = Convert.ToDateTime("10/04/2021 18:30:00");
                objIngresoSinId.informacionIngreso.peso = 400;
                objIngresoSinId.informacionIngreso.condicionCarga = 1;
                objIngresoSinId.informacionIngreso.observaciones = "Prueba de envio WS IngresoSinId al 10/04/2021 10:16";

                //----------Transporte
                wsSira.InformacionDeTransporte objTransporte = new wsSira.InformacionDeTransporte();
                objTransporte.numeroVueloBuqueViajeImo = "1";
                objTransporte.fechaHoraDeArribo = Convert.ToDateTime("10/04/2021 11:00:00");
                objTransporte.tipoTransporte = "M";
                objTransporte.origenVueloBuque = "1";
                objTransporte.numeroManifiesto = "ABGH976285JJDFGW";
                objTransporte.caat = "6890";
                objTransporte.peso = 2800;
                objTransporte.ump = "KG";
                objTransporte.piezas = 280;

                objIngresoSinId.transporte = objTransporte;

                //----------Master
                wsSira.InformacionBlMaster objMater = new wsSira.InformacionBlMaster();
                objMater.numeroGuiaBl = "KLJMNDH763527";
                objMater.caat = "6890";
                objMater.tipoOperacion = 1; //1:Impo - 2:Expo
                objMater.valorDeclarado = 9800.0; //?
                objMater.moneda = "USD";
                objMater.peso = 8200.0;
                objMater.ump = "KG";
                objMater.volumen = 875.0;
                objMater.umv = "M3";
                objMater.piezas = 760;
                objMater.idParcialidad = "T";
                objMater.secuencia = 1;
                objMater.observaciones = "EJEMPLO DE 1 CONTENEDOR. 1.SECUENCIA. 2 HOUSES ";

                //----------Contenedor
                wsSira.InformacionContenedor[] objContenedor = new wsSira.InformacionContenedor[1];
                objContenedor[0] = new wsSira.InformacionContenedor();
                objContenedor[0].iniciales = "MNBV";
                objContenedor[0].numero = "6542";
                objContenedor[0].estado = "F";
                objContenedor[0].tipo = "B";

                //----------Contenedor --- Sellos
                wsSira.InformacionDeSellos[] objSellos = new wsSira.InformacionDeSellos[3];
                objSellos[0] = new wsSira.InformacionDeSellos();
                objSellos[0].candado = "s1";
                objSellos[1] = new wsSira.InformacionDeSellos();
                objSellos[1].candado = "s2";
                objSellos[2] = new wsSira.InformacionDeSellos();
                objSellos[2].candado = "s3";
                objContenedor[0].sello = objSellos;


                wsSira.InformacionDeMercancia[] objMercancias = new wsSira.InformacionDeMercancia[1];
                objMercancias[0] = new wsSira.InformacionDeMercancia();
                objMercancias[0].secuencia = 1;
                objMercancias[0].pais = "CAN";
                objMercancias[0].descripcion = "MERCANCIA MIELINA DE CANADA";
                objMercancias[0].imo = null;
                objMercancias[0].vin = null;
                objMercancias[0].valor = 2500.0;
                objMercancias[0].moneda = "USD";
                objMercancias[0].peso = 2000.0;
                objMercancias[0].ump = "KG";
                objMercancias[0].volumenSpecified = true;
                objMercancias[0].volumen = 350.0;
                objContenedor[0].mercancia = objMercancias;

                //----------Contenedor --- Personas
                wsSira.InformacionDePersonas[] objPersonas = new wsSira.InformacionDePersonas[1];
                objPersonas[0] = new wsSira.InformacionDePersonas();
                objPersonas[0].tipoPersona = "Master";
                objPersonas[0].nombre = "Bruce";
                objPersonas[0].calleDomicilio = "Calle siempre viva";
                objPersonas[0].cp = "02532";
                objPersonas[0].municipio = "Montreal";
                objPersonas[0].entidadFederativa = "CAN";
                objPersonas[0].pais = "CAN";
                objPersonas[0].rfcOTaxId = "XX25621RFT";
                objPersonas[0].correoElectronico = "bruce@gmail.com";
                objPersonas[0].ciudad = "CAN";
                objPersonas[0].contacto = "YISUS CORP";
                objPersonas[0].telefono = 55251524;
                objContenedor[0].personas = objPersonas;

                //----------Contenedor --- Guia House
                wsSira.InformacionGuiaHouse[] listaHouse = new wsSira.InformacionGuiaHouse[2];
                listaHouse[0] = new wsSira.InformacionGuiaHouse();
                listaHouse[0].numeroGuiaBl = "KLJMNDH763527-1";
                listaHouse[0].caat = "6890";
                listaHouse[0].tipoOperacion = 1; //1:Impo - 2:Expo
                listaHouse[0].ump = "KG";

                listaHouse[1] = new wsSira.InformacionGuiaHouse();
                listaHouse[1].numeroGuiaBl = "KLJMNDH763527-2";
                listaHouse[1].caat = "2340";
                listaHouse[1].tipoOperacion = 1; //1:Impo - 2:Expo
                listaHouse[1].ump = "KG";

                objContenedor[0].guiaHouse = listaHouse;
                objMater.contenedor = objContenedor;
                objIngresoSinId.guiaMaster = objMater;



                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(objIngresoSinId);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoNoManifestado(objIngresoSinId);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(objIngresoSinId.informacionGeneral.idAsociado), "Ingreso Sin ID", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void IngresoSimple()
        {
            InformacionDeIngresoSimple objEntradaSimple = new InformacionDeIngresoSimple();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();


            try
            {

                //*************************************************
                //   Consulta BD 
                //*************************************************
                

                objEntradaSimple.informacionGeneral = new wsSira.InformacionOperacionGeneral();
                objEntradaSimple.informacionGeneral.idAsociado = "642";
                objEntradaSimple.informacionGeneral.fechaRegistro = Convert.ToDateTime("20/09/2018 18:30:00");
                objEntradaSimple.informacionGeneral.tipoMovimiento = 1; //Ingreso simple
                objEntradaSimple.informacionGeneral.detalleMovimiento = 3;//Maritimo
                objEntradaSimple.informacionGeneral.tipoOperacion = 1;//Importacion
                objEntradaSimple.informacionGeneral.cveRecintoFiscalizado = "67";

                objEntradaSimple.informacionIngreso = new wsSira.InformacionIngreso();
                objEntradaSimple.informacionIngreso.tipoMercancia = 1;
                objEntradaSimple.informacionIngreso.fechaFinDescarga = Convert.ToDateTime("20/09/2018 18:30:00");
                objEntradaSimple.informacionIngreso.fechaInicioDescarga = Convert.ToDateTime("20/09/2018 18:30:00");
                objEntradaSimple.informacionIngreso.peso = 400;

                objEntradaSimple.informacionIngreso.condicionCarga = 1;
                objEntradaSimple.informacionIngreso.observaciones = "Sin observaciones";
                

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(objEntradaSimple);
                

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoSimple(objEntradaSimple);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

                estadoPeticion = 1;
            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
                estadoPeticion = 0;
            }

            Inserta_Notificador(Int32.Parse(objEntradaSimple.informacionGeneral.idAsociado), "ingresoSimple", "Request", xmlRequest, xmlResponse, estadoPeticion);
                                    
        }

        public void IngresoParcial()
        {
            wsSira.InformacionDeIngresoParcial objEntradaParcial = new wsSira.InformacionDeIngresoParcial();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************

                objEntradaParcial.informacionGeneral = new wsSira.InformacionOperacionGeneral();
                objEntradaParcial.informacionGeneral.consecutivo        = "000000001";
                objEntradaParcial.informacionGeneral.idAsociado         = "000000001";
                objEntradaParcial.informacionGeneral.fechaRegistro      = Convert.ToDateTime("06/02/2020 10:00:00");
                objEntradaParcial.informacionGeneral.tipoMovimiento     = 3; //Ingreso parcial
                objEntradaParcial.informacionGeneral.detalleMovimiento = 3;//Maritimo
                objEntradaParcial.informacionGeneral.tipoOperacion      = 1;//Importacion
                objEntradaParcial.informacionGeneral.cveRecintoFiscalizado = "067";

                objEntradaParcial.informacionIngresoParcial = new wsSira.InformacionIngresoParcial();
                objEntradaParcial.informacionIngresoParcial.tipoMercancia = 1;
                objEntradaParcial.informacionIngresoParcial.fechaInicioDescarga = Convert.ToDateTime("05/02/2020 18:30:00");
                objEntradaParcial.informacionIngresoParcial.fechaFinDescarga = Convert.ToDateTime("05/02/2020 18:30:00");
                objEntradaParcial.informacionIngresoParcial.peso = 400;  //En kilos

                objEntradaParcial.informacionIngresoParcial.cantidadSpecified = true;
                objEntradaParcial.informacionIngresoParcial.cantidad = 400;

                objEntradaParcial.informacionIngresoParcial.condicionCarga = 1; //Revisar catalogo [Estados de la carga] 
                objEntradaParcial.informacionIngresoParcial.observaciones = "Prueba de Ingreso parcial al 06/02/2020";
                //objEntradaParcial.informacionIngresoParcial.totalParcialidadesSpecified = true;
                //objEntradaParcial.informacionIngresoParcial.totalParcialidades = 4; //Sin o se conoce se manda en la ultima parcialidad
                objEntradaParcial.informacionIngresoParcial.numeroParcialidad = 1; //Consecutivo del Recinto
                                
                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(objEntradaParcial);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoParcial(objEntradaParcial);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(objEntradaParcial.informacionGeneral.idAsociado), "ingreso Parcial", "Request", xmlRequest, xmlResponse, estadoPeticion);            
        }



        //Se recibe un obejto y se parsea a XML
        public String ParseObjeto(object pObjeto)
        {
            string xml;

            try
            {
                var serxml = new System.Xml.Serialization.XmlSerializer(pObjeto.GetType());
                var ms = new MemoryStream();
                serxml.Serialize(ms, pObjeto);
                xml = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                xml = null;
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraPeticiones.parseObjeto", ex.Message);
                Log.Guardar("xml", xml);
                #endregion
            }

            return xml;
        }

        public void Inserta_Notificador(Int32 pIdOperacion,String pEvento, String pDescripcion, String pXmlRequest, String pXmlResponse, Int32 pStatus)
        {
            try
            {
                siraHandlerDAL objHandler = new siraHandlerDAL();
                DataTable dtResult = new DataTable();

                dtResult = objHandler.InsertaSiraNotificador(pIdOperacion,pEvento, pDescripcion, pXmlRequest, pXmlResponse, pStatus);
            }
            catch (Exception ex)
            {
                #region ACTUALIZANDO LOG CON ERROR
                log Log = new log();
                Log.Guardar("siraPeticiones.Inserta_Notificador", ex.Message);
                #endregion
            }
        }

    }
}
