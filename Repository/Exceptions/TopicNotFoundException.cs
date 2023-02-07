using System.Runtime.Serialization;

namespace Personal_Collection_Manager.Repository.Exceptions
{
    public class TopicNotFoundException : Exception
    {
        public TopicNotFoundException()
        {
        }

        public TopicNotFoundException(string? message) : base(message)
        {
        }

        public TopicNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TopicNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
