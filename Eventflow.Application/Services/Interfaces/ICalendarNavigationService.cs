namespace Eventflow.Application.Services.Interfaces
{
    public interface ICalendarNavigationService
    {
        (int year, int month) Normalize(int? year, int? month);
    }
}
