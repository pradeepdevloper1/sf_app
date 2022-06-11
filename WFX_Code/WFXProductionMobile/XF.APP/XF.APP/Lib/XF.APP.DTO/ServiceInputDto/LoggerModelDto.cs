namespace XF.APP.DTO
{
    public class LoggerModelDto
    {
        public LoggerType LoggerType { get; set; } = LoggerType.Error;
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
