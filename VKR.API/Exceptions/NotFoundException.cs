namespace VKR.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ParameterName { get; set; } = string.Empty;

        public override string Message => $"{ParameterName} is not found";

        public NotFoundException(string parameterName) 
        {
            ParameterName = parameterName;
        }

    }
}
