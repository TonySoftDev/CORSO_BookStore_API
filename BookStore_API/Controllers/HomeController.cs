using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Controller API di prova
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Legge e restituisce tutti gli elementi
        /// </summary>
        /// <returns></returns>
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Test del log - INFO INFO INFO");
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Legge e restituisce un singolo elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<HomeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogDebug("Test del log - DEBUG DEBUG DEBUG");
            return "value";
        }

        // POST api/<HomeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("Test del log - ERRORE ERRORE ERRORE");
        }

        // PUT api/<HomeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("Test del log - Avviso Avviso Avviso");
        }
    }
}
