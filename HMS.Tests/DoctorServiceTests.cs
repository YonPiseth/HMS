using Xunit;
using Moq;
using HMS.Services;
using HMS.Repositories;
using HMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HMS.Tests
{
    public class DoctorServiceTests
    {
        private readonly Mock<IRepository<Doctor>> _mockRepo;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _mockRepo = new Mock<IRepository<Doctor>>();
            _service = new DoctorService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllDoctorsAsync_ShouldReturnListFromRepository()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorID = 1, FirstName = "John", LastName = "Doe" },
                new Doctor { DoctorID = 2, FirstName = "Jane", LastName = "Smith" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(doctors);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
        }

        [Fact]
        public async Task AddDoctorAsync_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var doctor = new Doctor { FirstName = "New", LastName = "Doctor" };
            _mockRepo.Setup(r => r.InsertAsync(It.IsAny<Doctor>())).ReturnsAsync(true);

            // Act
            var result = await _service.AddDoctorAsync(doctor);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.InsertAsync(It.IsAny<Doctor>()), Times.Once);
        }
    }
}
