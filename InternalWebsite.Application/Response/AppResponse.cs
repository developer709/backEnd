namespace InternalWebsite.Application.ViewModels
{
    public class AppResponse
    {
        public bool SuccessFlag { get; set; } // reName IsSuccessFull
        public double TotalCount { get; set; }
        public object Data { get; set; }
        public object MiscData { get; set; }
        public string Message { get; set; }

    }
    public class UppercaseAppResponse
    {
        public bool SUCCESSFLAG { get; set; } // reName IsSuccessFull
        public double TOTALCOUNT { get; set; }
        public object DATA { get; set; }
        public object MISCDATA { get; set; }
        public string MESSAGE { get; set; }
    }

    public class AppResponse<T>
    {
        public bool SuccessFlag { get; set; }
        public T Data { get; set; }
        public object MiscData { get; set; }
        public string Message { get; set; }
    }

    public class AppResponse<T, TT>
    {
        public bool SuccessFlag { get; set; }
        public T Data { get; set; }
        public TT MiscData { get; set; }
        public string Message { get; set; }
    }
}