using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssignmentDecs.Data
{
    public class Result
    {
        public Result()
        {
            IsSuccess = false;
            Messages = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }

        public string Message
        {
            get
            {
                if (Messages == null)
                {
                    Messages = new List<string>();
                }

                return Messages.FirstOrDefault();
            }
            set
            {
                if (Messages == null)
                {
                    Messages = new List<string>();
                }

                Messages.Add(value);
            }
        }

        public static Result FromException(Exception exception)
        {
            var result = new Result { Message = exception.Message, IsSuccess = false };

            while (exception.InnerException != null)
            {
                result.Message = exception.InnerException.Message;
                exception = exception.InnerException;
            }

            return result;
        }
    }

    public class Result<T> : Result
    {
        public Result()
        {
            IsSuccess = false;
            Messages = new List<string>();
        }

        public Result(Result result, T data)
        {
            IsSuccess = result.IsSuccess;
            Messages = result.Messages;
        }

        public T Data { get; set; }
        public static Result<T> FromException(Exception exception, T data)
        {
            var result = new Result<T> { Message = exception.Message, IsSuccess = false, Data = data };

            while (exception.InnerException != null)
            {
                result.Message = exception.InnerException.Message;
                exception = exception.InnerException;
            }

            return result;
        }
    }
}
