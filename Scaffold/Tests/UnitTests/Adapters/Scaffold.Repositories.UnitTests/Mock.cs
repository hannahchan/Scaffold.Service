namespace Scaffold.Repositories.UnitTests;

using System;

// Contains shared and reusable mocks
public static class Mock
{
    public class ModelWithAllTimestamps
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }

    public class ModelWithNoTimestamps
    {
        public int Id { get; set; }
    }

    public class ModelWithNonNullableTimestamps
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public DateTime DeletedAt { get; set; }
    }

    public class ModelWithCreatedAtTimestampOnly
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ModelWithLastModifiedAtTimestampOnly
    {
        public int Id { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }

    public class ModelWithDeletedAtTimestampOnly
    {
        public int Id { get; set; }

        public DateTime? DeletedAt { get; set; }
    }

    public class ModelWithLastModifiedAtNonNullableTimestampOnly
    {
        public int Id { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }

    public class ModelWithDeletedAtNonNullableTimestampOnly
    {
        public int Id { get; set; }

        public DateTime DeletedAt { get; set; }
    }

    public class ModelWithStringTimestamps
    {
        public int Id { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }

        public string DeletedAt { get; set; }
    }
}
