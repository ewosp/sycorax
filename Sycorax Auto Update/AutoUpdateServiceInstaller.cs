using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.ComponentModel;

namespace Sycorax.AutoUpdate {
    [RunInstallerAttribute(true)]
    class AutoUpdateServiceInstaller : Installer {
        public AutoUpdateServiceInstaller () {
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            //Le service fonctionne sous l'account Local System
            processInstaller.Account = ServiceAccount.LocalSystem;

            //Paramètres du service
            serviceInstaller.ServiceName = "SycoraxAutoUpdate";
            serviceInstaller.Description = AssemblyDescription;
            serviceInstaller.DisplayName = AssemblyTitle;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        #region Réflexion d'AssemblyInfo.cs
        /// <summary>
        /// Description de l'assembly
        /// </summary>
        public string AssemblyDescription {
            get {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyTitle {
            get {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0) {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        #endregion
    }
}