using Eventflow.Application.Services;
using Eventflow.Application.Services.Interfaces;
using Eventflow.DTOs.DTOs;
using Moq;

namespace Eventflow.Tests.Services {
   [TestClass]
   public class CalendarServiceTests {
      private Mock<IPersonalEventService> _personalMock;
      private Mock<INationalEventService> _nationalMock;
      private CalendarService _service;

      [TestInitialize]
      public void Setup()
      {
         _personalMock = new Mock<IPersonalEventService>();
         _nationalMock = new Mock<INationalEventService>();
         _service = new CalendarService(_personalMock.Object, _nationalMock.Object);
      }

      [TestMethod]
      public void GenerateCalendar_WithPersonalEvents_ReturnsCorrectStructure()
      {
         // Arrange
         int year = 2025, month = 5;
         var testDate = new DateTime(year, month, 15);
         var personalEvents = new List<PersonalEventWithCategoryNameDto>
         {
            new() { Date = testDate, Title = "Test Event" }
         };

         // Act
         var result = _service.GenerateCalendar(year, month, personalEvents);

         // Assert
         var matchingDay = result.Days.FirstOrDefault(d => d.Date?.Date == testDate.Date);
         Assert.IsNotNull(matchingDay);
         Assert.AreEqual(1, matchingDay.PersonalEvents.Count);
         Assert.AreEqual("Test Event", matchingDay.PersonalEvents[0].Title);
      }

      [TestMethod]
      public void GenerateCalendar_WithNationalEvents_ReturnsCorrectStructure()
      {
         // Arrange
         int year = 2025, month = 5;
         var testDate = new DateTime(year, month, 1);
         var nationalEvents = new List<NationalEventDto>
         {
               new() { Date = testDate, CountryName = "National Day" }
         };

         // Act
         var result = _service.GenerateCalendar(year, month, nationalEvents);

         // Assert
         var matchingDay = result.Days.FirstOrDefault(d => d.Date?.Date == testDate.Date);
         Assert.IsNotNull(matchingDay);
         Assert.AreEqual(1, matchingDay.NationalEvents.Count);
         Assert.AreEqual("National Day", matchingDay.NationalEvents[0].CountryName);
      }

      [TestMethod]
      public void GenerateCalendar_WithoutEvents_FillsCalendarCorrectly()
      {
         // Act
         var result = _service.GenerateCalendar(2025, 1);

         // Assert
         Assert.AreEqual(2025, result.Year);
         Assert.AreEqual(1, result.Month);
         Assert.IsTrue(result.Days.Count % 7 == 0);
      }

      [TestMethod]
      public void GenerateEmptyCalendar_ReturnsAllDaysWithoutEvents()
      {
         // Act
         var result = _service.GenerateEmptyCalendar(2025, 1);

         // Assert
         Assert.AreEqual(2025, result.Year);
         Assert.AreEqual(1, result.Month);
         Assert.IsTrue(result.Days.All(d =>
            (d.DayNumber == null || d.PersonalEvents.Count == 0 && d.NationalEvents.Count == 0)));
      }

      [TestMethod]
      public async Task GenerateNationalHolidayCalendarAsync_ReturnsPopulatedCalendar()
      {
         // Arrange
         int year = 2025, month = 5, countryId = 1;
         var testDate = new DateTime(year, month, 9);
         _nationalMock.Setup(s => s.GetNationalHolidaysForCountryAsync(countryId, year, month))
               .ReturnsAsync(new List<NationalEventDto>
               {
                  new() { Date = testDate, CountryName = "Mock Holiday" }
               });

         // Act
         var result = await _service.GenerateNationalHolidayCalendarAsync(countryId, year, month);

         // Assert
         var matchingDay = result.Days.FirstOrDefault(d => d.Date?.Date == testDate.Date);
         Assert.IsNotNull(matchingDay);
         Assert.AreEqual("Mock Holiday", matchingDay.NationalEvents[0].CountryName);
      }

      [TestMethod]
      public async Task GenerateUserCalendarAsync_ReturnsCombinedEventCalendar()
      {
         // Arrange
         int year = 2025, month = 5, userId = 7;
         var testDate = new DateTime(year, month, 4);

         var own = new List<PersonalEventWithCategoryNameDto> {
               new() { Date = testDate, Title = "Own Event" }
         };
         var invited = new List<PersonalEventWithCategoryNameDto> {
               new() { Date = testDate, Title = "Invited Event" }
         };
         var global = new List<PersonalEventWithCategoryNameDto> {
               new() { Date = testDate, Title = "Global Event" }
         };

         _personalMock.Setup(s => s.GetEventsWithCategoryNamesAsync(userId, year, month)).ReturnsAsync(own);
         _personalMock.Setup(s => s.GetAcceptedInvitedEventsAsync(userId, year, month)).ReturnsAsync(invited);
         _personalMock.Setup(s => s.GetGlobalEventsWithCategoryAsync(year, month)).ReturnsAsync(global);

         // Act
         var result = await _service.GenerateUserCalendarAsync(userId, year, month);

         // Assert
         var matchingDay = result.Days.FirstOrDefault(d => d.Date?.Date == testDate.Date);
         Assert.IsNotNull(matchingDay);
         Assert.AreEqual(3, matchingDay.PersonalEvents.Count);
      }

      [TestMethod]
      public void GenerateCalendar_AllowsMultipleEventsOnSameDay()
      {
         // Arrange
         var date = new DateTime(2025, 6, 15);
         var events = new List<PersonalEventWithCategoryNameDto>
         {
            new() { Date = date, Title = "Morning Run" },
            new() { Date = date, Title = "Evening Yoga" }
         };

         // Act
         var result = _service.GenerateCalendar(2025, 6, events);
         var day = result.Days.FirstOrDefault(d => d.Date?.Date == date.Date);

         // Assert
         Assert.IsNotNull(day);
         Assert.AreEqual(2, day.PersonalEvents.Count);
      }
   }
}