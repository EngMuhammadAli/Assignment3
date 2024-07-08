namespace Assignment3.Model
{
    public class AssignRoleModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        User User { get; set; }
        public int RoleId { get; set; }
        public Role Role  { get; set; }

    }
}
