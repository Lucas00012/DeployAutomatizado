using API.Core.DTOs;
using API.Core.Entities;
using API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TarefasController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTarefas()
        {
            var tarefas = await _dbContext.Tarefas.ToListAsync();
            return Ok(tarefas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTarefa(int id)
        {
            var tarefa = await _dbContext.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return Ok(tarefa);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarTarefa(TarefaDto tarefaDto)
        {
            var tarefa = new Tarefa
            {
                Descricao = tarefaDto.Descricao,
                Nome = tarefaDto.Nome
            };

            await _dbContext.Tarefas.AddAsync(tarefa);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarTarefa(int id, TarefaDto tarefaDto)
        {
            var tarefa = await _dbContext.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            tarefa.Nome = tarefaDto.Nome;
            tarefa.Descricao = tarefaDto.Descricao;

            _dbContext.Tarefas.Update(tarefa);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }   
    }
}
