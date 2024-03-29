﻿using System;

namespace TodolistDemo.Domain
{
    public class TodoItem
    {
        public TodoItem()
        {
            Id = Guid.NewGuid().ToString();
            Name = string.Empty;
            IsDone = false;
            CreatedDatetimeUtc = DateTime.UtcNow;
            DueDateUtc = DateTime.UtcNow.AddDays(1);
        }

        public TodoItem(string name, string description, DateTime dueDateLocal):this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Item name can not be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Item description can not be empty.", nameof(description));
            }

            var dueDateUtc = TimeZoneInfo.ConvertTimeToUtc(dueDateLocal, TimeZoneInfo.Local);
            if (dueDateUtc < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Due data cannot be in the past.", nameof(dueDateLocal));
            }

            Name = name;
            Description = description;
            DueDateUtc = dueDateUtc;
        }
        
        public string Id { private set; get; }
        public string Name { private set; get; }
        public string Description { private set; get; }
        public bool IsDone {private set; get;}
        public DateTime CreatedDatetimeUtc { private set; get; }
        public DateTime DueDateUtc { private set; get; }

        public TodoItem SetItemFinished()
        {
            IsDone = true;
            return this;
        }

        public TodoItem SetItemUnFinished()
        {
            IsDone = false;
            return this;
        }
    }
}
