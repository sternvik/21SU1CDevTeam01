using EntitetsLager;
using DataLager;

namespace AffärsLager.Controllers
{
    public class BestallningController
    {
        private readonly UnitOfWork _unitOfWork;

        public BestallningController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // TODO: Implementera beställningslogik
    }
}