using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Moq;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private CategoryService _categoryService;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowException_WhenRepositoryIsNull()
        {
            // Act
            var service = new CategoryService(null!);
        }

        [TestMethod]
        public async Task GetAllCategoriesAsync_ShouldReturnSortedList_WhenRepositoryReturnsUnsorted()
        {
            // Arrange
            var unsortedCategories = new List<Category>
            {
                new Category { Id = 2, Name = "Work" },
                new Category { Id = 1, Name = "Personal" },
                new Category { Id = 3, Name = "Outside" }
            };

            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(unsortedCategories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Outside", result[0].Name);
            Assert.AreEqual("Personal", result[1].Name);
            Assert.AreEqual("Work", result[2].Name);
        }

        [TestMethod]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(new List<Category>());

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryRetrievalException))]
        public async Task GetAllCategoriesAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ThrowsAsync(new Exception("Fails!"));

            // Act
            await _categoryService.GetAllCategoriesAsync();
        }

        [TestMethod]
        public async Task GetAllCategoriesAsync_ShouldIgnoreNullOrWhitespaceNames()
        {
            var unsortedCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Work" },
                new Category { Id = 2, Name = "" },
                new Category { Id = 3, Name = "  " },
                new Category { Id = 4, Name = "Family" }
            };

            _mockCategoryRepository
                .Setup(repo => repo.GetAllCategoriesAsync())
                .ReturnsAsync(unsortedCategories);

            var result = await _categoryService.GetAllCategoriesAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Family", result[0].Name);
            Assert.AreEqual("Work", result[1].Name);
        }
    }
}
