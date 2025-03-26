namespace Eventflow.Services.Interfaces
{
    public interface ICalendarService
    {
        public Task<string> GenerateCalendarHtmlAsync(int year, int month);
    }
}
