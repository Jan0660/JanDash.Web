using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JanDash.Data;
using JanDash.Shared;
using Microsoft.AspNetCore.Mvc;

namespace JanDash.Controllers
{
    [Route("/api/machines/")]
    public class MachinesController : Controller
    {
        [HttpGet("{id}/{token}")]
        public Machine GetMachine(string id, string token)
            => Mongo.MachinesStore.First(m => m.MachineId == id && m.Token == token);
        [HttpPost("{id}/{token}/updatePersistent")]
        public async Task UpdatePersistent(string id, string token)
        {
            var machine = GetMachine(id, token);
            machine.Info = await JsonSerializer.DeserializeAsync<MachineInfo>(Request.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            await machine.UpdateAsync();
            machine.Updated();
        }
        [HttpPost("{id}/{token}/updateOther")]
        public async Task UpdateOther(string id, string token)
        {
            var machine = GetMachine(id, token);
            machine.OtherInfo = await JsonSerializer.DeserializeAsync<MachineOtherInfo>(Request.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            machine.Updated();
        }
        [HttpPost("{id}/{token}/updateMemory")]
        public async Task UpdateMemory(string id, string token)
        {
            var machine = GetMachine(id, token);
            machine.MemoryInfo = await JsonSerializer.DeserializeAsync<MachineMemoryInfo>(Request.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            machine.Updated();
        }
    }
}