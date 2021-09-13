using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCoreIdentity.Models.Model.Administration
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public bool Active { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
