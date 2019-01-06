using System;

namespace Shared
{
    public class Envelope<T>
    {
        public Envelope(Guid id, T payload)
        {
            Id = id;
            Payload = payload;
        }

        public Guid Id { get; }
        public T Payload { get; }
    }
}
