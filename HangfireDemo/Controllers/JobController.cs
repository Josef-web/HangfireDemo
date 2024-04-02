using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard.Resources;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public IActionResult CreateBackgroundJob()
        {
            BackgroundJob.Enqueue<TestJob>(x => x.WriteLog("Background Job Triggered"));
            return Ok();
        }
        
        [HttpPost]
        [Route("CreateScheduledJob")]
        public IActionResult CreateScheduledJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job Triggered"),dateTimeOffset);
            return Ok();
        }
        
        [HttpPost]
        [Route("CreateContinuationJob")]
        public IActionResult CreateContinuationJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            var jobId = BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job 2 Triggered"), dateTimeOffset);

            var job2Id = BackgroundJob.ContinueJobWith<TestJob>(jobId,x => x.WriteLog("Continuation Job 1 Triggered"));
            var job3Id = BackgroundJob.ContinueJobWith<TestJob>(job2Id,x => x.WriteLog("Continuation Job 2 Triggered"));
            var job4Id = BackgroundJob.ContinueJobWith<TestJob>(job3Id,x => x.WriteLog("Continuation Job 3 Triggered"));
            
            return Ok();
        }
        
        [HttpPost]
        [Route("CreateRecurringJob")]
        public IActionResult CreateRecurringJob()
        {
            RecurringJob.AddOrUpdate<TestJob>("RecurringJob1", x => x.WriteLog("Recurring Job Triggered"),"* * * * * *");
            return Ok();
        }
        
        
    }
}
