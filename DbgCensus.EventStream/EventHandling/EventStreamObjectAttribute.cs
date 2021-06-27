using System;

namespace DbgCensus.EventStream.EventHandling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EventStreamObjectAttribute : Attribute
    {
        /// <summary>
        /// The websocket service that this object will originate from.
        /// </summary>
        public string Service { get; }

        /// <summary>
        /// The type of the object.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="EventStreamObjectAttribute"/> attribute.
        /// </summary>
        /// <param name="service">The websocket service that this object will originate from.</param>
        /// <param name="type">The type of the object.</param>
        public EventStreamObjectAttribute(string service, string type)
        {
            Service = service;
            Type = type;
        }
    }
}
