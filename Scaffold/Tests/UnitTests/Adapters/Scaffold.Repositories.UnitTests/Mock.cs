namespace Scaffold.Repositories.UnitTests
{
    using System;

    // Contains shared and reusable mocks
    public static class Mock
    {
        public class ModelWithNoTimestamps
        {
            public int Id { get; set; }
        }

        public class ModelWithCreatedAtTimestampOnly
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        public class ModelWithLastModifiedAtTimestampOnly
        {
            public int Id { get; set; }

            public DateTime LastModifiedAt { get; set; }
        }

        public class ModelWithNonNullableLastModifiedAtTimestamp
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime LastModifiedAt { get; set; }
        }

        public class ModelWithNullableLastModifiedAtTimestamp
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? LastModifiedAt { get; set; }
        }

        public class ModelWithStringTimestamps
        {
            public int Id { get; set; }

            public string CreatedAt { get; set; }

            public string LastModifiedAt { get; set; }
        }
    }
}
