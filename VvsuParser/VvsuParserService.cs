using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using VvsuParser.Models;
using VvsuParser.Selenium;
using VvsuParser.VvsuStudySchedule;

namespace VvsuParser;

public class VvsuParserService
{
    private readonly VvsuStudyScheduleParser _vvsuStudyScheduleParser;
    private readonly SeleniumLoader _seleniumLoader;

    public VvsuParserService(VvsuStudyScheduleParser vvsuStudyScheduleParser, SeleniumLoader seleniumLoader)
    {
        _vvsuStudyScheduleParser = vvsuStudyScheduleParser;
        _seleniumLoader = seleniumLoader;
    }

    public async Task<List<string>> ParseAllGroupsAsync()
    {
        var allGroups = await _seleniumLoader.GetAllGroupsHtml();
            
        // await SaveToJsonAsync("vvsu_groups_data.json", allGroups);

        return allGroups;
    } 
         
    public async Task<List<VvsuStudyScheduleWeek>> ParseScheduleTableAsync()
    {
        var htmlDocument = new HtmlDocument();
        
        var scheduleTableHtml = await _seleniumLoader.GetScheduleTableHtml();
        htmlDocument.LoadHtml(scheduleTableHtml);
        
        var table = await _vvsuStudyScheduleParser.ParseAsync(htmlDocument);
            
        // await SaveToJsonAsync("vvsu_schedule_data.json", table);
            
        return table;
    }
        
    private async Task SaveToJsonAsync(string fileName, object data)
    {
        var dirPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
        if (dirPath == null)
            throw new Exception("Пустой базовый путь");

        var filePath = Path.Combine(dirPath, "OutputJson", fileName);

        var json = JsonConvert.SerializeObject(data);
            
        await using var fs = new FileStream(filePath, FileMode.Create);
        await fs.WriteAsync(Encoding.UTF8.GetBytes(json));
    }
}