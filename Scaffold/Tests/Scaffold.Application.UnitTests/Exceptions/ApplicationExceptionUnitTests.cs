namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class ApplicationExceptionUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingApplicationException_Expect_ApplicationException()
            {
                // Arrange
                TestException exception;

                // Act
                exception = new TestException();

                // Assert
                Assert.NotNull(exception);
            }

            [Fact]
            public void When_InstantiatingApplicationExceptionWithMessage_Expect_ApplicationExceptionWithMessage()
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
            public void When_InstantiatingApplicationExceptionWithMessageAndInnerException_Expect_ApplicationExceptionWithMessageAndInnerException()
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

        private class TestException : Scaffold.Application.Exceptions.ApplicationException
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
