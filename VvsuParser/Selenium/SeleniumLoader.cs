using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VvsuParser.Extensions;
using VvsuParser.Models;

namespace VvsuParser.Selenium;

public class SeleniumLoader : IDisposable
{
    private readonly string _ul;
    private readonly ChromeDriver _chromeDriver;

    private string _getAllGroupsScript = @"
            fetch('https://www.vvsu.ru/controller/getFilterValues.php?field=543307', {
                'method': 'GET',
                'mode': 'cors',
                'credentials': 'include'
            })
            .then(r => r.json())
            .then(data => {
                window._allGroups = data; 
                console.log(window._allGroups);
            });
        ";

    public SeleniumLoader(string vvsuUrl)
    {
        _chromeDriver = new ChromeDriver();
        _chromeDriver.Navigate().GoToUrl(vvsuUrl);
    }
        
    public async Task<string> GetScheduleTableHtml()
    {
        var element = await _chromeDriver.WaitToFindElement(
            By.CssSelector("div[class='middle-slider--block shedule-slider hidden-xs owl-carousel owl-theme owl-loaded']"),
            15);

        var scheduleTableHtml = element.GetAttribute("innerHTML");
        return scheduleTableHtml;
    }
        
    public async Task<List<string>> GetAllGroupsHtml()
    {
        _chromeDriver.ExecuteScript(_getAllGroupsScript);
            
        await Task.Delay(TimeSpan.FromSeconds(10));
            
        var allGroupsJson = _chromeDriver.ExecuteScript("return JSON.stringify(window._allGroups);") as string;
         
        var allGroups = JsonConvert.DeserializeObject<GetAllGroupsResponse>(allGroupsJson);
            
        return allGroups.Rows.Select(r => r.Value.Trim()).ToList();
    }

    public void Dispose()
    {
        _chromeDriver.Dispose();
    }
}