using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyLib
{
	public class APIException : Exception
	{

		public HttpStatusCode StatusCode { get; private set; }

		public APIException(string Message, HttpStatusCode StatusCode): base(Message)
		{
			this.StatusCode = StatusCode;
		}

		public APIException(string Message, HttpStatusCode StatusCode, Exception InnerException) : base(Message, InnerException)
		{
			this.StatusCode = StatusCode;
		}

		public APIException(string Message) : base(Message)
		{
		}

		public APIException(string Message, Exception InnerException) : base(Message, InnerException)
		{
		}
	}
}
