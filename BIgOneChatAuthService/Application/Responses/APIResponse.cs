using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class APIResponse<TData>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CodeResponse {  get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TData? Response { get; set; }

        public APIResponse(int? codeResponse, string? message)
        {
            CodeResponse = codeResponse;
            Message = message;
        }

        public APIResponse(TData? response)
        {
            CodeResponse = null;
            Response = response;
        }

        public APIResponse()
        {
            CodeResponse = null;
        }
    }
}
