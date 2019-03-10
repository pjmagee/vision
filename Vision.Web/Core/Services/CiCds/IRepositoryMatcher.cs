namespace Vision.Web.Core
{
    public interface IRepositoryMatcher
    {
        bool IsSameRepository(string one, string two);
    }
}
