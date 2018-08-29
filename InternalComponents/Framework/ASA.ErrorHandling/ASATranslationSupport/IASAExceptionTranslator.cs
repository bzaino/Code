using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.ErrorHandling
{
    public interface IASAExceptionTranslator
    {
        /// <summary>
        /// Translate the given <see cref="System.SystemException"/> into a generic data access exception.
        /// </summary>
        /// <param name="exception">
        /// The <see cref="System.Exception"/> encountered by the ADO.NET implementation.
        /// </param>
        /// <returns>
        /// A  appropriate for the supplied
        /// <paramref name="exception"/>.
        /// </returns>
        ASAException Translate(Exception exception);
    }
}
