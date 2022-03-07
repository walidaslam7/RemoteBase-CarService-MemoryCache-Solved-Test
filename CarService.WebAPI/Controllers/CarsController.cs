using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarService.WebAPI.Data;
using CarService.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CarService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarsService _carsService;
        private readonly ImCache _memoryCache;
        private readonly string key = "car";
        public CarsController(ICarsService carsService, ImCache memoryCache)
        {
            _carsService = carsService;
            _memoryCache = memoryCache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = (await _carsService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] Filters filters)
        {
            var cache = _memoryCache.GetCacheByKey(key);
            if(cache != null)
                return Ok(cache);

            var cars = await _carsService.Get(null, filters);
            return Ok(cars);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Car car)
        {
            List<Car> cars = new List<Car>();
            await _carsService.Add(car);
            cars.Add(car);
            var cache = _memoryCache.GetCacheByKey(key);
            if (cache != null)
                cars.AddRange(cache);

            _memoryCache.RemoveCacheByKey(key);
            _memoryCache.CreateCache(cars, key);
            return Ok(car);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = (await _carsService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();

            await _carsService.Delete(user);
            _memoryCache.RemoveFromCacheById(id,key);
            return NoContent();
        }
    }
}
