using Eventflow.Application.Services;
using Eventflow.Application.Services.Interfaces;

namespace Eventflow.Tests.Services {
   [TestClass]
   public class CalendarNavigationServiceTests {
      private ICalendarNavigationService _service;

      [TestInitialize]
      public void Setup() {
         _service = new CalendarNavigationService();
      }

      [TestMethod]
      public void Normalize_ReturnsCurrentYearAndMonth_WhenInputsAreNull() {
         // Arrange
         var expected = (DateTime.Now.Year, DateTime.Now.Month);

         // Act
         var actual = _service.Normalize(null, null);

         // Assert
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void Normalize_HandlesMonthLessThanOne_ByWrappingToPreviousYear()
      {
         // Arrange
         int inputYear = 2025;
         int inputMonth = 0;

         // Act
         var (year, month) = _service.Normalize(inputYear, inputMonth);

         // Assert
         Assert.AreEqual(2024, year);
         Assert.AreEqual(12, month);
      }

      [TestMethod]
      public void Normalize_HandlesMonthGreaterThanTwelve_ByWrappingToNextYear()
      {
         // Arrange
         int inputYear = 2025;
         int inputMonth = 13;

         // Act
         var (year, month) = _service.Normalize(inputYear, inputMonth);

         // Assert
         Assert.AreEqual(2026, year);
         Assert.AreEqual(1, month);
      }

      [TestMethod]
      public void Normalize_ReturnsSameYearAndMonth_WhenWithinBounds()
      {
         // Arrange
         int inputYear = 2025;
         int inputMonth = 7;

         // Act
         var (year, month) = _service.Normalize(inputYear, inputMonth);

         // Assert
         Assert.AreEqual(2025, year);
         Assert.AreEqual(7, month);
      }
   }
}