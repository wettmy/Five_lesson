using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace кт5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var movies = new List<Movie>
            {
                new Movie("Мстители: Финал", 185),
                new Movie("Человек-паук: Новый день", 150),
                new Movie("Стражи Галактики", 121)
            };

            var hall = new CinemaHall(10);

            Console.WriteLine("Доступные фильмы:");
            for (int i = 0; i < movies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {movies[i]}");
            }

            Console.Write("\nВыберите номер фильма: ");
            if (!int.TryParse(Console.ReadLine(), out int movieIndex) ||
                movieIndex < 1 || movieIndex > movies.Count)
            {
                Console.WriteLine("Некорректный выбор фильма.");
                return;
            }
            var selectedMovie = movies[movieIndex - 1];

            Console.WriteLine("\nСхема зала:");
            hall.PrintSeats();

            Console.Write("\nВведите номер места для бронирования (1–10): ");
            if (!int.TryParse(Console.ReadLine(), out int seatNumber) ||
                seatNumber < 1 || seatNumber > hall.TotalSeats)
            {
                Console.WriteLine("Некорректный номер места.");
                return;
            }

            Console.Write("Введите ваше имя: ");
            string customerName = Console.ReadLine() ?? "Гость";
            var customer = new Customer(customerName);

            var bookingService = new BookingService(hall);
            bool success = bookingService.BookSeat(selectedMovie, customer, seatNumber);

            if (success)
            {
                Console.WriteLine($"\n✅ Бронирование успешно: {customerName} забронировал место {seatNumber} на фильм \"{selectedMovie.Title}\".");
            }
            else
            {
                Console.WriteLine($"\n❌ Не удалось забронировать место {seatNumber}: оно уже занято.");
            }

            Console.WriteLine("\nИтоговая схема зала:");
            hall.PrintSeats();

            Console.Write($"\nХотите отменить бронирование места {seatNumber}? (да/нет): ");
            string cancelInput = Console.ReadLine()?.Trim().ToLower();
            if (cancelInput == "да" || cancelInput == "yes")
            {
                bool canceled = bookingService.CancelBooking(seatNumber);
                if (canceled)
                {
                    Console.WriteLine($"\n✅ Бронирование места {seatNumber} отменено.");
                }
                else
                {
                    Console.WriteLine($"\n❌ Не удалось отменить бронирование: место не было забронировано.");
                }

                Console.WriteLine("\nСхема зала после отмены:");
                hall.PrintSeats();
            }
        }
    }

    public class Movie
    {
        public string Title { get; set; }
        public int DurationMinutes { get; set; }

        public Movie(string title, int durationMinutes)
        {
            Title = title;
            DurationMinutes = durationMinutes;
        }

        public override string ToString() => $"{Title} ({DurationMinutes} мин)";
    }

    public class Seat
    {
        public int Number { get; }
        private bool _isReserved;

        public Seat(int number)
        {
            Number = number;
            _isReserved = false;
        }

        public bool IsReserved => _isReserved;

        public bool Reserve()
        {
            if (_isReserved)
                return false;

            _isReserved = true;
            return true;
        }

        public void Unreserve()
        {
            _isReserved = false;
        }
    }

    public class CinemaHall
    {
        private List<Seat> _seats;
        public int TotalSeats => _seats.Count;

        public CinemaHall(int seatsCount)
        {
            _seats = new List<Seat>();
            for (int i = 1; i <= seatsCount; i++)
            {
                _seats.Add(new Seat(i));
            }
        }

        public List<Seat> GetSeats() => _seats;

        public Seat GetSeatByNumber(int number)
        {
            return _seats.Find(s => s.Number == number);
        }

        public void PrintSeats()
        {
            foreach (var seat in _seats)
            {
                string status = seat.IsReserved ? "[ЗАНЯТО]" : "[СВОБОДНО]";
                Console.WriteLine($"Место {seat.Number}: {status}");
            }
        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public List<int> ReservedSeats { get; set; }

        public Customer(string name)
        {
            Name = name;
            ReservedSeats = new List<int>();
        }
    }

    public class BookingService
    {
        private CinemaHall _hall;

        public BookingService(CinemaHall hall)
        {
            _hall = hall;
        }

        public bool BookSeat(Movie movie, Customer customer, int seatNumber)
        {
            var seat = _hall.GetSeatByNumber(seatNumber);
            if (seat == null)
                return false;

            if (seat.Reserve())
            {
                customer.ReservedSeats.Add(seatNumber);
                return true;
            }

            return false;
        }

        public bool CancelBooking(int seatNumber)
        {
            var seat = _hall.GetSeatByNumber(seatNumber);
            if (seat == null || !seat.IsReserved)
                return false;

            seat.Unreserve();
            return true;
        }
    }
}
