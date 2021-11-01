using System;

namespace Core5ApiBoilerplate.Infrastructure.Events
{
    public class EntityEventArgs : EventArgs
    {
        public Type[] Types { get; }

        public EntityEventArgs(Type[] types)
        {
            Types = types;
        }
    }
}
