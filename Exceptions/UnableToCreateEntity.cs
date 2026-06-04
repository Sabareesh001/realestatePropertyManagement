using Microsoft.AspNetCore.Diagnostics;

namespace propertyManagement.Exceptions
{
    public class UnableToCreateEntityException:Exception
    {
        string message;
        public UnableToCreateEntityException()
        {
            message = "Unable to create entity";
        }
        public UnableToCreateEntityException(string msg)
        {
            message= msg;
        }
        public override string Message => message;
    }
}