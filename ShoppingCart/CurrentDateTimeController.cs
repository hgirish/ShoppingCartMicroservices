using Microsoft.AspNetCore.Mvc;
using System;

namespace ShoppingCart
{
    public class CurrentDateTimeController : ControllerBase
    {
        [HttpGet("/")]
        public object Get() => DateTime.UtcNow;
    }
}
