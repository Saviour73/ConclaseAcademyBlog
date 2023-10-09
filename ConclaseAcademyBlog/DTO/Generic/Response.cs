namespace ConclaseAcademyBlog.DTO.Generic
{
    public class Response<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ResponseError ResponseError { get; set; }
    }
}
