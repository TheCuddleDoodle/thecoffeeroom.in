﻿namespace Coffeeroom.Models.Domain
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int? AvatarId { get; set; }
        public string Bio { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? DateEdited { get; set; }
        public string Role { get; set; }
    }
}
