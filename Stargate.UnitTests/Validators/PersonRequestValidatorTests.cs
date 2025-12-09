using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stargate.Application.Validators;
using Stargate.Domain.Dtos;

namespace Stargate.UnitTests.Validators
{
    [TestClass]
    public class PersonRequestValidatorTests
    {
        private PersonRequestValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new PersonRequestValidator();
        }

        [TestMethod]
        public void Validate_WithValidName_ShouldNotHaveErrors()
        {
            // Arrange
            var request = new PersonRequest { Name = "John Doe" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Validate_WithEmptyName_ShouldHaveError()
        {
            // Arrange
            var request = new PersonRequest { Name = "" };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required");
        }

        [TestMethod]
        public void Validate_WithNameExceeding255Characters_ShouldHaveError()
        {
            // Arrange
            var request = new PersonRequest { Name = new string('A', 256) };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name cannot exceed 255 characters");
        }
    }
}
