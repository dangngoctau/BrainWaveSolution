using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ambition.Home.ViewComponents
{
    public class ToDoListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            string MyView = "Default";

            return await Task.FromResult<IViewComponentResult>(View(MyView, new List<TodoItem> { new TodoItem { Id = 1, Name = "Task 1", Priority = 2, IsDone = true } }));
        }
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsDone { get; set; }
    }
}
