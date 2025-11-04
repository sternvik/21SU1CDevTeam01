using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// ExtraController - Tom placeholder-klass för framtida funktionalitet
    /// Kan användas för extravaror, tillval eller andra tilläggstjänster
    /// </summary>
    public class ExtraController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Frigör resurser och stänger databasanslutningar
        /// </summary>
        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}
