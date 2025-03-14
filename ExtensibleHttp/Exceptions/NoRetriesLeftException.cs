﻿/*
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

using System;
using System.Runtime.Serialization;

namespace ExtensibleHttp.Exceptions
{
	[Serializable]
	public class NoRetriesLeftException : BaseException
	{
		public int RetryCount { get; private set; }

		protected NoRetriesLeftException() : base() { }
		protected NoRetriesLeftException(string message) : base(message) { }
		protected NoRetriesLeftException(string message, Exception innerException) : base(message, innerException) { }

		public static NoRetriesLeftException Factory(int retryCount, Exception innerException)
		{
			var exceptionMessage = $"All {retryCount} retry attempts spent. ";
			var exception = new NoRetriesLeftException(exceptionMessage, innerException)
			{
				RetryCount = retryCount
			};
			return exception;
		}

		protected NoRetriesLeftException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
