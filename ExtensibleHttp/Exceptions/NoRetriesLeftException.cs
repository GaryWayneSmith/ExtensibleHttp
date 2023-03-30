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

namespace ExtensibleHttp.Exceptions
{
    public class NoRetriesLeftException : BaseException
    {
        public int RetryCount { get; private set; }

        private NoRetriesLeftException(string message, System.Exception innerException) : base(message, innerException)
        { }

        public static NoRetriesLeftException Factory(int retryCount, System.Exception innerException)
        {
            var exceptionMessage = string.Format("All {0} retry attempts spent. ", retryCount.ToString());
            var exception = new NoRetriesLeftException(exceptionMessage, innerException)
            {
                RetryCount = retryCount
            };
            return exception;
        }
    }
}
