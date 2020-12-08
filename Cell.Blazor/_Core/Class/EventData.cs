using Microsoft.AspNetCore.Components;
using System;

namespace Cell.Blazor._Core.Class
{
    public class EventData
    {
        public object Handler { get; set; }

        public Type ArgumentType { get; set; }

        public EventData Set<T>(EventCallback<T> action, Type type)
        {
            this.Handler = (object)action;
            this.ArgumentType = type;
            return this;
        }
    }
}