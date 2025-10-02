using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffärsLager.Controllers
{
    public class ExtraController
    {

        private UnitOfWork _unitOfWork = new UnitOfWork();

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }

    }
}
