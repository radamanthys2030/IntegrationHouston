using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Integration.Houston.Application.Contract.Models
{
    // Raíz del request
    public class CreditCardCommand
    {
        // Idioma del request (ej: "es", "en"). Usado para mensajes de error.
        public string language { get; set; } = "es";

        // Siempre "SUBMIT_TRANSACTION" para enviar transacciones.
        public string command { get; set; } = "SUBMIT_TRANSACTION";

        // Bandera de prueba: true (sandbox) / false (producción).
        // En JSON suele llamarse "test".
        public bool test { get; set; } = true;

        // Datos de autenticación (merchant)
        public Merchant merchant { get; set; } = new();

        // Datos de la transacción (orden, tarjeta, etc.)
        public Transaction transaction { get; set; } = new();
    }

    public class Merchant
    {
        // Usuario/login provisto por PayU (API Login)
        public string apiLogin { get; set; } = "TU_API_LOGIN";

        // API Key provista por PayU
        public string apiKey { get; set; } = "TU_API_KEY";
    }

    public class Transaction
    {
        // Obligatorio: datos de la orden (referencia, descripción, montos)
        public Order order { get; set; } = new();

        // Datos del pagador (recomendado para antifraude y recibos)
        public Payer payer { get; set; }

        // Datos de la tarjeta
        public CreditCard creditCard { get; set; }

        // Parámetros extra (ej: número de cuotas)
        public Dictionary<string, object> extraParameters { get; set; }

        // Tipo de transacción: para 1-step (charge) usar "AUTHORIZATION_AND_CAPTURE"
        public string type { get; set; } = "AUTHORIZATION_AND_CAPTURE";

        // Método de pago (VISA, MASTERCARD, AMEX, DINERS, CODENSA, etc.)
        public string paymentMethod { get; set; } = "VISA";

        // País de pago (CO para Colombia)
        public string paymentCountry { get; set; } = "CO";

        // Datos de dispositivo (antifraude); opcional pero recomendable
        public string deviceSessionId { get; set; }
        public string ipAddress { get; set; }
        public string cookie { get; set; }
        public string userAgent { get; set; }

        // (Opcional) Tokenización: si usás token, empleás creditCardTokenId y seguridad
        public string? creditCardTokenId { get; set; }

        // (Opcional) 3DS passthrough; objeto adicional si haces autenticación propia
        public ThreeDSecure? threeDomainSecure { get; set; }
    }

    public class Order
    {
        // Identificador de cuenta en PayU (si aplica)
        public string accountId { get; set; }

        // Código de referencia único de tu comercio (para conciliación)
        public string referenceCode { get; set; }

        // Descripción/Concepto de la compra
        public string description { get; set; }

        // Idioma para la orden (consistente con language)
        public string language { get; set; } = "es";

        // Firma (signature) si tu flujo la requiere (clásico en APIs anteriores);
        // en la 4.0 no siempre se exige, pero puedes incluirla si corresponde
        public string? signature { get; set; }

        // URL de notificación (webhook del comercio)
        public string notifyUrl { get; set; } = "https://tu-dominio.com/payu/webhook";

        // Valores de la transacción (monto, impuestos, etc.)
        public AdditionalValues additionalValues { get; set; } = new();

        // (Opcional) Cliente / shipping info si tu modelo lo requiere
        public Buyer? buyer { get; set; }
    }

    public class AdditionalValues
    {
        // Valor total
        public Money TX_VALUE { get; set; }

        // Impuesto (IVA) si aplica
        public Money TX_TAX { get; set; }

        // Base gravable del impuesto (si aplica)
        public Money TX_TAX_RETURN_BASE { get; set; }
    }

    public class Money
    {
        public decimal value { get; set; }
        // Moneda (COP, USD, etc.)
        public string currency { get; set; } = "COP";
    }

    public class Buyer
    {
        public string fullName { get; set; }
        public string emailAddress { get; set; }
        public string contactPhone { get; set; }
        public string dniNumber { get; set; } // CC/NIT pasaporte, etc.
        public Address shippingAddress { get; set; }
    }

    public class Payer
    {
        public string fullName { get; set; }
        public string emailAddress { get; set; }
        public string contactPhone { get; set; }
        public string dniNumber { get; set; }
        public Address billingAddress { get; set; }
    }

    public class Address
    {
        public string street1 { get; set; }
        public string? street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; } = "CO";
        public string postalCode { get; set; }
        public string phone { get; set; }
    }

    public class CreditCard
    {
        // Número de tarjeta (PAN) – solo si haces server-to-server
        public string number { get; set; }

        // CVV – por defecto es requerido; si tu comercio está habilitado
        // para procesar sin CVV, puedes usar processWithoutCvv2=true y omitirlo.
        public string securityCode { get; set; }

        // Expiración en formato "YYYY/MM" (mucho doc lo muestra como "YYYY/MM")
        public string expirationDate { get; set; }

        // Nombre como figura en la tarjeta
        public string name { get; set; }

        // Si tu comercio tiene habilitado procesar sin CVV:
        public bool? processWithoutCvv2 { get; set; }
    }

    public class ThreeDSecure
    {
        // Campos de passthrough (Ejemplo; depende de tu escenario)
        public string cavv { get; set; }
        public string eci { get; set; }
        public string xid { get; set; }
        public string pares { get; set; }
    }
}

