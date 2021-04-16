using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Design;
using System;
using System.Collections.Generic;
using System.Xml;
namespace SIRA_Notificaciones
{
    public class RemoveAddressingHeadersAssertion : PolicyAssertion

    {

        public override SoapFilter CreateClientInputFilter(FilterCreationContext context)

        {
            Console.WriteLine("1");
            return new ClientInputFilter();

        }

        public override SoapFilter CreateClientOutputFilter(FilterCreationContext context)

        {
            Console.WriteLine("2");
            return new ClientOutputFilter();

        }

        public override SoapFilter CreateServiceInputFilter(FilterCreationContext context)

        {
            Console.WriteLine("4");
            return new ServiceInputFilter();

        }


        public override SoapFilter CreateServiceOutputFilter(FilterCreationContext context)

        {
            Console.WriteLine("5");
            return new ServiceOutputFilter();

        }

        public override System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Type>> GetExtensions()

        {
            Console.WriteLine("6");
            return new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("RemoveAddressingHeadersAssertion", this.GetType()) };

        }

        public override void ReadXml(XmlReader reader, IDictionary<string, Type> extensions)

        {
            Console.WriteLine("7");
            reader.ReadStartElement("RemoveAddressingHeadersAssertion");

        }

    }

    public class ClientInputFilter : SoapFilter

    {

        public override SoapFilterResult ProcessMessage(SoapEnvelope envelope)

        {
            envelope.Header.RemoveAll();
            Console.WriteLine("8");
            return SoapFilterResult.Continue;

        }

    }

    //provide implementation for only the ClientOutOutputFilter as we are trying to modify an outgoing soap request

    public class ClientOutputFilter : SoapFilter
    {
        public ClientOutputFilter() : base()
        {
            Console.WriteLine("9");
        }

        public override SoapFilterResult ProcessMessage(SoapEnvelope envelope)
        {

            //removing Addressing headers from the outgoing request
            XmlNode actionNode = envelope.Header["wsa:Action"];
            envelope.Header.RemoveChild(actionNode);
            XmlNode messageNode = envelope.Header["wsa:MessageID"];
            envelope.Header.RemoveChild(messageNode);
            XmlNode replyToNode = envelope.Header["wsa:ReplyTo"];
            envelope.Header.RemoveChild(replyToNode);
            XmlNode toNode = envelope.Header["wsa:To"];
            envelope.Header.RemoveChild(toNode);

            var v = envelope.Header;
            var z = v["wsse:Security"];
            envelope.Header["wsse:Security"].Attributes["soap:mustUnderstand"].Value = "0";
            Console.WriteLine("10");
            return SoapFilterResult.Continue;
        }
    }

    public class ServiceInputFilter : SoapFilter

    {

        public override SoapFilterResult ProcessMessage(SoapEnvelope envelope)

        {
            Console.WriteLine("11");
            return SoapFilterResult.Continue;

        }

    }

    public class ServiceOutputFilter : SoapFilter

    {

        public override SoapFilterResult ProcessMessage(SoapEnvelope envelope)
        {
            Console.WriteLine("12");
            return SoapFilterResult.Continue;
        }

    }
}
