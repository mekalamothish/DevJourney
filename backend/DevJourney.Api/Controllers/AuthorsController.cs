using Microsoft.AspNetCore.Mvc;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Dto.Common;
using DevJourney.Application.Repositories;

namespace DevJourney.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _service;

        public AuthorsController(IAuthorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AuthorQuery query)
        {
            var result = await _service.GetPagedAsync(query);
            return Ok(new { data = result.Data, meta = result.Meta });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return Ok(new { data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto create)
        {
            var created = await _service.CreateAsync(create);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { data = created });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateAuthorDto update)
        {
            var updated = await _service.UpdateAsync(id, update);
            return Ok(new { data = updated });
        }
    }
}
