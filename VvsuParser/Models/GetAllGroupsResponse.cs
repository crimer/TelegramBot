using System.Collections.Generic;
using Newtonsoft.Json;

namespace VvsuParser.Models;

public class GetAllGroupsResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
        
    [JsonProperty("rows")]
    public List<GroupRow> Rows { get; set; }
}

public class GroupRow
{
    [JsonProperty("id")]
    public string Id { get; set; }
        
    [JsonProperty("value")]
    public string Value { get; set; }
}