using System.ComponentModel.DataAnnotations;

namespace Assignment3.Model
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
