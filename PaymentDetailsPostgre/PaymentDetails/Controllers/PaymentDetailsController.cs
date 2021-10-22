using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaymentDetails.Data;
using PaymentDetails.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace PaymentDetails.Controllers
{
    [Route("finalproject/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentDetailsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public PaymentDetailsController(ApiDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentData data)
        {
            if (ModelState.IsValid)
            {
                await _context.Payments.AddAsync(data);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPayments", new { data.PaymentDataId }, data);
            }
            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentDataId == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpGet("GetUsers/{user1}/{user2}")]
        public async Task<IActionResult> GetUsers(int user1, int user2)
        {
            var user = await _context.Payments.Where(x => x.PaymentDataId >= user1 && x.PaymentDataId <= user2).ToListAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentData payment)
        {
            if (id != payment.PaymentDataId)
                return BadRequest();

            var existPayment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentDataId == id);

            if (existPayment == null)
                return NotFound();

            existPayment.CardOwnerName = payment.CardOwnerName;
            existPayment.CardNumber = payment.CardNumber;
            existPayment.ExpirationDate = payment.ExpirationDate;
            existPayment.SecurityCode = payment.SecurityCode;

            await _context.SaveChangesAsync();
            
            object[] result = new object[2];
            result[0] = "Berhasil update data!";
            // result[1] = CreatedAtAction("GetPayments", new { payment.PaymentDataId }, payment);
            result[1] = existPayment;
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var existPayment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentDataId == id);

            if (existPayment == null)
                return NotFound();

            _context.Payments.Remove(existPayment);
            await _context.SaveChangesAsync();

            return new JsonResult("Payment dengan id " + id + " berhasil dihapus!") { StatusCode = 200 };
        }
    }
}
