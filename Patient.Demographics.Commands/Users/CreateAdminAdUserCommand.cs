using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Demographics.Commands.Users
{
    public class CreateAdminAdUserCommand: Command
    {
        public string Email { get; set; }

        public List<Guid> SetIds { get; set; }
        public List<Guid> RoleIds { get; set; }
        public Guid? UserId { get; set; }
        public int Status { get; set; }
        public bool? FileUploadSuccess { get; set; }
        public bool? FileUploadFailure { get; set; }
    }
}
