using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using precioLuzApi.Services;
using precioLuzApi.DTOs;
using System.Threading.Tasks;

namespace precioLuzApi.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    public class PrecioController : ControllerBase
    {
        private readonly GetDataService _getDataService;

        public PrecioController(GetDataService getDataService)
        {
            _getDataService = getDataService;
        }

        [HttpGet]
        public JsonResult Get()
        {
            JsonResult result = new JsonResult("");

            Task<List<PriceDataDTO>> task = _getDataService.GetDataAsync();

            if (task.Result is not null)
            {
                result = new JsonResult(task.Result);
            }

            return result;
        }
    }
}