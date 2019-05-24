namespace Scaffold.Domain.UnitTests.Exception
{
    using System;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class DomainExceptionUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingDomainException_Expect_DomainException()
            {
                // Arrange
                TestException exception;

                // Act
                exception = new TestException();

                // Assert
                Assert.NotNull(exception);
            }

            [Fact]
            public void When_InstantiatingDomainExceptionWithMessage_Expect_DomainExceptionWithMessage()
            {
                // Arrange
                TestException exception;
                string message = Guid.NewGuid().ToString();

                // Act
                exception = new TestException(message);

                // Assert
                Assert.Equal(message, exception.Message);
            }

            [Fact]
            public void When_InstantiatingDomainExceptionWithMessageAndInnerException_Expect_DomainExceptionWithMessageAndInnerException()
            {
                // Arrange
                TestException exception;

                string message = Guid.NewGuid().ToString();
                Exception innerException = new Exception();

                // Act
                exception = new TestException(message, innerException);

                // Assert
                Assert.Equal(message, exception.Message);
                Assert.Equal(innerException, exception.InnerException);
            }
        }

        public class SetDetail
        {
            [Fact]
            public void When_SettingDetail_Expect_DetailSet()
            {
                // Arrange
                TestException exception = new TestException();
                string value = Guid.NewGuid().ToString();

                // Act
                exception.Detail = value;

                // Assert
                Assert.Equal(value, exception.Detail);
            }
        }

        public class SetTitle
        {
            [Fact]
            public void When_SettingTitle_Expect_TitleSet()
            {
                // Arrange
                TestException exception = new TestException();
                string value = Guid.NewGuid().ToString();

                // Act
                exception.Title = value;

                // Assert
                Assert.Equal(value, exception.Title);
            }
        }

        private class TestException : DomainException
        {
            public TestException()
                : base()
            {
            }

            public TestException(string message)
                : base(message)
            {
            }

            public TestException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}
