using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IFoundBackend.SqlModels;
using IFoundBackend.Model.Enums;
using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.Areas.Posts;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace IFoundBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostPersonsController : ControllerBase
    {
        private readonly IfoundContext _context;

        public PostPersonsController(IfoundContext context)
        {
            _context = context;
        }

        // GET: api/PostPersons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostPerson>>> GetPostPeople()
        {
            return await _context.PostPeople.ToListAsync();
        }

        

        // PUT: api/PostPersons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostPerson(int id, PostPerson postPerson)
        {
            if (id != postPerson.PostPersonId)
            {
                return BadRequest();
            }

            _context.Entry(postPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostPersonExists(id))
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

        // POST: api/PostPersons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostPerson>> PostPostPerson(PostPerson postPerson)
        {
            _context.PostPeople.Add(postPerson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostPerson", new { id = postPerson.PostPersonId }, postPerson);
        }

        // DELETE: api/PostPersons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostPerson(int id)
        {
            try
            {
                var postPerson = await _context.PostPeople.FindAsync(id);
                if (postPerson == null)
                {
                    return NotFound();
                }

                _context.PostPeople.Remove(postPerson);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            } 
        }

        private bool PostPersonExists(int id)
        {
            return _context.PostPeople.Any(e => e.PostPersonId == id);
        }
    }
}
