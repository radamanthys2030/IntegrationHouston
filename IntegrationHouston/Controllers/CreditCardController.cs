using Integration.Houston.Application.Contract;
using Integration.Houston.Application.Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IntegrationHouston.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CreditCardController : ControllerBase
    {
        private IApplicationContract _applicationContract;

        public CreditCardController(IApplicationContract applicationContract)
        {
            _applicationContract = applicationContract;
        }

        [HttpPost]
        [Route("Transaction", Name = "Transaction")]
       
        public async Task<IActionResult>  Transaction(CreditCardCommand creditCardCommand)
        {
            var tra = await _applicationContract.AddTransaction(creditCardCommand);
            return Ok(tra);
        }


        [HttpGet("Transactiondetails/{id}")]
        public async Task<IActionResult> GetPago(Guid id)
        {
            var trans = await _applicationContract.GetTransaction(id);
            return Ok(trans);
        }


    }
}
