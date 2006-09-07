/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 13/07/2006
 * Time: 19:47
 *
 */

using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace Sycorax.AutoUpdate {
    static class Program {
        /// <summary>
        /// This method starts the service.
        /// </summary>
        static void Main () {
            ServiceBase.Run(new AutoUpdateService());
        }
    }
}