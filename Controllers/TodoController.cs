using Microsoft.AspNetCore.Mvc;
using MyApiProject.Models.Todo;
using MyApiProject.Services;

namespace MyApyiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? title, [FromQuery] string? description = null)
        {
            var todos = _todoService.GetAll();

            if (!string.IsNullOrEmpty(title))
            {
                todos = todos.Where(t => t.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(description))
            {
                todos = todos.Where(t => t.Description != null && t.Description.Contains(description));
            }

            return Ok(todos.ToList());
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(int id)
        {
            var item = _todoService.GetById(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create(TodoCreateModel item)
        {
            var newTodo = _todoService.Create(new Todo { Title = item.Title, Description = item.Description });
            return CreatedAtRoute("GetTodo", new { id = newTodo.Id }, newTodo);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Todo item)
        {
            var todo = _todoService.GetById(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.IsCompleted = item.IsCompleted;
            todo.Title = item.Title;

            _todoService.Update(todo);

            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var todo = _todoService.GetById(id);

            if (todo == null)
            {
                return NotFound();
            }

            _todoService.Delete(id);

            return NoContent();
        }
    }
}

