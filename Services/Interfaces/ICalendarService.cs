namespace Eventflow.Services.Interfaces
{
    public interface ICalendarService
    {
        string GenerateCalendarHtml(int year, int month);
    }
}
