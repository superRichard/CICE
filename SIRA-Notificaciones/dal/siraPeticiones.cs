using SIRA_Notificaciones.el;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using SIRA_Notificaciones.wsSira;
using Microsoft.Web.Services3.Security.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SIRA_Notificaciones.dal
{
    class siraPeticiones
    {
        #region DECLARACIONES Y CONSTANTES
        Int32 estadoPeticion;
        String xmlRequest;
        String xmlResponse;        

        static OperacionEntradaService ws = new OperacionEntradaService();

        //constantes
        static string cClaveRecinto = "4304"; // de acuerdo a catalogo CAMIR

        //enumerados
        enum cEnumTipoOperacion
        {
            Importacion = 1,
            Exportacion = 2,
            Transbordo  = 3,
            Cabotaje    = 4,
            ContenedorVacio = 5
        };

        enum cEnumTipoMovimiento
        {
            NotificacionesDeArribo = 0,
            IngresoSimple = 1,
            IngresoParcial = 3,
            IngresoMercancíaNomanifestado = 4,
            Traspaleo = 5,
            Desconsolidacion = 6,
            Consolidacion = 7,
            Rechazo = 8,
            Subdivision = 9,
            Incidencia = 10,
            Salidas = 11,
            CambioEmbalaje = 12,
            ReconocimientoPrevio = 13,
            AvisoRF = 16,
            Transferencia = 17,
            Cancelaciones = 18
        };

        enum cEnumDetalleMovimiento
        {
            IngresoSimpleManifiestoFerroviario = 1,
            IngresoSimpleManifiestoAereo = 2,
            IngresoSimpleManifiestoMaritimo = 3,
            IngresoSimpleAutoridadPedimento = 4,
            IngresoSimpleAutoridadEdocto = 5,
            IngresoSimpleutoridadSecuencia = 6,
            IngresoSimpleAutoridadContenedor = 7,
            IngresoSimplePedimento = 8,
            IngresoSimpleReingreso = 9,
            IngresoSimpleTransferencia = 10,
            IngresoSimpleAnexo29 = 11,
            IngresoSimpleContenedorVacio = 12,
            IngresoSimpleAutoridadBL = 13,
            IngresoSimpleDesconsolidacion = 14
        };

        enum cEnumTipoMercancia
        {
            dias_3  = 1,
            dias_45 = 2,
            dias_60 = 3
        }
        enum cEnumCondicionCarga
        {
            CargaEnOptimasCondiciones = 1,
            CargaMojada             = 2,
            CargaDannada            = 3
        }
        #endregion

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

        public void ingresoSimple()
        {
            wsSira.InformacionDeIngresoSimple EntradaSimple = new wsSira.InformacionDeIngresoSimple();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            wsPolicy();

            /*using (OperationContextScope scope = new OperationContextScope((IContextChannel)objEntradaSimple))
            {
                MessageHeader aMessageHeader = MessageHeader.CreateHeader("MyTimestamp", "http://tempuri.org", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);
            }*/

            try
            {
                EntradaSimple.informacionGeneral = new InformacionOperacionGeneral();
                EntradaSimple.informacionGeneral.consecutivo             = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                EntradaSimple.informacionGeneral.idAsociado              = "000000001";
                EntradaSimple.informacionGeneral.fechaRegistro           = DateTime.Now;
                EntradaSimple.informacionGeneral.tipoOperacion           = (long)cEnumTipoOperacion.Importacion;
                EntradaSimple.informacionGeneral.tipoMovimiento          = (long)cEnumTipoMovimiento.IngresoSimple;     // Ingreso simple
                EntradaSimple.informacionGeneral.detalleMovimiento       = (long)cEnumDetalleMovimiento.IngresoSimpleManifiestoMaritimo;
                EntradaSimple.informacionGeneral.cveRecintoFiscalizado   = cClaveRecinto;

                #region ingreso
                EntradaSimple.informacionIngreso = new InformacionIngreso();
                EntradaSimple.informacionIngreso.tipoMercancia           = (long)cEnumTipoMercancia.dias_3; 
                EntradaSimple.informacionIngreso.fechaInicioDescarga     = DateTime.Now.AddMinutes(-120);
                EntradaSimple.informacionIngreso.fechaFinDescarga        = DateTime.Now.AddMinutes(-60);
                EntradaSimple.informacionIngreso.peso                    = 400;
                EntradaSimple.informacionIngreso.condicionCarga          = (long)cEnumCondicionCarga.CargaEnOptimasCondiciones ;
                EntradaSimple.informacionIngreso.observaciones           = "PRUEBA DE INGRESO SIMPLE idasociado[000000001] RECINTO 067 (4304 CAMIR) CORPORACION INTEGRAL DE COMERCIO EXTERIOR, S.C.";
                #endregion 

                #region contenedores
                int c =1;
                wsSira.Contenedor[] contenedores = new wsSira.Contenedor[c];
                contenedores[0] = new wsSira.Contenedor();
                contenedores[0].iniciales = "CAXU123456";
                contenedores[0].numero = "NUMERO1";
                #endregion

                #region guias house
                int g = 10;
                string[] guias = new string[g];
                guias[0] = "GUIA00";
                guias[1] = "GUIA01";
                guias[2] = "GUIA02";
                guias[3] = "GUIA03";
                guias[4] = "GUIA04";
                guias[5] = "GUIA05";
                guias[6] = "GUIA06";
                guias[7] = "GUIA07";
                guias[8] = "GUIA08";
                guias[9] = "GUIA09";
                EntradaSimple.guiasHouse = guias;
                #endregion

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(EntradaSimple);
             

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoSimple(EntradaSimple);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

                estadoPeticion = 1;
            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
                estadoPeticion = 0;
            }

            Inserta_Notificador(Int32.Parse(EntradaSimple.informacionGeneral.idAsociado), "ingresoSimple", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void ingresoSinID()
        {
            wsSira.InformacionDeIngresoSinIdAsociado IngresoSinId = new wsSira.InformacionDeIngresoSinIdAsociado ();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            wsPolicy();

            /*
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ICredentials credentials = new NetworkCredential("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", "https://*");
            ws.Credentials = credentials;
            UsernameToken token = new UsernameToken("MAR870122MX9", "i1yzMzAa3RvVgMzNAmTnL0hvVmSRTYDOpmuTrO0+REFMnCTj+k+LFHmZRtgkMkEq", PasswordOption.SendPlainText);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            */
            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************


                #region Informacion general
                wsSira.InformacionOperacionGeneral InfoGeneral = new wsSira.InformacionOperacionGeneral();
                InfoGeneral.consecutivo          = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                InfoGeneral.idAsociado           = "000000001";
                InfoGeneral.fechaRegistro        = DateTime.Now;
                InfoGeneral.tipoIngreso          = "M"; // H= Ingreso parcial por House | M = Ingreso parcial por Master

                InfoGeneral.tipoMovimiento       = (long)cEnumTipoMovimiento.IngresoMercancíaNomanifestado; ; // Ingreso sin Id
                InfoGeneral.detalleMovimiento    = (long)cEnumDetalleMovimiento.IngresoSimpleManifiestoMaritimo; ; //Maritimo
                InfoGeneral.tipoOperacion        = (long)cEnumTipoOperacion.Importacion; // 1= Importación| 2 = Exportación| 3 = Transbordo| 4 = Cabotaje |5 = Contenedor Vacío
                InfoGeneral.cveRecintoFiscalizado = cClaveRecinto;
                IngresoSinId.informacionGeneral   = InfoGeneral;
                #endregion 

                #region Informacion ingreso
                IngresoSinId.informacionIngreso = new InformacionIngreso();
                IngresoSinId.informacionIngreso.tipoMercancia           = (long)cEnumTipoMercancia.dias_3 ;   // 1 = 3 días | 2 = 45 días | 3 = 60 días
                IngresoSinId.informacionIngreso.fechaInicioDescarga     = DateTime.Now.AddMinutes(-120);
                IngresoSinId.informacionIngreso.fechaFinDescarga        = DateTime.Now.AddMinutes(-60);
                IngresoSinId.informacionIngreso.peso                    = 400;
                IngresoSinId.informacionIngreso.condicionCarga          = (long)cEnumCondicionCarga.CargaEnOptimasCondiciones;   // 1=Carga en óptimas condiciones 2 = Carga Mojada  3 = Carga dañada
                IngresoSinId.informacionIngreso.observaciones           = "PRUEBA DE INGRESO SIN ID idasociado[000000001] RECINTO 067 (4304 CAMIR) CORPORACION INTEGRAL DE COMERCIO EXTERIOR, S.C.";
                #endregion 

                #region Transporte
                wsSira.InformacionDeTransporte Transporte = new wsSira.InformacionDeTransporte();
                Transporte.numeroVueloBuqueViajeImo = "1";
                Transporte.fechaHoraDeArribo        = DateTime.Now.AddMinutes(-80);
                Transporte.tipoTransporte           = "M";
                Transporte.origenVueloBuque         = "1";
                Transporte.numeroManifiesto         = "ABGH976285JJDFGW";
                Transporte.caat                     = "6890";
                Transporte.peso                     = 2800;
                Transporte.ump                      = "K";        // “E” Toneladas (Metric Ton) | “K” Kilos(Kilograms) | “L” Libras(Pounds)
                Transporte.piezas                   = 280;
                IngresoSinId.transporte             = Transporte;
                #endregion 

                #region Master
                wsSira.InformacionBlMaster Mater = new wsSira.InformacionBlMaster();
                Mater.numeroGuiaBl          = "KLJMNDH763527";
                Mater.caat                  = "6890";
                Mater.tipoOperacion         = (long)cEnumTipoOperacion.Importacion; // 1= Importación| 2 = Exportación| 3 = Transbordo| 4 = Cabotaje |5 = Contenedor Vacío
                Mater.valorDeclarado        = 9800.0; //?
                Mater.moneda                = "USD";
                Mater.peso                  = 8200.0;
                Mater.ump                   = "K";
                Mater.volumen               = 875.0;
                Mater.umv                   = "M3";
                Mater.piezas                = 760;
                Mater.idParcialidad         = "T";  // S = Split | T = Total
                Mater.secuencia             = 1;
                Mater.observaciones = "EJEMPLO DE 1 CONTENEDOR. 1.SECUENCIA. 2 HOUSES ";

                //Contenedores
                wsSira.InformacionContenedor[] Contenedores = new wsSira.InformacionContenedor[1];
                Contenedores[0] = new wsSira.InformacionContenedor();
                Contenedores[0].iniciales   = "MNBV";
                Contenedores[0].numero      = "6542";
                Contenedores[0].estado      = "F";
                Contenedores[0].tipo        = "B";

                // Contenedor --- Sellos
                wsSira.InformacionDeSellos[] objSellos = new wsSira.InformacionDeSellos[3];
                objSellos[0] = new wsSira.InformacionDeSellos();
                objSellos[0].candado = "sello01";
                objSellos[1] = new wsSira.InformacionDeSellos();
                objSellos[1].candado = "sello02";
                objSellos[2] = new wsSira.InformacionDeSellos();
                objSellos[2].candado = "sello03";
                Contenedores[0].sello = objSellos;

                // Mercancias
                wsSira.InformacionDeMercancia[] Mercancias = new wsSira.InformacionDeMercancia[1];
                Mercancias[0] = new wsSira.InformacionDeMercancia();
                Mercancias[0].secuencia     = 1;
                Mercancias[0].pais          = "CAN";
                Mercancias[0].descripcion   = "MERCANCIA MIELINA DE CANADA";
                Mercancias[0].imo           = null;
                Mercancias[0].vin           = null;
                Mercancias[0].valor         = 2500.0;
                Mercancias[0].moneda        = "USD";
                Mercancias[0].peso          = 2000.0;
                Mercancias[0].ump           = "K";
                Mercancias[0].volumenSpecified = true;
                Mercancias[0].volumen       = 350.0;
                Contenedores[0].mercancia = Mercancias;

                //Contenedor --- Personas
                wsSira.InformacionDePersonas[] Personas = new wsSira.InformacionDePersonas[1];
                Personas[0] = new wsSira.InformacionDePersonas();
                Personas[0].tipoPersona     = "SH"; //Shipper   Embarcador
                Personas[0].nombre          = "Bruce";
                Personas[0].calleDomicilio  = "Calle siempre viva";
                Personas[0].cp              = "02532";
                Personas[0].municipio       = "Montreal";
                Personas[0].entidadFederativa = "CAN";
                Personas[0].pais            = "CAN";
                Personas[0].rfcOTaxId = "XX25621RFT";
                Personas[0].correoElectronico = "bruce@gmail.com";
                Personas[0].ciudad      = "CAN";
                Personas[0].contacto    = "YISUS CORP";
                Personas[0].telefono = 55251524;
                Contenedores[0].personas = Personas;

                //Contenedor --- Guia House
                wsSira.InformacionGuiaHouse[] GuiasHouse = new wsSira.InformacionGuiaHouse[2];
                GuiasHouse[0] = new wsSira.InformacionGuiaHouse();
                GuiasHouse[0].numeroGuiaBl = "KLJMNDH763527-1";
                GuiasHouse[0].caat = "6890";
                GuiasHouse[0].tipoOperacion = 1; //1:Impo - 2:Expo
                GuiasHouse[0].ump = "K";

                GuiasHouse[1] = new wsSira.InformacionGuiaHouse();
                GuiasHouse[1].numeroGuiaBl = "KLJMNDH763527-2";
                GuiasHouse[1].caat = "2340";
                GuiasHouse[1].tipoOperacion = 1; //1:Impo - 2:Expo
                GuiasHouse[1].ump = "KG";
                #endregion

                Contenedores[0].guiaHouse = GuiasHouse;
                Mater.contenedor = Contenedores;
                IngresoSinId.guiaMaster = Mater;

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(IngresoSinId);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoNoManifestado(IngresoSinId);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(IngresoSinId.informacionGeneral.idAsociado), "Ingreso Sin ID", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void ingresoParcial()
        {
            wsSira.InformacionDeIngresoParcial EntradaParcial = new wsSira.InformacionDeIngresoParcial();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            wsPolicy();

            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************
                #region generales
                EntradaParcial.informacionGeneral = new wsSira.InformacionOperacionGeneral();
                EntradaParcial.informacionGeneral.consecutivo        = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                EntradaParcial.informacionGeneral.idAsociado         = "000000001";
                EntradaParcial.informacionGeneral.fechaRegistro      = DateTime.Now;
                EntradaParcial.informacionGeneral.tipoMovimiento     = (long)cEnumTipoMovimiento.IngresoParcial; //Ingreso parcial
                EntradaParcial.informacionGeneral.detalleMovimiento  = 3; //Maritimo
                EntradaParcial.informacionGeneral.tipoOperacion      = (long)cEnumTipoOperacion.Importacion;
                EntradaParcial.informacionGeneral.cveRecintoFiscalizado = cClaveRecinto;
                #endregion

                #region ingreso parcial
                EntradaParcial.informacionIngresoParcial = new wsSira.InformacionIngresoParcial();
                EntradaParcial.informacionIngresoParcial.tipoMercancia       = (long)cEnumTipoMercancia.dias_3;
                EntradaParcial.informacionIngresoParcial.fechaInicioDescarga = DateTime.Now.AddMinutes(-120);
                EntradaParcial.informacionIngresoParcial.fechaFinDescarga    = DateTime.Now.AddMinutes(-60);
                EntradaParcial.informacionIngresoParcial.peso                = 400;  //En kilos
                EntradaParcial.informacionIngresoParcial.cantidadSpecified   = true;
                EntradaParcial.informacionIngresoParcial.cantidad            = 400;
                EntradaParcial.informacionIngresoParcial.condicionCarga      = (long)cEnumCondicionCarga.CargaEnOptimasCondiciones;
                EntradaParcial.informacionIngresoParcial.cantidadSpecified   = true;
                EntradaParcial.informacionIngresoParcial.numeroParcialidad   = 1; //Consecutivo del Recinto
                EntradaParcial.informacionIngresoParcial.observaciones      = "PRUEBA DE INGRESO PARCIAL idasociado[000000001] RECINTO 067 (4304 CAMIR) CORPORACION INTEGRAL DE COMERCIO EXTERIOR, S.C.";
                #endregion

                #region guias house
                wsSira.GuiasHouseParcial guiasHouseParcial = new wsSira.GuiasHouseParcial();
                guiasHouseParcial.cantidad = 1;
                guiasHouseParcial.condicionCarga = (long)cEnumCondicionCarga.CargaEnOptimasCondiciones;
                guiasHouseParcial.fechaInicioDescarga = DateTime.Now.AddMinutes(-120);
                guiasHouseParcial.fechaFinDescarga = DateTime.Now.AddMinutes(-60);
                guiasHouseParcial.guiaHouse = "GUIA00";
                guiasHouseParcial.numeroParcialidad = 1;
                guiasHouseParcial.peso = 100;
                guiasHouseParcial.tipoMercancia = (long)cEnumTipoMercancia.dias_3;
                guiasHouseParcial.observaciones = "PRUEBA DE INGRESO PARCIAL idasociado[000000001] RECINTO 067 (4304 CAMIR) CORPORACION INTEGRAL DE COMERCIO EXTERIOR, S.C.";
                EntradaParcial.guiasHouse = guiasHouseParcial;
                #endregion

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(EntradaParcial);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresoParcial(EntradaParcial);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(EntradaParcial.informacionGeneral.idAsociado), "ingreso Parcial", "Request", xmlRequest, xmlResponse, estadoPeticion);            
        }

        public void ingresosParciales()
        {
            wsSira.InformacionDeIngresosParciales IngresosParciales = new wsSira.InformacionDeIngresosParciales();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();
            
            wsPolicy();

            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************
                #region generales
                IngresosParciales.informacionGeneral = new wsSira.InformacionOperacionGeneral();
                IngresosParciales.informacionGeneral.consecutivo          = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                IngresosParciales.informacionGeneral.idAsociado           = "000000001";
                IngresosParciales.informacionGeneral.fechaRegistro        = DateTime.Now;
                IngresosParciales.informacionGeneral.tipoMovimiento       = (long)cEnumTipoMovimiento.IngresoParcial; //Ingreso parcial
                IngresosParciales.informacionGeneral.detalleMovimiento    = 3;//Maritimo
                IngresosParciales.informacionGeneral.tipoOperacion        = (long)cEnumTipoOperacion.Importacion;
                IngresosParciales.informacionGeneral.cveRecintoFiscalizado = cClaveRecinto;
                #endregion

                #region parciales
                IngresosParciales.informacionIngresoParcial = new wsSira.InformacionIngresosParciales ();
                IngresosParciales.informacionIngresoParcial.tipoMercancia = (long)cEnumTipoMercancia.dias_3;
                IngresosParciales.informacionIngresoParcial.fechaInicioDescarga = DateTime.Now.AddMinutes(-120);
                IngresosParciales.informacionIngresoParcial.fechaFinDescarga    = DateTime.Now.AddMinutes(-60);
                IngresosParciales.informacionIngresoParcial.cantidad            = 400;  //En kilos
                IngresosParciales.informacionIngresoParcial.condicionCarga      = (long)cEnumCondicionCarga.CargaEnOptimasCondiciones;
                IngresosParciales.informacionIngresoParcial.numeroParcialidad   = 1;
                IngresosParciales.informacionIngresoParcial.observaciones       = "PRUEBA DE INGRESOa PARCIALES idasociado[000000001] RECINTO 067 (4304 CAMIR) CORPORACION INTEGRAL DE COMERCIO EXTERIOR, S.C.";
                IngresosParciales.informacionIngresoParcial.cantidadSpecified   = true;
                IngresosParciales.informacionIngresoParcial.cantidad            = 400;
                #endregion

                #region vins                
                int g = 1;
                string[] vins = new string[g];
                vins[0] = "JH4DA9360NS008662";
                IngresosParciales.informacionIngresoParcial.vins = vins;
                #endregion

                #region mercancias
                int m = 1;
                wsSira.ParcialesMercancia[] Mercancias = new wsSira.ParcialesMercancia[m];
                Mercancias[0] = new wsSira.ParcialesMercancia();
                Mercancias[0].numPiezas = 1;
                Mercancias[0].peso      = 400;
                Mercancias[0].secuencia = 1;
                Mercancias[0].umc       = "1";
                Mercancias[0].ump       = "K";
                IngresosParciales.informacionIngresoParcial.mercancia = Mercancias;
                #endregion

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(IngresosParciales);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.ingresosParciales (IngresosParciales);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(IngresosParciales.informacionGeneral.idAsociado), "ingresos Parciales", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void confirmacionDeSalida()
        {
            wsSira.InformacionSalidaPedimento  SalidaPedimento = new wsSira.InformacionSalidaPedimento();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();

            wsPolicy();

            try
            {

                //*************************************************
                //                    Consulta BD
                //*************************************************
                SalidaPedimento.tipoSalida          = "M";           //M = Master  | H = House  |C = Contenedor
                #region general
                SalidaPedimento.informacionGeneral = new wsSira.InformacionOperacionGeneralSalidas();
                SalidaPedimento.informacionGeneral.consecutivo = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                SalidaPedimento.informacionGeneral.idAsociado           = "000000001";
                SalidaPedimento.informacionGeneral.fechaRegistro        = DateTime.Now;
                SalidaPedimento.informacionGeneral.tipoMovimiento       = (long)cEnumTipoMovimiento.Salidas;
                SalidaPedimento.informacionGeneral.detalleMovimiento    = 2;//Salidas - Pedimento
                SalidaPedimento.informacionGeneral.tipoOperacion        = (long)cEnumTipoOperacion.Importacion;
                SalidaPedimento.informacionGeneral.cveRecintoFiscalizado = cClaveRecinto;
                #endregion

                #region master
                SalidaPedimento.master = new wsSira.SalidaGuiaMaster();
                SalidaPedimento.master.aduana               = "430";
                SalidaPedimento.master.nIntregracion        = "1";
                SalidaPedimento.master.numMaster            = "1";
                SalidaPedimento.master.partes2              = "2";
                SalidaPedimento.master.patente              = "1805";
                SalidaPedimento.master.pedimento            = "210000001";
                SalidaPedimento.master.remesa               = "1";
                #endregion

                #region contenedor
                SalidaPedimento.master.contenedor = new wsSira.SalidaContenedor();
                SalidaPedimento.master.contenedor.iniciales = "CAXU1234567890";
                SalidaPedimento.master.contenedor.mercancia             = new wsSira.SalidaMercancia();
                SalidaPedimento.master.contenedor.mercancia.numPiezas   = 1;
                SalidaPedimento.master.contenedor.mercancia.peso        = 1;
                SalidaPedimento.master.contenedor.mercancia.secuencia   = 1;
                SalidaPedimento.master.contenedor.mercancia.umc         = "1";
                SalidaPedimento.master.contenedor.mercancia.ump         = "K";
                SalidaPedimento.master.contenedor.numero                = 99;

                //mercancia
                int mm = 1;
                wsSira.SalidaMercancia[] MercanciasMaster = new wsSira.SalidaMercancia[mm];
                MercanciasMaster[0] = new wsSira.SalidaMercancia();
                MercanciasMaster[0] = new wsSira.SalidaMercancia();
                MercanciasMaster[0].numPiezas = 1;
                MercanciasMaster[0].peso = 1;
                MercanciasMaster[0].secuencia = 1;
                MercanciasMaster[0].secuenciaSpecified = true;
                MercanciasMaster[0].umc = "1";
                MercanciasMaster[0].ump = "K";
                SalidaPedimento.master.mercancia = MercanciasMaster;
                #endregion

                #region house
                int h = 1;
                wsSira.SalidaHouse[] Houses = new wsSira.SalidaHouse[h];
                Houses[0] = new wsSira.SalidaHouse();
                Houses[0].numHouse = "GUIA01";
                
                //mercancia
                int mh = 1;
                wsSira.SalidaMercancia [] MercanciasHouse = new wsSira.SalidaMercancia[mh];
                MercanciasHouse[0] = new wsSira.SalidaMercancia();
                MercanciasHouse[0].numPiezas = 1;
                MercanciasHouse[0].peso = 1;
                MercanciasHouse[0].secuencia = 1;
                MercanciasHouse[0].secuenciaSpecified = true;
                MercanciasHouse[0].umc = "1";
                MercanciasHouse[0].ump = "K";

                Houses[0].mercancia = MercanciasHouse;
                SalidaPedimento.master.house = Houses;
                #endregion

                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(SalidaPedimento);

                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.confirmacionDeSalida(SalidaPedimento);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
            }

            Inserta_Notificador(Int32.Parse(SalidaPedimento.informacionGeneral.idAsociado), "confirmacion de salida", "Request", xmlRequest, xmlResponse, estadoPeticion);
        }

        public void salidaPedimento()
        {
            wsSira.InformacionSalidaPedimento objSalidaPedimento = new wsSira.InformacionSalidaPedimento();
            wsSira.respuesta objRespuesta = new wsSira.respuesta();


            try
            {

                //*************************************************
                //   Consulta BD 
                //*************************************************


                objSalidaPedimento.informacionGeneral = new wsSira.InformacionOperacionGeneralSalidas ();
                objSalidaPedimento.informacionGeneral.idAsociado = "642";
                objSalidaPedimento.informacionGeneral.fechaRegistro = DateTime.Now;
                objSalidaPedimento.informacionGeneral.tipoMovimiento = 1; //Ingreso simple
                objSalidaPedimento.informacionGeneral.detalleMovimiento = 3;//Maritimo
                objSalidaPedimento.informacionGeneral.tipoOperacion = 1;//Importacion
                objSalidaPedimento.informacionGeneral.cveRecintoFiscalizado = cClaveRecinto;
                objSalidaPedimento.informacionGeneral.consecutivo = (DateTime.Now.ToString()).Replace("/", "").Replace(":", "").Replace("a. m.", "").Replace("p. m.", "").Replace(" ", "");
                objSalidaPedimento.informacionGeneral.detalleMovimiento = 1;

                objSalidaPedimento.master = new SalidaGuiaMaster();

                objSalidaPedimento.tipoSalida = "1";


                //objEntradaSimple.guiasHouse[];


                //Serializamos Objeto Request
                xmlRequest = ParseObjeto(objSalidaPedimento);


                //Lanzamos peticion al WS de Sira
                objRespuesta = ws.salidaPedimento (objSalidaPedimento);

                //Serializamos objeto Response
                xmlResponse = ParseObjeto(objRespuesta);

                estadoPeticion = 1;
            }
            catch (Exception ex)
            {
                xmlResponse = ParseObjeto(ex.Message);
                estadoPeticion = 0;
            }

            Inserta_Notificador(Int32.Parse(objSalidaPedimento.informacionGeneral.idAsociado), "ingresoSimple", "Request", xmlRequest, xmlResponse, estadoPeticion);

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
