using FlipkartProj.Entities;

namespace FlipkartProj.Support
{
    public static class BookingDataGenerator
    {
        private static readonly Random Rng = new();

        private static readonly string[] FirstNames =
            { "Aarav", "Diya", "Mohan", "Meera", "Rohan", "Sana", "Vihaan", "Isha" };

        private static readonly string[] LastNames =
            { "Sharma", "Iyer", "Kumar", "Patel", "Nair", "Reddy", "Gupta", "Menon" };

        private static readonly string[] Needs =
            { "Breakfast", "Late checkout", "Airport pickup", "Extra bed" };

        public static Booking Create()
        {
            var checkIn = DateTime.Today.AddDays(Rng.Next(1, 30));
            var checkOut = checkIn.AddDays(Rng.Next(1, 10));

            return new Booking
            {
                FirstName = FirstNames[Rng.Next(FirstNames.Length)],
                // Random suffix keeps every run's data unique
                LastName = $"{LastNames[Rng.Next(LastNames.Length)]}{Rng.Next(1000, 9999)}",
                TotalPrice = Rng.Next(100, 1000),
                DepositPaid = Rng.Next(2) == 0,
                BookingDates = new BookingDates
                {
                    CheckIn = checkIn.ToString("yyyy-MM-dd"),
                    CheckOut = checkOut.ToString("yyyy-MM-dd")
                },
                AdditionalNeeds = Needs[Rng.Next(Needs.Length)]
            };
        }
    }
}