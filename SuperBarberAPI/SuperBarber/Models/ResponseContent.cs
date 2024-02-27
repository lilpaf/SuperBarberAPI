#pragma warning disable CS8618

namespace SuperBarber.Models
{
    public class ResponseContent<TResponse> : ResponseContent
    {
        public TResponse Result { get; set; }
    }

    public class ResponseContent
    {
        public ErrorResponse Error { get; set; }
    }
}
