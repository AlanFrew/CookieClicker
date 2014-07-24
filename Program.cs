using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieClicker
{
    class Program
    {
        //the end number to shoot for; orders are slightly different based on the cutoff point
        static int goalInCpS = 1000000;

        //cookies per second being earned
        static double currentCps = 0.1;

        //time since game start
        static double seconds = 0.0;

        //the solution
        static List<string> finalPurchaseOrder = new List<string>();

        //key = name of building, value = number owned
        static Dictionary<string, int> buildingsOwned = new Dictionary<string, int>();

        //the possible choices to build
        static List<Building> buildingOptions = new List<Building>(10) {
            new Building {Cost = 15 * 1.15, Cps = 0.1, Name = "Cursor"},
            new Building {Cost = 100, Cps = 0.5, Name = "Grandma"},
            new Building {Cost = 500, Cps = 4, Name = "Farm"},
            new Building {Cost = 3000, Cps = 10, Name = "Factory"},
            new Building {Cost = 10000, Cps = 40, Name = "Mine"},
            new Building {Cost = 40000, Cps = 100, Name = "Shipment"},
            new Building { Cost = 200000, Cps = 400, Name = "AlchemyLab" },
            new Building { Cost = 1666666, Cps = 6666, Name = "Portal" },
            new Building { Cost = 123456789, Cps = 98765, Name = "TimeMachine" },
            new Building { Cost = 3999999999, Cps = 999999, Name = "AntimatterCondenser" }
        };

        static void Main(string[] args)
        {
            //If you want the goal to be defined by the user:
            //goalInCpS = Int32.Parse(args[0]);   //You wouldn't pass an invalid argument, would you?         
            
            //only option available to start
            buildingsOwned.Add("Cursor", 1);
            Console.WriteLine("Start by buying 1 cursor");

            RunMainLoop();

            Console.ReadLine();     //keep window open in debug mode
        }

        /// <summary>
        /// Returns the building that gives the most CpS for its cost
        /// </summary>
        public static Building FindMostEfficientPurchase(List<Building> buildingOptions)
        {
            return buildingOptions.OrderBy(building => building.TimeToRecoupCost).First();
        }

        /// <summary>
        /// Returns the cheapest building that will pay for itself by the time the most efficient option would have been purchased
        /// </summary>
        public static Building FindCheaperOption(List<Building> cheaperBuildings, Building potentialBuyTarget)
        {
            foreach (var buildingOption in cheaperBuildings)
            {
                var delay = buildingOption.TimeToPurchase(currentCps) + potentialBuyTarget.TimeToPurchase(currentCps + buildingOption.Cps) - potentialBuyTarget.TimeToPurchase(currentCps);

                var timeOfDelayedPurchase = buildingOption.TimeToPurchase(currentCps) + potentialBuyTarget.TimeToPurchase(currentCps + buildingOption.Cps);

                var extraCookiesBanked = (timeOfDelayedPurchase - buildingOption.TimeToPurchase(currentCps)) * buildingOption.Cps;

                var cookiesLost = potentialBuyTarget.Cps * (timeOfDelayedPurchase - potentialBuyTarget.TimeToPurchase(currentCps));

                if (extraCookiesBanked > cookiesLost) potentialBuyTarget = buildingOption;
            }

            return potentialBuyTarget;
        }

        /// <summary>
        /// Add selected building to total owned; add to solution; update gamestate
        /// </summary>
        public static void PurchaseBuilding(Dictionary<string, int> buildingsOwned, Building buildingBought)
        {
            if (!buildingsOwned.ContainsKey(buildingBought.Name)) buildingsOwned.Add(buildingBought.Name, 0);

            buildingsOwned[buildingBought.Name]++;

            seconds += buildingBought.TimeToPurchase(currentCps);

            currentCps += buildingBought.Cps;

            finalPurchaseOrder.Add(buildingBought.Name);
        }

        /// <summary>
        /// Does the heavy lifting and prints out the solution to console
        /// </summary>
        /// <remarks>The algorithm: find the most efficient purchase (MEP), and then build the cheapest building that will pay for itself by the time the MEP could be bought, or the MEP itself if none exists. Repeat.</remarks>
        public static void RunMainLoop()
        {
            while (currentCps < goalInCpS)
            {
                //start by looking for the most CpS per unit of cost in cookies
                var potentialBuyTarget = FindMostEfficientPurchase(buildingOptions);

                var timeToBestPurchase = potentialBuyTarget.Cost / currentCps;

                //see if there is anything cheaper that is worth buying first
                var cheaperBuildings = buildingOptions.Where(building => building.Cost < potentialBuyTarget.Cost).OrderByDescending(building => building.Cost).ToList();
                potentialBuyTarget = FindCheaperOption(cheaperBuildings, potentialBuyTarget);

                PurchaseBuilding(buildingsOwned, potentialBuyTarget);     

                Console.Write(potentialBuyTarget.Name + " for " + Math.Round(potentialBuyTarget.Cost) + ", total " + buildingsOwned[potentialBuyTarget.Name]);
                Console.WriteLine("\t\t" + currentCps + " CpS after " + Math.Round(seconds) + " sec");

                //each building gets more expensive as you buy more
                potentialBuyTarget.Cost *= 1.15;
            }
        }
    }
}
