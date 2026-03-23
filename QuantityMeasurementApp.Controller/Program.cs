using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Repository;


namespace QuantityMeasurementApp.Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            IQuantityMeasurementRepository repository = QuantityMeasurementCacheRepository.GetInstance();
            IQuantityMeasurementService    service    = new QuantityMeasurementServiceImpl(repository);
            QuantityMeasurementController  controller = new QuantityMeasurementController(service);

            Menu menu = new Menu(controller);
            menu.Start();
        }
    }
}