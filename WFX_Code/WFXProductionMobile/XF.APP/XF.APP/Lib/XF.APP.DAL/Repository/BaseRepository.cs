using AutoMapper;
using XF.APP.ABSTRACTION;
using XF.APP.DATA;

namespace XF.APP.DAL
{
    public class BaseRepository : IBaseRepository
    {
        protected ApplicationContext _dbContext;
        protected IMapper mapper;

        public BaseRepository(IMapper mapper)
        {
            //this._dbContext = new ApplicationContext(Constants.DbPath);
            this.mapper = mapper;
        }
    }
}
