using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using OpenDDSharp.ShapesDemo.View;
using OpenDDSharp.ShapesDemo.Model;
using OpenDDSharp.ShapesDemo.Service;

namespace OpenDDSharp.ShapesDemo.ViewModel
{    
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Properties
        public static InteroperatibilityProvider Provider { get; set; }

        public MainWindowViewModel MainWindow
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // ViewModels
            SimpleIoc.Default.Register<MainWindowViewModel>();            

            // Services            
            SimpleIoc.Default.Register<IOpenDDSharpService, OpenDDSharpService>();            
            SimpleIoc.Default.Register<IConfigurationService, ConfigurationService>();
            SimpleIoc.Default.Register<IViewService, ViewService>();

            // Services initialization
            ServiceLocator.Current.GetInstance<IViewService>().RegisterView(typeof(WriterQosWindow), typeof(WriterQosViewModel));
            ServiceLocator.Current.GetInstance<IViewService>().RegisterView(typeof(ReaderQosWindow), typeof(ReaderQosViewModel));
            ServiceLocator.Current.GetInstance<IViewService>().RegisterView(typeof(ReaderFilterWindow), typeof(ReaderFilterViewModel));

            ServiceLocator.Current.GetInstance<IOpenDDSharpService>().Provider = Provider;           
        }
        #endregion

        #region Methods
        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<IOpenDDSharpService>().Dispose();
            ServiceLocator.Current.GetInstance<MainWindowViewModel>().Cleanup();                               
            ServiceLocator.Current.GetInstance<IViewService>().Dispose();
        }
        #endregion
    }
}