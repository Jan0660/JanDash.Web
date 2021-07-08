using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazored.LocalStorage;
using JanDash.Data;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace JanDash
{
    public class JanDashService
    {
        private ILocalStorageService Storage { get; }
        private NavigationManager NavigationManager { get; }
        private DashUser SelfCached { get; set; }
        public List<Func<Task>> Update { get; set; } = new();

        public JanDashService(ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            Console.WriteLine("JanDashService.ctor");
            (Storage, NavigationManager) = (localStorageService, navigationManager);
        }

        public void Updated()
        {
            Task.Factory.StartNew(async () =>
            {
                foreach (var up in Update)
                {
                    await up();
                }
            });
        }

        public async Task<DashUser> GetSelf(bool loginRedirect = true)
        {
            Console.WriteLine(SelfCached?.Username ?? "not cached");
            if (SelfCached != null)
                return SelfCached;
            string token = null;
            try
            {
                token = await Storage.GetItemAsStringAsync("jandash-token");
            }
            catch
            {
                NavigationManager.NavigateTo(
                    $"redirect/{HttpUtility.UrlEncode(NavigationManager.Uri)}");
                return null;
            }

            if (token == null && loginRedirect)
            {
                NavigationManager.NavigateTo("login");
                 return null;
            }
            else if (token == null)
                return null;

            return SelfCached ??= Mongo.GetUserByToken(token);
        }

        public async Task<IEnumerable<Machine>> GetMachinesAsync()
            => GetMachinesByOwnerId((await GetSelf()).UserId);

        public static IEnumerable<Machine> GetMachinesByOwnerId(string ownerId)
            => Mongo.MachinesStore.Where(m => m.OwnerId == ownerId);

        public async Task NewMachine(string name)
        {
            var machine = new Machine(name, (await GetSelf()).UserId);
            Mongo.MachinesStore.Add(machine);
            await Mongo.MachinesCollection.InsertOneAsync(machine.ToBsonDocument());
        }

        public async Task<Machine> GetMachineAsync(string id)
        {
            var self = await GetSelf();
            var machine = Mongo.MachinesStore.First(m => m.MachineId == id);
            if (machine.OwnerId != self.UserId)
                // todo: maybe do something else yeah
                throw new Exception("Unauthorized.");
            return machine;
        }

        public async Task LogOut()
        {
            await Storage.RemoveItemAsync("jandash-token");
            SelfCached = null;
        }
    }
}