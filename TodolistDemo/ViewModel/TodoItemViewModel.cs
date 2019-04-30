using System;
using TodolistDemo.Domain;

namespace TodolistDemo.ViewModel
{
    public class TodoItemViewModel
    {
        public TodoItemViewModel()
        {

        }

        public TodoItemViewModel(TodoItem todoItem)
        {
            if (todoItem == null) throw new ArgumentNullException(nameof(todoItem));

            Id = todoItem.Id;
            Name = todoItem.Name;
            Description = todoItem.Description;
            IsDone = todoItem.IsDone;
            DueDate = todoItem.DueDateUtc.ToLocalTime();
        }

        public string Id { set; get; } = string.Empty;
        public string Name { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public bool IsDone { set; get; } = false;
        public DateTime DueDate { set; get; } = DateTime.Now.AddDays(1);
    }
}
