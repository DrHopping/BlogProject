namespace Blog.Hashing
{
    public enum Argon2HashStrength
    {
        Interactive = 0, //32MB, t=4
        Medium, //64MB, t=4
        Moderate, //128MB, t=6
        Sensitive, //512MB, t=8
    }
}