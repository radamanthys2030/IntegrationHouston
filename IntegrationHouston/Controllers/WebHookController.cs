using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Internal;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationHouston.Controllers
{

    public sealed class CryptomusWebhookPayload
    {
        [JsonPropertyName("type")] public string Type { get; set; } = default!;           // "wallet" | "payment"
        [JsonPropertyName("uuid")] public string Uuid { get; set; } = default!;
        [JsonPropertyName("order_id")] public string OrderId { get; set; } = default!;
        [JsonPropertyName("amount")] public string Amount { get; set; } = default!;
        [JsonPropertyName("payment_amount")] public string? PaymentAmount { get; set; }
        [JsonPropertyName("payment_amount_usd")] public string? PaymentAmountUsd { get; set; }
        [JsonPropertyName("merchant_amount")] public string? MerchantAmount { get; set; }
        [JsonPropertyName("commission")] public string? Commission { get; set; }
        [JsonPropertyName("is_final")] public bool IsFinal { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = default!;       // paid, paid_over, wrong_amount, cancel, fail, system_fail, ...
        [JsonPropertyName("currency")] public string Currency { get; set; } = default!;
        [JsonPropertyName("payer_currency")] public string? PayerCurrency { get; set; }
        [JsonPropertyName("network")] public string? Network { get; set; }
        [JsonPropertyName("txid")] public string? TxId { get; set; }                      // puede no estar en P2P
        [JsonPropertyName("sign")] public string Sign { get; set; } = default!;
        // ... otros campos disponibles en la doc (wallet_address_uuid, convert, additional_data, etc.)
    }



    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = null
        };


        [HttpPost("payments")]
        public async Task<IActionResult> HandlePaymentWebhook()
        {
            // 1) Leer el body exacto (tal cual) para poder verificar la firma
            var rawBody = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            // 2) Deserializar (opcional pero útil)
            var payload = JsonSerializer.Deserialize<CryptomusWebhookPayload>(rawBody, SerializerOptions);
            if (payload is null)
                return BadRequest("Invalid payload");

            // 3) Verificar firma del webhook:
            //    sign == md5(base64(rawBody) + PaymentApiKey)
            var bodyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawBody));
            var expectedSign = Md5Hex(bodyBase64 +"API KEY");

            if (!string.Equals(payload.Sign, expectedSign, StringComparison.OrdinalIgnoreCase))
                return Unauthorized(); // firma inválida

            // 4) Manejar estados de pago
            //    Referencia de estados y campos: doc de Webhook. [1](https://doc.cryptomus.com/merchant-api/payments/webhook)
            switch (payload.Status)
            {
                case "paid":
                    // Pago exacto acreditado
                    await OnPaidAsync(payload);
                    break;

                case "paid_over":
                    // Pago mayor al requerido; maneja la diferencia si corresponde
                    await OnPaidOverAsync(payload);
                    break;

                case "wrong_amount":
                    // Monto inferior al requerido; si is_payment_multiple=true, podría completarse luego
                    await OnWrongAmountAsync(payload);
                    break;

                case "cancel":
                case "fail":
                case "system_fail":
                    // Cancelación / expiración / fallo del sistema
                    await OnFailOrCancelAsync(payload);
                    break;

                case "confirm_check":
                    // Estado intermedio (ej. verificación en curso)
                    await OnConfirmCheckAsync(payload);
                    break;

                // otros: refund_process, refund_fail, refund_paid...
                default:
                    // Loguear para monitoreo
                    Console.WriteLine($"Webhook desconocido: {payload.Status}");
                    break;
            }

            // 5) Importante: responder 200 para que Cryptomus considere el webhook entregado
            return Ok();
        }


        private static string Md5Hex(string input)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        // ————————————————————————
        // Handlers de negocio (dummy)
        // En tu implementación: actualizar Factura/Pago en DB, emitir eventos, etc.

        private Task OnPaidAsync(CryptomusWebhookPayload p)
        {
            Console.WriteLine($"[PAID] order={p.OrderId} uuid={p.Uuid} amount={p.PaymentAmount ?? p.Amount} currency={p.Currency} txid={p.TxId}");
            // TODO: marcar Factura como pagada y registrar el pago contra la factura
            return Task.CompletedTask;
        }

        private Task OnPaidOverAsync(CryptomusWebhookPayload p)
        {
            Console.WriteLine($"[PAID_OVER] order={p.OrderId} pago={p.PaymentAmount} requerido={p.Amount}");
            // TODO: acreditar y manejar diferencia (según política)
            return Task.CompletedTask;
        }

        private Task OnWrongAmountAsync(CryptomusWebhookPayload p)
        {
            Console.WriteLine($"[WRONG_AMOUNT] order={p.OrderId} pago={p.PaymentAmount} requerido={p.Amount}");
            // TODO: notificar al usuario y permitir completar si habilitado
            return Task.CompletedTask;
        }

        private Task OnFailOrCancelAsync(CryptomusWebhookPayload p)
        {
            Console.WriteLine($"[CANCEL/FAIL] order={p.OrderId} status={p.Status}");
            // TODO: cerrar invoice / marcar como cancelada / permitir reintento
            return Task.CompletedTask;
        }

        private Task OnConfirmCheckAsync(CryptomusWebhookPayload p)
        {
            Console.WriteLine($"[CONFIRM_CHECK] order={p.OrderId}");
            // TODO: estado transitorio; usualmente no mover estado contable aún
            return Task.CompletedTask;
        }


    }
}
