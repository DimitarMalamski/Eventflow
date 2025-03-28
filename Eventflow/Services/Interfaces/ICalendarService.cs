namespace Eventflow.Services.Interfaces
{
    public interface ICalendarService
    {
        public string GenerateCalendarHtml(int year, int month);
    }
}
