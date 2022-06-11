using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface ISQLiteManager
    {
        Task<string> GetDatabasePath(); 
    }
}
