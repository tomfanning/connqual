using connqual_server.Services;
using Microsoft.AspNetCore.Mvc;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace connqual_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FramesController : ControllerBase
    {
        private readonly FrameService frameService;

        public FramesController(FrameService frameService)
        {
            this.frameService = frameService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<List<WrappedFrame>> Get()
        {
            var filtered = frameService.Frames.Where(f => DateTime.UtcNow - f.Received < TimeSpan.FromMinutes(1));

            return filtered.Select(f => new WrappedFrame(f.Frame, f.Received)).ToList();
        }
    }
}