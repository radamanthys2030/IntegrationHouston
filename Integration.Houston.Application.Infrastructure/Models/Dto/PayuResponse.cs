using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Models.Dto
{
    public class PayuResponse
    {

        public string code { get; set; }

        /// <summary>
        /// Mensaje de error (si code != "SUCCESS").
        /// </summary>
        public string error { get; set; }

        /// <summary>
        /// Detalle de la transacción procesada.
        /// </summary>
        public TransactionResponse transactionResponse
        {
            get; set;

        }
    }


    public class TransactionResponse
    {
        /// <summary>
        /// ID de la orden generada en PayU.
        /// </summary>
        public long? orderId { get; set; }

        /// <summary>
        /// ID de la transacción en PayU.
        /// </summary>
        public long? transactionId { get; set; }

        /// <summary>
        /// Estado de la transacción (ej.: "APPROVED", "DECLINED", "PENDING").
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Código de respuesta del gateway (ej.: "APPROVED", "DECLINED", "PENDING", "ERROR", etc.).
        /// </summary>
        public string responseCode { get; set; }

        /// <summary>
        /// Código de autorización de la red (si fue aprobada).
        /// </summary>
        public string authorizationCode { get; set; }

        /// <summary>
        /// Código de respuesta de la red adquirente (Visa/Master/etc.).
        /// </summary>
        public string paymentNetworkResponseCode { get; set; }

        /// <summary>
        /// Mensaje de error de la red adquirente.
        /// </summary>
        public string paymentNetworkResponseErrorMessage { get; set; }

        /// <summary>
        /// (Opcional) Razón de pendiente (si state == "PENDING").
        /// </summary>
        public string pendingReason { get; set; }

        /// <summary>
        /// Parámetros adicionales devueltos por PayU/red (ej.: referencia, soft descriptor, etc.).
        /// </summary>
        public Dictionary<string, object> extraParameters { get; set; }

        /// <summary>
        /// Fecha/hora de la operación (usualmente ISO 8601).
        /// </summary>
        public DateTime? operationDate { get; set; }

        /// <summary>
        /// Valor total procesado.
        /// </summary>
        public decimal? transactionValue { get; set; }

        /// <summary>
        /// Moneda del valor (ej.: "COP").
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// (Opcional) Número de cuotas aplicadas.
        /// </summary>
        public int? installmentsNumber { get; set; }
    }


}
