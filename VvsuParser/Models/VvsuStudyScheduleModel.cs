using System.Collections.Generic;
using Newtonsoft.Json;

namespace VvsuParser.Models;

public class VvsuStudyScheduleWeek
{
    [JsonProperty("week")]
    public List<VvsuStudyScheduleDay> Week { get; set; }

    public VvsuStudyScheduleWeek(List<VvsuStudyScheduleDay> week)
    {
        Week = week;
    }
}
        
public class VvsuStudyScheduleDay
{
    [JsonProperty("day")]
    public string Day { get; set; }
        
    [JsonProperty("lessons")]
    public List<VvsuStudyScheduleLesson> Lessons { get; set; }

    public VvsuStudyScheduleDay(string day, List<VvsuStudyScheduleLesson> lessons)
    {
        Day = day;
        Lessons = lessons;
    }
}
        
public class VvsuStudyScheduleLesson
{
    [JsonProperty("time")]
    public string Time { get; set; }
        
    [JsonProperty("discipline")]
    public string Discipline { get; set; }

    [JsonProperty("teacher")]
    public string Teacher { get; set; }
        
    [JsonProperty("lessonForm")]
    public string LessonForm { get; set; }
        
    [JsonProperty("lectureHall")]
    public string LectureHall { get; set; }

    public VvsuStudyScheduleLesson(string time, string discipline, string teacher, string lessonForm, string lectureHall)
    {
        Time = time;
        Discipline = discipline;
        Teacher = teacher;
        LessonForm = lessonForm;
        LectureHall = lectureHall;
    }
}