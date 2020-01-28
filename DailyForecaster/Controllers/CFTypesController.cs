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
    public class CFTypesController : ControllerBase
    {
        private readonly FinPlannerContext _context;

        public CFTypesController(FinPlannerContext context)
        {
            _context = context;
        }

        // GET: api/CFTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CFType>>> GetCFTypes()
        {
            return await _context.CFTypes.ToListAsync();
        }

        // GET: api/CFTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CFType>> GetCFType(string id)
        {
            var cFType = await _context.CFTypes.FindAsync(id);

            if (cFType == null)
            {
                return NotFound();
            }

            return cFType;
        }

        // PUT: api/CFTypes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCFType(string id, CFType cFType)
        {
            if (id != cFType.Id)
            {
                return BadRequest();
            }

            _context.Entry(cFType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CFTypeExists(id))
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

        // POST: api/CFTypes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CFType>> PostCFType(CFType cFType)
        {
            _context.CFTypes.Add(cFType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CFTypeExists(cFType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCFType", new { id = cFType.Id }, cFType);
        }

        // DELETE: api/CFTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CFType>> DeleteCFType(string id)
        {
            var cFType = await _context.CFTypes.FindAsync(id);
            if (cFType == null)
            {
                return NotFound();
            }

            _context.CFTypes.Remove(cFType);
            await _context.SaveChangesAsync();

            return cFType;
        }

        private bool CFTypeExists(string id)
        {
            return _context.CFTypes.Any(e => e.Id == id);
        }
    }
}
