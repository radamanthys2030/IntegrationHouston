using Integration.Houston.Application.Contract;
using Integration.Houston.Application.Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationHouston.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CryptoController : ControllerBase
    {
        private IApplicationContract _applicationContract;

        public CryptoController(IApplicationContract applicationContract)
        {
            _applicationContract = applicationContract;
        }

        [HttpPost]
        [Route("Transaction", Name = "Crypto_Transaction")]

        public async Task<IActionResult> Transaction(CryptoCommand creditCardCommand)
        {
            var tra = await _applicationContract.AddTCryptoTransaction(creditCardCommand);
            return Ok(tra);
        }


        [HttpGet("Transactiondetails/{id}")]
        public async Task<IActionResult> GetPago(Guid id)
        {
            var trans = await _applicationContract.GetCryptoTransaction(id);
            return Ok(trans);
        }
    }
}
