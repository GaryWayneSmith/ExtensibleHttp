using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleHttp.Sample.Implementation
{
    internal class SampleResult
    {

        internal enum Answers
        {
            Yes,
            No,
            Maybe,
        }

        [JsonProperty("answer", NullValueHandling = NullValueHandling.Ignore)]
        public Answers Answer { get; set; } = 0;
        [JsonProperty("forced", NullValueHandling = NullValueHandling.Ignore)]
        public bool Forced { get; set; } = false;
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SampleResult {\n");
            sb.Append("  Answer: ").Append(Answer).Append('\n');
            sb.Append("  Forced: ").Append(Forced).Append('\n');
            sb.Append("  Image: ").Append(Image).Append('\n');
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
