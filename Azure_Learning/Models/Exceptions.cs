using System;
using System.Runtime.Serialization;

namespace AzureLearning.Models
{
    [Serializable]
    public class DbItemNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DbItemNotFoundException()
        {
        }

        public DbItemNotFoundException(string message) : base(message)
        {
        }

        public DbItemNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DbItemNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class DbAddFailedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DbAddFailedException()
        {
        }

        public DbAddFailedException(string message) : base(message)
        {
        }

        public DbAddFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DbAddFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}