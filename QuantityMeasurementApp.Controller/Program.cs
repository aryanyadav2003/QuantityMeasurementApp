using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Repository.Repositories;
using QuantityMeasurementApp.Repository.Utilities;

namespace QuantityMeasurementApp.Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfig config = AppConfig.Instance;

            IQuantityMeasurementRepository repository;

            if (config.RepositoryType == "database")
                repository = new QuantityMeasurementDatabaseRepository();
            else
                repository = QuantityMeasurementCacheRepository.GetInstance();

            IQuantityMeasurementService   service    = new QuantityMeasurementServiceImpl(repository);
            QuantityMeasurementController controller = new QuantityMeasurementController(service);

            Menu menu = new Menu(controller);
            menu.Start();

            repository.ReleaseResources();
        }
    }
}