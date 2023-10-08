using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace CalSample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void GenerateICalClicked(object sender, EventArgs e)
    {
        string summary = SummaryEntry.Text;
        DateTime startDate = StartDatePicker.Date;
        DateTime endDate = EndDatePicker.Date;

        FrequencyType frequency =
            (FrequencyType)Enum.Parse(typeof(FrequencyType), FrequencyPicker.SelectedItem.ToString(), true);
        int interval = Convert.ToInt32(IntervalEntry.Text);
        DayOfWeek? dayOfWeek = DayOfWeekPicker.SelectedItem.ToString() == "None"
            ? null
            : (DayOfWeek)Enum.Parse(typeof(DayOfWeek), DayOfWeekPicker.SelectedItem.ToString());
        int offset = Convert.ToInt32(OffsetEntry.Text);

        var recurrence = new RecurrencePattern
        {
            Frequency = frequency,
            Interval = interval,
            ByDay = dayOfWeek != null
                ? new List<WeekDay> { new WeekDay { DayOfWeek = dayOfWeek.Value, Offset = offset } }
                : null,
            Until = RecurrenceEndDatePicker.Date
        };

        var icalEvent = new CalendarEvent
        {
            Summary = summary,
            Start = new CalDateTime(startDate.Year, startDate.Month, startDate.Day),
            End = new CalDateTime(endDate.Year, endDate.Month, endDate.Day),
            IsAllDay = true,
            RecurrenceRules = new List<RecurrencePattern> { recurrence }
        };

        var calendar = new Calendar();
        calendar.Events.Add(icalEvent);

        var serializer = new CalendarSerializer();
        string icalContent = serializer.SerializeToString(calendar);

        string filePath = "event.ics";

        string popoverTitle = "Read ical file";
        string file = System.IO.Path.Combine(FileSystem.CacheDirectory, filePath);

        System.IO.File.WriteAllText(file, icalContent);

        Launcher.Default.OpenAsync(new OpenFileRequest(popoverTitle, new ReadOnlyFile(file))).ConfigureAwait(false);
    }
}