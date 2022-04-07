using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VvsuParser.Models;

namespace VvsuParser.VvsuStudySchedule;

public class VvsuStudyScheduleParser
{
    public async Task<List<VvsuStudyScheduleWeek>> ParseAsync(HtmlDocument htmlDocument)
    {
        try
        {
            var studyScheduleModels = new List<VvsuStudyScheduleWeek>();

            var scheduleWeekTables = htmlDocument
                .DocumentNode
                .SelectNodes("//div[contains(@class, 'owl-item')]");

            foreach (var scheduleWeekTable in scheduleWeekTables)
            {
                var week = ParseWeekSchedule(scheduleWeekTable);
                studyScheduleModels.Add(week);
            }
                
            return studyScheduleModels;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private VvsuStudyScheduleWeek ParseWeekSchedule(HtmlNode htmlNode)
    {
        var lessonsDict = new Dictionary<string, List<VvsuStudyScheduleLesson>>();
            
        var dateKey = string.Empty;
            
        var lessonsTr = htmlNode
            .ChildNodes.FindFirst("tbody")
            .ChildNodes;

        foreach (var lessonTr in lessonsTr)
        {
            if(string.IsNullOrEmpty(lessonTr.InnerHtml.Trim()))
                continue;
                
            var lesson = ParseLesson(lessonTr);
            if(lesson.Lesson == null)
                continue;

            if (!string.IsNullOrEmpty(lesson.Date))
                dateKey = lesson.Date;
                
            if (!lessonsDict.ContainsKey(dateKey))
                lessonsDict.Add(dateKey, new List<VvsuStudyScheduleLesson>() { lesson.Lesson });
            else
                lessonsDict[dateKey].Add(lesson.Lesson);
                
        }

        var days = lessonsDict
            .Select(item => new VvsuStudyScheduleDay(item.Key, item.Value))
            .ToList();
            
        return new VvsuStudyScheduleWeek(days);
    }

    private (string Date, VvsuStudyScheduleLesson Lesson) ParseLesson(HtmlNode lessonTr)
    {
        var nodes = lessonTr
            .ChildNodes
            .Where(el => !string.IsNullOrEmpty(el.InnerHtml.Trim()))
            .ToList();
            
        if(!nodes.Any())
            return (string.Empty, null);

        if (nodes.Count == 6)
        {
            var dateItems = nodes[0]?.InnerHtml.Split("<br>");
            var date = string.Join(' ', dateItems).Trim() ?? "Не определенно";
            var lessonTime = nodes[1]?.InnerText.Trim() ?? "Не определенно";
            var lessonDiscipline= nodes[2].InnerText.Trim() ?? "Не определенно";
            var lessonForm = nodes[3].InnerText.Trim() ?? "Не определенно";
            var lessonHall = nodes[4].InnerText.Trim() ?? "Не определенно";
            var lessonTeacher= nodes[5].InnerText.Trim() ?? "Не определенно";
            
            return (date, new VvsuStudyScheduleLesson(lessonTime, lessonDiscipline, lessonTeacher, lessonForm, lessonHall));
        }

        if (nodes.Count == 5)
        {
            var lessonTime = nodes[0]?.InnerText.Trim() ?? "Не определенно";
            var lessonDiscipline= nodes[1].InnerText.Trim() ?? "Не определенно";
            var lessonForm = nodes[2].InnerText.Trim() ?? "Не определенно";
            var lessonHall = nodes[3].InnerText.Trim() ?? "Не определенно";
            var lessonTeacher= nodes[4].InnerText.Trim() ?? "Не определенно";
                
            return (string.Empty, new VvsuStudyScheduleLesson(lessonTime, lessonDiscipline, lessonTeacher, lessonForm, lessonHall));
        }
            
        return (string.Empty, null);
    }
}