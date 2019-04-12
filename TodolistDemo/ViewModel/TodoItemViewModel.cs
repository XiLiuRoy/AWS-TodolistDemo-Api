using System;

namespace TodolistDemo.ViewModel
{
    public class TodoItemViewModel
    {
        public TodoItemViewModel()
        {

        }

        public string Name { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public bool IsDone { set; get; } = false;
        public DateTime DueDate { set; get; } = DateTime.Now.AddDays(1);
    }
}
