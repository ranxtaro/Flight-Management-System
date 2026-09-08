using System;

namespace Lab1_OOP_Bradul.Models
{
    public class Flight
    {
        // Public fields
        public string FlightNumber;
        public string Destination;
        public DateTime DepartureTime;
        public FlightStatus Status;
        public bool IsInternational;

        // Private fields
        private int passengerCount;
        private double ticketPrice;

        // Properties for private fields
        public int PassengerCount
        {
            get { return passengerCount; }
        }

        public double TicketPrice
        {
            get { return ticketPrice; }
        }

        // Constructor
        public Flight(
            string flightNumber,
            string destination,
            DateTime departureTime,
            int passengerCount,
            double ticketPrice,
            FlightStatus status,
            bool isInternational)
        {
            FlightNumber = flightNumber;
            Destination = destination;
            DepartureTime = departureTime;
            this.passengerCount = passengerCount;
            this.ticketPrice = ticketPrice;
            Status = status;
            IsInternational = isInternational;
        }

        // 1. Add passenger
        public bool AddPassenger()
        {
            if (passengerCount >= 500)
                return false;

            passengerCount++;
            return true;
        }

        // 2. Delay flight
        public void DelayFlight(int minutes)
        {
            if (minutes <= 0)
                return;

            DepartureTime = DepartureTime.AddMinutes(minutes);
            Status = FlightStatus.Delayed;
        }

        // 3. Start boarding
        public void StartBoarding()
        {
            if (Status != FlightStatus.Cancelled &&
                Status != FlightStatus.Departed &&
                Status != FlightStatus.Landed)
            {
                Status = FlightStatus.Boarding;
            }
        }

        // 4. Cancel flight
        public void CancelFlight()
        {
            Status = FlightStatus.Cancelled;
        }
    }
}