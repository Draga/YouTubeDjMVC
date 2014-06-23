using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models.Responses
{
    public class ErrorResponse:Response
    {
        public ErrorResponse(string message)
        {
            Success = false;
            Message = message;
        }

        public string Message { get; set; }
    }
}