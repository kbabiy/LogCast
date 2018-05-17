using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AspNetCoreService.Tracing.Models;
using LogCast;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreService.Tracing.Controllers
{
    [Route("api/[controller]")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public class ValuesController : Controller
    {
        private readonly ValuesRepository _valuesRepository;

        private readonly ILogger _logger;

        public ValuesController()
        {
            _valuesRepository = new ValuesRepository();
            _logger = LogManager.GetLogger(GetType());
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (new LogCastContext())
            {
                _logger.Info("Operation started");
                Debug.Assert(LogCastContext.Current != null);
                var values = _valuesRepository.Get();
                _logger.Info("Values got from repository");
                Debug.Assert(LogCastContext.Current != null);
                return values;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
