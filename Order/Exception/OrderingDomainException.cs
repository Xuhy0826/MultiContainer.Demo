namespace Order.Exception
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class OrderingDomainException : System.Exception
    {
        public OrderingDomainException()
        { }

        public OrderingDomainException(string message)
            : base(message)
        { }

        public OrderingDomainException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }
}
