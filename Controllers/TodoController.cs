using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;


#region TodoController
namespace TodoApi.Controllers
{

    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _remoteUrl = "https://test1016-backend.azurewebsites.net";
        private readonly TodoContext _context;
        #endregion

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
       {
           base.OnActionExecuting(context);
           _client.DefaultRequestHeaders.Accept.Clear();
           _client.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);
       }

        #region snippet_GetAll
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            var data = _client.GetStringAsync($"{_remoteUrl}/api/Todo").Result;
            return JsonConvert.DeserializeObject<List<TodoItem>>(data);
        }

        #region snippet_GetByID
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var data = _client.GetStringAsync($"{_remoteUrl}/api/Todo/{id}").Result;
            return Content(data, "application/json");
        }
        #endregion
        #endregion
        #region snippet_Create
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            var response = _client.PostAsJsonAsync($"{_remoteUrl}/api/Todo", item).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return Content(data, "application/json");
        }
        #endregion

        #region snippet_Update
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            var res = _client.PutAsJsonAsync($"{_remoteUrl}/api/Todo/{id}", item).Result;
            return new NoContentResult();
        }
        #endregion

        #region snippet_Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var res = _client.DeleteAsync($"{_remoteUrl}/api/Todo/{id}").Result;
            return new NoContentResult();
        }
        #endregion
    }
}

