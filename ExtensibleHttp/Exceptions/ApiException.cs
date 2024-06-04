/*
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using ExtensibleHttp.Interfaces;
using System;
using System.Runtime.Serialization;

namespace ExtensibleHttp.Exceptions
{
	[Serializable]
	public class ApiException : BaseException
	{
		public IErrorsPayload Details { get; private set; }
		public IResponse Response { get; private set; }

		protected ApiException() : base() { }

		public ApiException(string message) : base(message) { }

		public static ApiException Factory(IErrorsPayload errorDetails, IResponse errorResponse)
		{
			if (errorDetails == null) throw new ArgumentNullException(nameof(errorDetails));
			if (errorResponse == null) throw new ArgumentNullException(nameof(errorResponse));

			var httpResponse = errorResponse.RawResponse;
			var exceptionMessage = $"API Error Occured [{((int)httpResponse.StatusCode)} {httpResponse.ReasonPhrase}]";
			exceptionMessage += errorDetails.RenderErrors();
			var exception = new ApiException(exceptionMessage)
			{
				Details = errorDetails,
				Response = errorResponse
			};
			return exception;
		}

		public ApiException(string message, Exception innerException) : base(message, innerException) { }

		protected ApiException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Details = (IErrorsPayload)info.GetValue("Details", typeof(IErrorsPayload));
			Response = (IResponse)info.GetValue("Response", typeof(IResponse));
		}
	}
}
