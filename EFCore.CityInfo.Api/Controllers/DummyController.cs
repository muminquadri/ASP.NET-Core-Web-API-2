using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.CityInfo.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EFCore.CityInfo.Api.Controllers
{      
    public class DummyController : Controller
    {
        private CityInfoContext _context;
        public DummyController(CityInfoContext context)
        {
            _context = context;  
        }
        [HttpGet]
        [Route("api/seed")]
        public IActionResult SeedDatabase()
        {
            return Ok();
        }
    }
}