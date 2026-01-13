using Integration.Houston.Application.Infrastructure.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Services
{

    public interface ICryptoProcessor
    {
        public Task<CryptoResponseProcesador> CreateCryptoTransaction(string referenceCode, decimal amount, Guid TransaccionID);  
    }
    public class OOPayProcessor : ICryptoProcessor
    {
        const string API_KEY = "mk_MERCHANT_1e1931029fffda26da461f27a8238af3"; // <= pon tu API key real
        const string BASE_URL = "https://api.ooppay.io/api/v1"; // endpoint público de OOPPay (según docs)
        const string CALLBACK_URL = "https://tu-dominio.com/webhooks/ooppay"; // tu webhook público

        static JsonSerializerOptions JsonOptions() => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
        };
        public async Task<CryptoResponseProcesador> CreateCryptoTransaction(string referenceCode, decimal amount,Guid TransaccionID)
        {
            var orderReq = new CreateOrderRequest
            {
                MerchantOrderId = $"{TransaccionID.ToString()}",
                Amount = $"{amount}",               // importe en USD como string; OOPPay muestra un buffer de red adicional en la respuesta
                CallbackUrl = CALLBACK_URL,     // OOPPay enviará el webhook aquí al confirmar el pago
                ExpireMinutes = 30              // tiempo de expiración de la orden en minutos
            };


            using var http = new HttpClient
            {
                BaseAddress = new Uri("https://api.ooppay.io/") // 👈 sólo dominio, con barra final
            };
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", API_KEY);

            var jsonReq = JsonSerializer.Serialize(orderReq, JsonOptions());

            using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/orders") // 👈 sin barra inicial
            {
                Content = new StringContent(jsonReq, Encoding.UTF8, "application/json")
            };

            Console.WriteLine($">> URL final: {http.BaseAddress}{req.RequestUri}"); // debería imprimir https://api.ooppay.io/api/v1/orders
            var resp = await http.SendAsync(req);

            var raw = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"ERROR HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}");
                Console.WriteLine(raw);
                return null;
            }

            var orderResp = JsonSerializer.Deserialize<CreateOrderResponse>(raw, JsonOptions())!;

            return new CryptoResponseProcesador
            {
                OrderId = orderResp.OrderId,
                PayAddress = orderResp.PayAddress,
                PayAmount = decimal.Parse(orderResp.PayAmount),
                OrderAmount = decimal.Parse(orderResp.OrderAmount),
                BufferAmount = decimal.Parse(orderResp.BufferAmount),
                QrCodeUrl = orderResp.QrCodeUrl,
                ExpireAt = DateTimeOffset.FromUnixTimeSeconds(orderResp.ExpireAt).DateTime
            };

        }
    }

    public sealed class CreateOrderRequest
    {
        [JsonPropertyName("merchant_order_id")] public string MerchantOrderId { get; set; } = default!;
        [JsonPropertyName("amount")] public string Amount { get; set; } = default!;
        [JsonPropertyName("callback_url")] public string CallbackUrl { get; set; } = default!;
        [JsonPropertyName("expire_minutes")] public int ExpireMinutes { get; set; } = 30;
    }

    public sealed class CreateOrderResponse
    {
        // Los nombres exactos provienen del ejemplo de respuesta de OOPPay
        [JsonPropertyName("order_id")] public string OrderId { get; set; } = default!;
        [JsonPropertyName("pay_address")] public string PayAddress { get; set; } = default!;
        [JsonPropertyName("pay_amount")] public string PayAmount { get; set; } = default!;
        [JsonPropertyName("order_amount")] public string OrderAmount { get; set; } = default!;
        [JsonPropertyName("buffer_amount")] public string BufferAmount { get; set; } = default!;
        [JsonPropertyName("qr_code_url")] public string QrCodeUrl { get; set; } = default!;
        [JsonPropertyName("expire_at")] public long ExpireAt { get; set; }
    }
}
