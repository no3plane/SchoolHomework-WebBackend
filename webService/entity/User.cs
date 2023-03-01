namespace webService
{
    public enum Role
    {
        Admin,
        NormalUser,
        UserNonExistent
    }
    class User
    {
        public int id;
        public string name;
        public string password;
        public string token;
        public int sex;
        public Role role;
        public string avatar;
    }
}