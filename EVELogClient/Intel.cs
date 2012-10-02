using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVELogMonitor;

/*
 * Not currently used, I though it might be worth keeping around though
 */
namespace EVELogClient
{

    class Store
    {
        private static Dictionary<string, StarSystem> systemIntel = new Dictionary<string, StarSystem>();

        //add 
        public static void updateSystem(string system, string pilot)
        {
            if (!systemIntel.ContainsKey(system))
            {
                systemIntel.Add(system, new StarSystem(system));
            }
            systemIntel[system].addPilot(pilot);
        }

        //cull the store 
        public static void cull()
        {
            foreach (StarSystem system in systemIntel.Values)
            {
                system.cull();
            }
        }
    }

    class StarSystem
    {
        public readonly string name;
        private Dictionary<string, Pilot> pilots = new Dictionary<string, Pilot>();
        //might want to add more info, like positions in system or some such

        public StarSystem(string n)
        {
            name = n;
        }

        public List<Pilot> getPilots()
        {
            return new List<Pilot>(pilots.Values);
        }

        public void addPilot(string name)
        {
            Console.WriteLine(this.name + ": " + name);
            pilots[name] = new Pilot(name);
        }

        public void cull()
        {
            foreach (Pilot p in pilots.Values)
            {
                TimeSpan diff = new DateTime() - p.lastSeen;
                if (diff.TotalMinutes > Properties.EXPIRY)
                {
                    pilots.Remove(p.name);
                }
            }
        }

    }

    class Pilot
    {
        public readonly string name;
        public readonly DateTime lastSeen = new DateTime();

        public Pilot(string n)
        {
            name = n;
        }
    }
}
