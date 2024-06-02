namespace RoadReady.Exceptions
{
    public class NoSuchReviewException : Exception
    {
        string message;
        public NoSuchReviewException()
        {
            message = "No review with the given id";
        }
        public override string Message => message;
    }
}
