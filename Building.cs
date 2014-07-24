namespace CookieClicker
{
    /// <summary>
    /// A cookie-producing structure that can be purchased with cookies
    /// </summary>
    class Building
    {
        //price to purchase in cookies
        public double Cost;

        //cookies per second produced when owned
        public double Cps;

        public string Name;

        //time to earn the amount of cookies used to purchase (from this building alone)
        public double TimeToRecoupCost
        {
            get
            {
                return Cost / Cps;
            }
        }

        public double TimeToPurchase(double currentCps)
        {
            return Cost / currentCps;
        }
    }
}
