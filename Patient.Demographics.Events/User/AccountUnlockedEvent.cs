using System;

namespace Patient.Demographics.Events.User
{
    public class AccountUnlockedEvent : Event
    {
        public AccountUnlockedEvent(Guid userId)
        {
            UserId = userId;

        }
        public Guid UserId { get; }
    }
}