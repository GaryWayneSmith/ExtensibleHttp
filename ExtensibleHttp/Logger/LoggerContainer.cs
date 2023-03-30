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

namespace ExtensibleHttp.Logger
{
    public class LoggerContainer
    {
        public static ILoggerAdapter Logger = new NullLogger();

        private static LoggerContainer instance;
        public static LoggerContainer GetInstance()
        {
            if (instance is null)
            {
                instance = new LoggerContainer();
            }
            return instance;
        }

        public void Info(string message) => Logger.Info(message);
        public void Debug(string message) => Logger.Debug(message);
        public void Warning(string message) => Logger.Warning(message);
        public void Error(string message) => Logger.Error(message);
        public void Fatal(string message) => Logger.Fatal(message);
        public bool IsLevelEnabled(LogLevel level) => Logger.IsLevelEnabled(level);
    }
}
