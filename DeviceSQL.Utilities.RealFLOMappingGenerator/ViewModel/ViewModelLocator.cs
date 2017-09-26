#region Imported Types

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

#endregion

namespace DeviceSQL.Utilities.RealFLOMappingGenerator.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {

        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {

        }
    }
}