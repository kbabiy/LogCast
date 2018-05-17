using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using LogCast;
using JetBrains.Annotations;
using WebApiService.Tracing.Models;

namespace WebApiService.Tracing.Controllers
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class StoreController : ApiController
    {
        private readonly ILogger _logger = LogManager.GetLogger();

        Item[] _stock = { 
            new Item { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 }, 
            new Item { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M }, 
            new Item { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M } 
        };

        public IEnumerable<Item> GetAllItems()
        {
            _logger.Info("All items");
            //throw new ArgumentException("Propblems!");
            return _stock;
        }

        public IHttpActionResult GetItem(int id)
        {
            _logger.Info("Starting with id" + id);
            var item = _stock.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _logger.Info("Finished");
            return Ok(item);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] string item)
        {
            _logger.Info("Starting with body: " + item);

            Thread.Sleep(100);

            _logger.Info("Finished");

            return Ok(_stock[0]);
        }
    }
}