using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DailyForecaster.Models;

namespace DailyForecaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualCashFlowsController : ControllerBase
    {
        private readonly FinPlannerContext _context;

        public ManualCashFlowsController(FinPlannerContext context)
        {
            _context = context;
        }

        // GET: api/ManualCashFlows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManualCashFlow>>> GetManualCashFlows()
        {
            return await _context.ManualCashFlows.Where(x=>x.isDeleted == false).ToListAsync();
        }

        // GET: api/ManualCashFlows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManualCashFlow>> GetManualCashFlow(string id)
        {
            var manualCashFlow = await _context.ManualCashFlows.FindAsync(id);

            if (manualCashFlow == null || manualCashFlow.isDeleted == true)
            {
                return NotFound();
            }

            return manualCashFlow;
        }

        // PUT: api/ManualCashFlows/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManualCashFlow(string id, ManualCashFlow manualCashFlow)
        {
            if (id != manualCashFlow.Id || manualCashFlow.isDeleted)
            {
                return BadRequest();
            }

            _context.Entry(manualCashFlow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManualCashFlowExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ManualCashFlows
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ManualCashFlow>> PostManualCashFlow(tempManualCashFlow mcf)
        {
            CFType type = _context.CFTypes.Find(mcf.CFType);
            CFClassification classification = _context.CFClassifications.Find(mcf.CFClassification);
            ManualCashFlow cf = new ManualCashFlow(type,classification,mcf.Amount,mcf.DateBooked,mcf.SourceOfExpense,mcf.UserId,mcf.Expected,mcf.ExpenseLocation,"");
            if (ModelState.IsValid)
            {
                _context.ManualCashFlows.Add(cf);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ManualCashFlowExists(cf.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetManualCashFlow", new { id = cf.Id }, cf);
        }

        // DELETE: api/ManualCashFlows/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManualCashFlow>> DeleteManualCashFlow(string id)
        {
            var manualCashFlow = await _context.ManualCashFlows.FindAsync(id);
            if (manualCashFlow == null)
            {
                return NotFound();
            }
            manualCashFlow.isDeleted = true;

            _context.Entry(manualCashFlow).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return manualCashFlow;
        }

        private bool ManualCashFlowExists(string id)
        {
            return _context.ManualCashFlows.Any(e => e.Id == id);
        }
    }
}
