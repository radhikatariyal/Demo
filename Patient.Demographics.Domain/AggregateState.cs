using System;

namespace Patient.Demographics.Domain
{
    [Serializable]
    public enum AggregateState
    {
        NotModified = 0,
        New = 1,
        Modified = 2,
        Deleted = 3
    }
}