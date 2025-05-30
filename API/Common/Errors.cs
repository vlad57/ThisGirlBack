using System;

namespace API.Common;

    public class NotFoundError
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string? Detail { get; set; }
        public string? Errors { get; set; }
    }

    public class BadRequestError
    {
        public int Status { get; set; }
        public string Errors { get; set; }
    }

    public class InternalError
    {
        public int Status { get; set; }
        public string Errors { get; set; }
    }
