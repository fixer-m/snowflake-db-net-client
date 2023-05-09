namespace Snowflake.Client.Model
{
    public interface IAuthInfo
    {
        string Account { get; set; }
        string User { get; set; }
        string Region { get; set; }

        string ToString();
    }
}